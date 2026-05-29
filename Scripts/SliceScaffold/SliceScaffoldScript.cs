#:include FileDetails.cs
#:include FileScaffoldFactory.cs

using System.IO;
using System.Text.RegularExpressions;

string featurePlanPath = args[0];

if (!File.Exists(featurePlanPath))
{
    Console.WriteLine($"[slice-scaffold] plan not found: {featurePlanPath}");
    return;
}

// Derive repo root the same way PS1 does: two levels up from the plan file's directory
// Plan files live under .github/plans/{Feature}/ → go up 3 levels
string repoRoot = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(featurePlanPath)!, "..", "..", ".."));

string planContent = File.ReadAllText(featurePlanPath);
Dictionary<string, string> planSections = [];

string[] sections = planContent.Split(new[] { "## " }, StringSplitOptions.RemoveEmptyEntries);
foreach (string section in sections)
{
    string[] lines = section.Split('\n', 2);
    if (lines.Length > 1)
    {
        string title   = lines[0].Trim();
        string content = lines[1].Trim();
        planSections[title] = content;
    }
}

Console.WriteLine($"[slice-scaffold] processing plan: {featurePlanPath}");
Console.WriteLine($"[slice-scaffold] repo root: {repoRoot}");

// Find section 2
string? section2Key = planSections.Keys.FirstOrDefault(k => Regex.IsMatch(k, @"^2\."));
if (section2Key is null)
{
    Console.WriteLine("[slice-scaffold] #2 table not found in plan");
    return;
}

// Parse table rows (skip header row 0 and divider row 1)
string[] tableLines = planSections[section2Key]
    .Split('\n')
    .Where(l => l.TrimStart().StartsWith('|'))
    .ToArray();

if (tableLines.Length < 3)
{
    Console.WriteLine("[slice-scaffold] #2 table empty or malformed");
    return;
}

List<FileDetails> entries = [];
for (int i = 2; i < tableLines.Length; i++)
{
    // Strip leading/trailing pipe then split
    string trimmed = tableLines[i].Trim().Trim('|');
    string[] cols   = trimmed.Split('|').Select(c => c.Trim().Trim('`')).ToArray();
    if (cols.Length < 2 || string.IsNullOrWhiteSpace(cols[0]))
        continue;

    entries.Add(FileDetails.Create(tableLines[i]));
}

// Process
List<string> created     = [];
List<string> skipped     = [];
List<string> modified    = [];
List<string> unsupported = [];

foreach (FileDetails entry in entries)
{
    if (entry.Action == FileAction.Modify)
    {
        modified.Add(entry.Path);
        continue;
    }

    if (entry.Action != FileAction.Create)
    {
        unsupported.Add($"{entry.Path} (unknown action)");
        continue;
    }

    if (entry.Type == FileType.Unknown)
    {
        unsupported.Add($"{entry.Path} (unknown type)");
        continue;
    }

    if (entry.Feature is null)
    {
        unsupported.Add($"{entry.Path} (could not parse path for type '{entry.Type}')");
        continue;
    }

    string scaffold = FileScaffoldFactory.CreateFileScaffold(entry);
    if (string.IsNullOrEmpty(scaffold))
    {
        unsupported.Add($"{entry.Path} (no scaffold template for type '{entry.Type}')");
        continue;
    }

    string absolutePath = Path.Combine(repoRoot, entry.Path);
    if (File.Exists(absolutePath))
    {
        skipped.Add(entry.Path);
        continue;
    }

    _ = Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
    File.WriteAllText(absolutePath, scaffold, new System.Text.UTF8Encoding(false));
    created.Add(entry.Path);
}

Console.WriteLine();
Console.WriteLine($"[slice-scaffold] created:     {created.Count}");
foreach (string x in created)     Console.WriteLine($"  + {x}");
Console.WriteLine($"[slice-scaffold] skipped:     {skipped.Count} (already exist)");
foreach (string x in skipped)     Console.WriteLine($"  = {x}");
Console.WriteLine($"[slice-scaffold] modify rows: {modified.Count} (hand-edit required)");
foreach (string x in modified)    Console.WriteLine($"  ~ {x}");
Console.WriteLine($"[slice-scaffold] unsupported: {unsupported.Count}");
foreach (string x in unsupported) Console.WriteLine($"  ? {x}");
