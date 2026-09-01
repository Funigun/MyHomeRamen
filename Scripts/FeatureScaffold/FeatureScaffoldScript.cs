#:include FeatureDetails.cs
#:include FileFactory.cs


string planPath = args[0];

if(!File.Exists(planPath))
{
    Console.WriteLine($"[feature-scaffold] plan not found: {planPath}");
    return;
}

const string tableHeader = "## 2. Files to create / modify";
const string constructorsHeader = "## 2.1 Constructors";
const string domainChangesHeader = "## 3. Domain changes";

string fileContent = File.ReadAllText(planPath);

string tableContent = GetSectionContent(fileContent, tableHeader, constructorsHeader);
string constructorsContent = GetSectionContent(fileContent, constructorsHeader, domainChangesHeader);

if (string.IsNullOrWhiteSpace(tableContent))
{
    Console.WriteLine("[feature-scaffold] table section not found or empty");
    return;
}

IEnumerable<string> tableLines = tableContent.Split('\n')
                                             .Skip(2) // Skip the header and separator lines
                                             .Where(l => l.TrimStart().StartsWith('|'))
                                             .ToArray();

List<string> created = [];
List<string> skipped = [];
List<string> notHandled = [];
List<string> removed = [];

string repoRoot = $@"C:\Users\{Environment.UserName}\source\repos\MyHomeRamen";
const string filePathTemplate = "MyHomeRamen.Features.{Module}.Features.{Aggregate}.{Feature}.{TypeName}.cs";

foreach (string line in tableLines)
{
    FeatureDetails feature = FeatureDetails.Create(line, constructorsContent);

    switch (feature.Action)
    {
        case "create":
            HandleFeatureToCreate(feature, repoRoot, filePathTemplate, created, notHandled);
            break;

        case "modify":
            HandleFileToUpdate(feature, repoRoot, filePathTemplate, skipped, notHandled);
            break;

        case "delete":
            HandleFileToDelete(feature, repoRoot, filePathTemplate, removed, notHandled);
            break;

        default:
            notHandled.Add(feature.Name);
            break;
    }
}



private static string GetSectionContent(string content, string sectionHeader, string nextSectionHeader)
{
    int startIndex = content.IndexOf(sectionHeader);
    if (startIndex == -1)
    {
        return "";
    }

    int endIndex = content.IndexOf(nextSectionHeader, startIndex + sectionHeader.Length);
    if (endIndex == -1)
    {
        return "";
    }

    string sectionContent = content.Substring(startIndex + sectionHeader.Length, endIndex - (startIndex + sectionHeader.Length));
    return sectionContent.Trim();
}

private static void HandleFeatureToCreate(FeatureDetails featureDetails, string repoRoot, string filePathTemplate, List<string> createdFiles, List<string> notHandledFiles)
{
    string endpointfilePath = GenerateFilePath(featureDetails, $"{featureDetails.Name}Endpoint", repoRoot, filePathTemplate);
    string commandfilePath = GenerateFilePath(featureDetails, $"{featureDetails.Name}{featureDetails.Command.Type}", repoRoot, filePathTemplate);

    if (!File.Exists(endpointfilePath) && !File.Exists(commandfilePath))
    {
        string endpointContent = FileFactory.CreateEndpoint(featureDetails);
        string commandContent = FileFactory.CreateCqrs(featureDetails);

        File.WriteAllText(endpointfilePath, endpointContent);
        File.WriteAllText(commandfilePath, commandContent);
        createdFiles.Add($"{featureDetails.Name}Endpoint");
        createdFiles.Add($"{featureDetails.Name}{featureDetails.Command.Type}");
    }
    else
    {
        notHandledFiles.Add(featureDetails.Name);
    }
}

private static void HandleFileToUpdate(FeatureDetails featureDetails, string repoRoot, string filePathTemplate, List<string> skippedFiles, List<string> notHandledFiles)
{
    string filePath = GenerateFilePath(featureDetails, $"{featureDetails.Name}{featureDetails.Command.Type}", repoRoot, filePathTemplate);

    string endpointFilePath = GenerateFilePath(featureDetails, $"{featureDetails.Name}Endpoint", repoRoot, filePathTemplate);

    if (File.Exists(endpointFilePath) || File.Exists(filePath))
    {
        skippedFiles.Add(featureDetails.Name);
    }
    else
    {
        notHandledFiles.Add(featureDetails.Name);
    }
}

private static void HandleFileToDelete(FeatureDetails featureDetails, string repoRoot, string filePathTemplate, List<string> removedFiles, List<string> notHandledFiles)
{
    string endpointFilePath = GenerateFilePath(featureDetails, $"{featureDetails.Name}Endpoint", repoRoot, filePathTemplate);
    string cqrsFilePath = GenerateFilePath(featureDetails, $"{featureDetails.Name}{featureDetails.Command.Type}", repoRoot, filePathTemplate);

    if (File.Exists(endpointFilePath) || File.Exists(cqrsFilePath))
    {
        if (File.Exists(endpointFilePath))
        {
            File.Delete(endpointFilePath);
        }

        if (File.Exists(cqrsFilePath))
        {
            File.Delete(cqrsFilePath);
        }

        removedFiles.Add(featureDetails.Name);
    }
    else
    {
        notHandledFiles.Add(featureDetails.Name);
    }
}

private static string GenerateFilePath(FeatureDetails featureDetails, string fileName, string repoRoot, string filePathTemplate)
{
    string filePath = filePathTemplate.Replace("{Module}", featureDetails.Module)
                                      .Replace("{Aggregate}", featureDetails.Aggregate)
                                      .Replace("{Feature}", featureDetails.Name)
                                      .Replace("{TypeName}", fileName);

    return Path.Combine(repoRoot, filePath);
}
