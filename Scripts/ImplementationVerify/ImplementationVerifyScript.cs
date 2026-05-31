#:include CheckResult.cs
#:include StepRunner.cs
#:include PlanParser.cs
#:include DiffParser.cs
#:include ReportRenderer.cs

using System.Text.RegularExpressions;

if (args.Length < 1)
{
    Console.WriteLine("[verifier] usage: dotnet run ImplementationVerifyScript.cs -- <PlanPath> [DiffPath]");
    return 3;
}

string planPath = args[0];
string diffPath = args.Length > 1 ? args[1] : string.Empty;

if (!File.Exists(planPath))
{
    Console.WriteLine($"[verifier] plan not found: {planPath}");
    return 3;
}

string planFilename = Path.GetFileName(planPath);
bool isBackend = planFilename.EndsWith("backend-plan.md", StringComparison.OrdinalIgnoreCase);

if (isBackend)
{
    if (string.IsNullOrEmpty(diffPath))
    {
        string planDir = Path.GetDirectoryName(planPath)!;
        string planBase = Path.GetFileNameWithoutExtension(planPath);
        string diffBase = Regex.Replace(planBase, "backend-plan$", "diff");
        diffPath = Path.Combine(planDir, $"{diffBase}.patch");
    }

    if (!File.Exists(diffPath))
    {
        Console.WriteLine($"[verifier] diff not found: {diffPath}");
        return 3;
    }
}

string repoRoot = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(planPath)!, "..", ".."));
string reportPath = Path.Combine(Path.GetDirectoryName(planPath)!, "verify-report.md");
string fence = "```";

List<CheckResult> results = [];
List<string> failureNotes = [];
IReadOnlyList<string> plannedAll = [];

// ---------- Pre-checks (backend scope only) -----------------------------------

if (isBackend)
{
    plannedAll = PlanParser.GetCreatedPaths(planPath);
    IReadOnlyList<string> diffAdded = DiffParser.GetAddedPaths(diffPath);

    // Pre-1: test completeness
    IReadOnlyList<string> plannedTests = plannedAll
        .Where(p => p.StartsWith("myhomeramen.unittests/") ||
                    p.StartsWith("myhomeramen.integrationtests/") ||
                    p.StartsWith("myhomeramen.architecturetests/"))
        .ToList();

    IReadOnlyList<string> missingTests = plannedTests.Except(diffAdded).ToList();

    if (missingTests.Count == 0)
    {
        results.Add(new CheckResult("Pre: test completeness", "PASS"));
    }
    else
    {
        string missingList = string.Join(", ", missingTests);
        results.Add(new CheckResult("Pre: test completeness", "FAIL", $"missing: {missingList}"));
        failureNotes.Add($"Plan §2 calls for these test files but the diff does not add them:\n  {string.Join("\n  ", missingTests)}");
    }

    // Pre-2: migration check
    string planText = File.ReadAllText(planPath);
    bool migrationPlanned = planText.Contains("dotnet ef migrations add", StringComparison.OrdinalIgnoreCase);
    IReadOnlyList<string> migrationFiles = diffAdded
        .Where(p => Regex.IsMatch(p, @"myhomeramen\.persistance/[^/]+/migrations/"))
        .ToList();

    if (!migrationPlanned)
    {
        results.Add(new CheckResult("Pre: migration check", "N/A"));
    }
    else if (migrationFiles.Count > 0)
    {
        results.Add(new CheckResult("Pre: migration check", "PASS", $"found: {string.Join(", ", migrationFiles)}"));
    }
    else
    {
        results.Add(new CheckResult("Pre: migration check", "FAIL", "plan mentions a migration but no file under MyHomeRamen.Persistance/*/Migrations/ in diff"));
        failureNotes.Add("Plan calls for `dotnet ef migrations add ...` but the diff has no MyHomeRamen.Persistance/*/Migrations/* file.");
    }
}
else
{
    results.Add(new CheckResult("Pre: test completeness", "N/A", "frontend scope — diff pre-checks skipped"));
    results.Add(new CheckResult("Pre: migration check", "N/A", "frontend scope — diff pre-checks skipped"));
}

bool preFailed = results.Any(r => r.Status == "FAIL");

