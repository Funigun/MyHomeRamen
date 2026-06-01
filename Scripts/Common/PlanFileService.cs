using System.Text.RegularExpressions;

public class PlanFile
{
    public string Path { get; private set; }

    public string Title { get; private set; }

    public Dictionary<string, string> Sections { get; private set; }

    public PlanFile(string path, string title, Dictionary<string, string> sections)
    {
        Path = path;
        Title = title;
        Sections = sections;
    }

    public string GetRawTableSectionContent()
    {
        string header = PlanFileConstants.FilesSectionHeader.Replace("## ", "").Trim();
        Sections.TryGetValue(header, out string? content);
        return content ?? string.Empty;
    }

    public bool TryGetFilesTableSection(out string[] tableLines, out string? message)
    {
        string? section2Key = Sections.Keys.FirstOrDefault(k => k == PlanFileConstants.FilesSectionHeader.Replace("## ", "").Trim());
        tableLines = [];

        if (section2Key is null)
        {
            message = "[slice-scaffold] #2 table not found in plan";
            return false;
        }

        tableLines = Sections[section2Key].Split('\n')
                                          .Where(l => l.TrimStart().StartsWith('|'))
                                          .ToArray();

        if (tableLines.Length < 3)
        {
            message = "[slice-scaffold] #2 table empty or malformed";
            return false;
        }

        message = null;
        return true;
    }
}

public class PlanFileService
{
    public PlanFile LoadPlanFile(string filePath)
    {
        string planContent = File.ReadAllText(filePath);

        Dictionary<string, string> planSections = [];

        string[] sections = planContent.Split(new[] { "## " }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string section in sections)
        {
            string[] lines = section.Split('\n', 2);
            if (lines.Length > 1)
            {
                string title = lines[0].Trim();
                string content = lines[1].Trim();
                planSections[title] = content;
            }
        }

        string planTitle = planContent.Split('\n').FirstOrDefault(l => l.StartsWith("# Plan:")) ?? "Untitled";

        return new(filePath, planTitle, planSections);
    }
}
