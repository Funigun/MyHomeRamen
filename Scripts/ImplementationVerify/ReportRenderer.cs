public static class ReportRenderer
{
    public static string Render(IEnumerable<CheckResult> results, IEnumerable<string> failureNotes)
    {
        List<CheckResult> resultList = [.. results];
        List<string> noteList = [.. failureNotes];

        bool overallFail = resultList.Any(r => r.Status == "FAIL");
        string overall = overallFail ? "FAIL" : "PASS";

        System.Text.StringBuilder sb = new();
        sb.AppendLine("# Verify report");
        sb.AppendLine();
        sb.AppendLine("| Check | Result | Duration | Details |");
        sb.AppendLine("|-------|--------|----------|---------|");

        foreach (CheckResult r in resultList)
        {
            string dur = r.DurationSec > 0 ? $"{Math.Round(r.DurationSec, 2)}s" : "-";
            sb.AppendLine($"| {r.Name} | {r.Status} | {dur} | {r.Details} |");
        }

        sb.AppendLine();
        sb.AppendLine($"## Overall: {overall}");
        sb.AppendLine();

        if (noteList.Count > 0)
        {
            sb.AppendLine("## Failure tail");
            foreach (string note in noteList)
            {
                sb.AppendLine(note);
                sb.AppendLine();
            }
        }

        return sb.ToString();
    }
}