// ---------- Build / test (skipped on pre-fail) --------------------------------

if (!preFailed)
{
    StepResult buildRun = StepRunner.Run("dotnet", "build MyHomeRamen.slnx --nologo", repoRoot);
    string buildStatus = buildRun.ExitCode == 0 ? "PASS" : "FAIL";
    results.Add(new CheckResult("dotnet build", buildStatus, "-", buildRun.DurationSec));

    if (buildRun.ExitCode != 0)
    {
        failureNotes.Add("Build failed; skipping tests.");
        IEnumerable<string> buildTail = buildRun.Output.Split('\n').TakeLast(80);
        failureNotes.Add($"Build output (tail):\n{fence}\n{string.Join(Environment.NewLine, buildTail)}\n{fence}");
        results.Add(new CheckResult("Unit tests", "SKIPPED"));
        results.Add(new CheckResult("Architecture tests", "SKIPPED"));
        results.Add(new CheckResult("Integration tests", "SKIPPED"));
    }
    else
    {
        StepResult unit = StepRunner.Run("dotnet", "test MyHomeRamen.UnitTests/MyHomeRamen.UnitTests.csproj --nologo --no-build", repoRoot);
        string unitStatus = unit.ExitCode == 0 ? "PASS" : "FAIL";
        results.Add(new CheckResult("Unit tests", unitStatus, "-", unit.DurationSec));
        if (unit.ExitCode != 0)
        {
            IEnumerable<string> tail = unit.Output.Split('\n').TakeLast(80);
            failureNotes.Add($"Unit tests output (tail):\n{fence}\n{string.Join(Environment.NewLine, tail)}\n{fence}");
        }

        StepResult arch = StepRunner.Run("dotnet", "test MyHomeRamen.ArchitectureTests/MyHomeRamen.ArchitectureTests.csproj --nologo --no-build", repoRoot);
        string archStatus = arch.ExitCode == 0 ? "PASS" : "FAIL";
        results.Add(new CheckResult("Architecture tests", archStatus, "-", arch.DurationSec));
        if (arch.ExitCode != 0)
        {
            IEnumerable<string> tail = arch.Output.Split('\n').TakeLast(80);
            failureNotes.Add($"Arch tests output (tail):\n{fence}\n{string.Join(Environment.NewLine, tail)}\n{fence}");
        }

        bool needsIntegration = plannedAll.Any(p => p.StartsWith("myhomeramen.integrationtests/"));
        if (needsIntegration)
        {
            StepResult itg = StepRunner.Run("dotnet", "test MyHomeRamen.IntegrationTests/MyHomeRamen.IntegrationTests.csproj --nologo --no-build", repoRoot);
            string itgStatus = itg.ExitCode == 0 ? "PASS" : "FAIL";
            results.Add(new CheckResult("Integration tests", itgStatus, "-", itg.DurationSec));
            if (itg.ExitCode != 0)
            {
                IEnumerable<string> tail = itg.Output.Split('\n').TakeLast(80);
                failureNotes.Add($"Integration tests output (tail):\n{fence}\n{string.Join(Environment.NewLine, tail)}\n{fence}");
            }
        }
        else
        {
            results.Add(new CheckResult("Integration tests", "SKIPPED", "plan does not include MyHomeRamen.IntegrationTests/*"));
        }
    }
}
else
{
    foreach (string key in new[] { "dotnet build", "Unit tests", "Architecture tests", "Integration tests" })
        results.Add(new CheckResult(key, "SKIPPED", "pre-check failed"));
}

// ---------- Render report ------------------------------------------------------

string report = ReportRenderer.Render(results, failureNotes);
File.WriteAllText(reportPath, report, new System.Text.UTF8Encoding(false));

// ---------- Stdout summary -----------------------------------------------------

string summary = string.Join(" ", results.Select(r => $"{r.Name}={r.Status}"));
Console.WriteLine($"[verifier] {summary}");

bool overallFail = results.Any(r => r.Status == "FAIL");
string overall = overallFail ? "FAIL" : "PASS";
Console.WriteLine($"[verifier] overall: {overall}");
Console.WriteLine($"[verifier] report: {reportPath}");

return overallFail ? 1 : 0;
