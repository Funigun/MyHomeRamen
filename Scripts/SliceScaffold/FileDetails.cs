public class FileDetails
{
    public string Path { get; init; }

    public FileAction Action { get; init; }

    public FileType Type { get; init; }

    public string Notes { get; init; }

    public FeatureDetails Feature { get; init; }

    public static FileDetails Create(string inputFileLine)
    {
        string[] parts = inputFileLine.Split('|');

        string path = parts[1].Replace("`", "").Trim();
        FileType type = parts[3].Trim().ToFileType();

        return new FileDetails
        {
            Path = path,
            Action = parts[2].Trim().ToFileAction(),
            Type = type,
            Notes = parts.Length > 4 ? parts[4].Trim() : string.Empty,
            Feature = FeatureDetails.Create(path, type)
        };
    }
}

public class FeatureDetails
{
    public string Module { get; init; }

    public string Entity { get; init; }

    public string Subfolder { get; init; }

    public string TypeName { get; init; }

    public static FeatureDetails Create(string filePath, FileType type)
    {
        return type switch
        {
            FileType.Request          => CreateForApiContract(filePath),
            FileType.Response         => CreateForApiContract(filePath),
            FileType.UnitTest         => CreateForTestFile(filePath),
            FileType.IntegrationTest  => CreateForTestFile(filePath),
            FileType.Unknown          => null,
            _                         => CreateForApiSlice(filePath)
        };
    }

    // MyHomeRamen.Common.Contracts\{Module}\{Entity}\Requests|Responses\{TypeName}.cs
    private static FeatureDetails CreateForApiContract(string filePath)
    {
        string normalized = filePath.Replace('\\', '/');
        System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(
            normalized,
            @"^MyHomeRamen\.Common\.Contracts/([^/]+)/([^/]+)/([^/]+)/([^/]+)\.cs$");

        if (!m.Success)
            return null;

        return new FeatureDetails
        {
            Module    = m.Groups[1].Value,
            Entity    = m.Groups[2].Value,
            Subfolder = m.Groups[3].Value,
            TypeName  = m.Groups[4].Value
        };
    }

    // MyHomeRamen.{Tests}\{Module}Module\{Entity}\{TypeName}.cs
    private static FeatureDetails CreateForTestFile(string filePath)
    {
        string normalized = filePath.Replace('\\', '/');
        System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(
            normalized,
            @"^MyHomeRamen\.[^/]+/([^/]+)Module/([^/]+)/([^/]+)\.cs$");

        if (!m.Success)
            return null;

        return new FeatureDetails
        {
            Module    = m.Groups[1].Value,
            Entity    = m.Groups[2].Value,
            Subfolder = string.Empty,
            TypeName  = m.Groups[3].Value
        };
    }

    // MyHomeRamen.{Project}\{Module}\Features\{Entity}\{Feature}\{TypeName}.cs
    // Subfolder holds the Feature name (e.g. ClearBasket)
    private static FeatureDetails CreateForApiSlice(string filePath)
    {
        string normalized = filePath.Replace('\\', '/');
        System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(
            normalized,
            @"^MyHomeRamen\.[^/]+/([^/]+)/Features/([^/]+)/([^/]+)/([^/]+)\.cs$");

        if (!m.Success)
            return null;

        return new FeatureDetails
        {
            Module    = m.Groups[1].Value,
            Entity    = m.Groups[2].Value,
            Subfolder = m.Groups[3].Value,
            TypeName  = m.Groups[4].Value
        };
    }
}
