using System.Text.RegularExpressions;

public static class PlanParser
{
    // Returns lowercased forward-slash paths from §2 table rows where Action == "create"
    public static IReadOnlyList<string> GetCreatedPaths(string planPath)
    {
        string[] lines = File.ReadAllLines(planPath);
        List<string> paths = [];

        bool inSection2 = false;
        bool headerParsed = false;
        int pathColIdx = -1;
        int actionColIdx = -1;

        foreach (string line in lines)
        {
            if (Regex.IsMatch(line, @"^##\s+2\."))
            {
                inSection2 = true;
                continue;
            }

            if (inSection2 && Regex.IsMatch(line, @"^##\s+[3-9]\."))
                break;

            if (!inSection2 || !line.TrimStart().StartsWith('|'))
                continue;

            string[] cells = line.Split('|').Select(c => c.Trim()).ToArray();
            // cells[0] is empty (leading |); real data starts at index 1
            string[] dataCells = cells[1..^1];

            if (!headerParsed)
            {
                for (int i = 0; i < dataCells.Length; i++)
                {
                    if (string.Equals(dataCells[i], "Path", StringComparison.OrdinalIgnoreCase))
                        pathColIdx = i;
                    if (string.Equals(dataCells[i], "Action", StringComparison.OrdinalIgnoreCase))
                        actionColIdx = i;
                }

                if (pathColIdx >= 0 && actionColIdx >= 0)
                    headerParsed = true;

                continue;
            }

            if (dataCells.Length == 0 || dataCells[0].All(c => c == '-' || c == ':' || c == ' '))
                continue;

            if (dataCells.Length <= Math.Max(pathColIdx, actionColIdx))
                continue;

            string path = dataCells[pathColIdx].Trim('`');
            string action = dataCells[actionColIdx];

            if (action.StartsWith("create", StringComparison.OrdinalIgnoreCase))
                paths.Add(path.Replace('\\', '/').ToLowerInvariant());
        }

        return paths;
    }
}
