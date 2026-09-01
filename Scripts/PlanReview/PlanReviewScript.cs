#:include */Common/PlanFileService.cs
#:include */Common/FileType.cs
#:include */Common/PlanFileConstants.cs

using System.Text.RegularExpressions;

string featurePlanPath = args[0];

if (!File.Exists(featurePlanPath))
{
    Console.WriteLine($"[slice-scaffold] plan not found: {featurePlanPath}");
    return;
}

string repoRoot = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(featurePlanPath)!, "..", "..", ".."));

PlanFileService planFileService = new();
PlanFile planFile = planFileService.LoadPlanFile(featurePlanPath);

Console.WriteLine($"[plan-verifier] processing plan: {featurePlanPath}");
Console.WriteLine($"[plan-verifier] repo root: {repoRoot}");

List<string> issues = [];
HashSet<string> validModules = ["Menu", "Orders", "ShoppingCart", "Reservations", "Payments", "Users"];

// Title checks
if (planFile.Title == "Untitled")
{
    issues.Add("[plan-verifier] plan title is missing");
}

Match titleMatch = Regex.Match(planFile.Title, @"^# Plan: (.*) - .*$");

if (!titleMatch.Success)
{
    issues.Add("[plan-verifier] plan title has wrong format");
}
else
{
    string module = titleMatch.Groups[1].Value.Trim();
    if (!validModules.Contains(module))
    {
        issues.Add($"[plan-verifier] unknown module '{module}' in plan title. Valid modules: {string.Join(", ", validModules)}");
    }
}

// Section header checks
foreach (string header in planFile.Sections.Keys)
{
    if (header.StartsWith("# Plan:"))
    {
        continue; // Skip the title header which is already checked
    }

    if (!PlanFileConstants.ExpectedSectionHeaders.Contains($"## {header}"))
    {
        issues.Add($"[plan-verifier] unexpected section header: '{header}'");
    }
}

// Table section checks
string[] tableLines = planFile.GetRawTableSectionContent().Split('\n').Where(line => !string.IsNullOrEmpty(line.Trim())).ToArray();

if(tableLines.Count() < 3)
{
    issues.Add("[plan-verifier] #2 table section is empty or malformed");
}
else
{
    IEnumerable<string> expectedColumns = ["Action", "Module", "Aggregate", "Feature Name", "Endpoint Kind", "Route", "DB Query Options Required", "Policies"];
    int expectedColumnCount = expectedColumns.Count();

    IEnumerable<string> headerCols = tableLines.First().Trim().Trim('|').Split('|').Select(c => c.Trim()).ToList();
    if (headerCols.Count() < expectedColumnCount || !expectedColumns.SequenceEqual(headerCols))
    {
        issues.Add("[plan-verifier] #2 table header is malformed or missing required columns");
    }

    for (int i = 2; i < tableLines.Count(); i++)
    {
        string[] cols = tableLines.ElementAt(i).Trim().Trim('|').Split('|').Select(c => c.Trim()).ToArray();
        string pathColumn = cols[0].Trim('`');
        string actionColumn = cols.Length > 1 ? cols[1].Trim() : string.Empty;
        string typeColumn = cols.Length > 2 ? cols[2].Trim() : string.Empty;

        if (string.IsNullOrWhiteSpace(pathColumn))
        {
            issues.Add($"[plan-verifier] #2 table row {i + 1} has empty file path");
        }

        if (string.IsNullOrWhiteSpace(actionColumn))
        {
            issues.Add($"[plan-verifier] #2 table row {i + 1} has empty action");
        }
        else if (!Enum.TryParse<FileAction>(actionColumn, true, out _))
        {
            issues.Add($"[plan-verifier] #2 table row {i + 1} has invalid action '{actionColumn}'");
        }
    }
}

// Output results
if (issues.Count == 0)
{
    Console.WriteLine("[plan-verifier] no issues found in plan");
}
else
{
    Console.WriteLine($"[plan-verifier] {issues.Count} issue(s) found in plan:");
    foreach (string issue in issues)
    {
        Console.WriteLine(issue);
    }
}
