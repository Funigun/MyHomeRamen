using System.Text.RegularExpressions;

public static class DiffParser
{
    // Returns lowercased forward-slash paths added by the diff (--- /dev/null + +++ b/... pattern)
    public static IReadOnlyList<string> GetAddedPaths(string diffPath)
    {
        string[] lines = File.ReadAllLines(diffPath);
        List<string> added = [];

        for (int i = 0; i < lines.Length; i++)
        {
            Match match = Regex.Match(lines[i], @"^\+\+\+ b/(.+)$");
            if (match.Success && i > 0 && lines[i - 1].StartsWith("--- /dev/null"))
                added.Add(match.Groups[1].Value.Replace('\\', '/').ToLowerInvariant());
        }

        return added;
    }
}
