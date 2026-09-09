#:include FeatureDetails.cs
#:include FileFactory.cs

string planPath = args[0];

if(!File.Exists(planPath))
{
    Console.WriteLine($"[feature-scaffold] plan not found: {planPath}");
    return;
}

const string tableHeader = "## 2. Files to create / modify";
const string domainChangesHeader = "## 3. Domain changes";

string fileContent = File.ReadAllText(planPath);

string tableContent = GetSectionContent(fileContent, tableHeader, domainChangesHeader);

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

foreach (string line in tableLines)
{
    FeatureDetails feature = FeatureDetails.Create(line);

    switch (feature.Action)
    {
        case "create":
            HandleFeatureToCreate(feature, created, notHandled);
            HandleIntegrationTestScaffold(feature, created, notHandled);
            break;

        case "modify":
            HandleFileToUpdate(feature, skipped, notHandled);
            break;

        case "delete":
            HandleFileToDelete(feature, removed, notHandled);
            break;

        default:
            notHandled.Add(feature.Name);
            break;
    }
}



static string GetSectionContent(string content, string sectionHeader, string nextSectionHeader)
{
    int startIndex = content.IndexOf(sectionHeader, StringComparison.Ordinal);
    if (startIndex == -1)
    {
        return "";
    }

    int endIndex = content.IndexOf(nextSectionHeader, startIndex + sectionHeader.Length, StringComparison.Ordinal);
    if (endIndex == -1)
    {
        return "";
    }

    string sectionContent = content.Substring(startIndex + sectionHeader.Length, endIndex - (startIndex + sectionHeader.Length));
    return sectionContent.Trim();
}

static void HandleFeatureToCreate(FeatureDetails featureDetails, List<string> createdFiles, List<string> notHandledFiles)
{
    string endpointfilePath = GenerateFeatureFilePath(featureDetails, $"{featureDetails.Name}Endpoint");
    string commandfilePath = GenerateFeatureFilePath(featureDetails, $"{featureDetails.Name}{featureDetails.Command.Type}");

    if (!File.Exists(endpointfilePath) && !File.Exists(commandfilePath))
    {
        Directory.CreateDirectory(Path.GetDirectoryName(endpointfilePath));

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

static void HandleIntegrationTestScaffold(FeatureDetails featureDetails, List<string> createdFiles, List<string> notHandledFiles)
{
    string integrationTestFilePath = GenerateIntegrationTestFilePath(featureDetails, $"{featureDetails.Name}Tests");
    
    if (!File.Exists(integrationTestFilePath))
    {
        Directory.CreateDirectory(Path.GetDirectoryName(integrationTestFilePath));
        string integrationTestContent = FileFactory.CreateIntegrationTest(featureDetails);
        File.WriteAllText(integrationTestFilePath, integrationTestContent);
        createdFiles.Add($"{featureDetails.Name}Tests");
    }
    else
    {
        notHandledFiles.Add(featureDetails.Name);
    }
}

static void HandleFileToUpdate(FeatureDetails featureDetails, List<string> skippedFiles, List<string> notHandledFiles)
{
    string filePath = GenerateFeatureFilePath(featureDetails, $"{featureDetails.Name}{featureDetails.Command.Type}");

    string endpointFilePath = GenerateFeatureFilePath(featureDetails, $"{featureDetails.Name}Endpoint");

    if (File.Exists(endpointFilePath) || File.Exists(filePath))
    {
        skippedFiles.Add(featureDetails.Name);
    }
    else
    {
        notHandledFiles.Add(featureDetails.Name);
    }
}

static void HandleFileToDelete(FeatureDetails featureDetails, List<string> removedFiles, List<string> notHandledFiles)
{
    string endpointFilePath = GenerateFeatureFilePath(featureDetails, $"{featureDetails.Name}Endpoint");
    string cqrsFilePath = GenerateFeatureFilePath(featureDetails, $"{featureDetails.Name}{featureDetails.Command.Type}");
    string integrationTestFilePath = GenerateIntegrationTestFilePath(featureDetails, $"{featureDetails.Name}Tests");

    if (File.Exists(endpointFilePath) || File.Exists(cqrsFilePath) || File.Exists(integrationTestFilePath))
    {
        if (File.Exists(endpointFilePath))
        {
            File.Delete(endpointFilePath);
        }

        if (File.Exists(cqrsFilePath))
        {
            File.Delete(cqrsFilePath);
        }

        if (File.Exists(integrationTestFilePath))
        {
            File.Delete(integrationTestFilePath);
        }

        removedFiles.Add(featureDetails.Name);
    }
    else
    {
        notHandledFiles.Add(featureDetails.Name);
    }
}

static string GenerateFeatureFilePath(FeatureDetails featureDetails, string fileName)
{
    string repoRoot = $@"C:\Users\{Environment.UserName}\source\repos\MyHomeRamen";
    const string filePathTemplate = "{Module}.Features.{Aggregate}.{Feature}.{TypeName}";

    string[] filePathParts = filePathTemplate.Replace("{Module}", featureDetails.Module, StringComparison.Ordinal)
                                         .Replace("{Aggregate}", featureDetails.Aggregate, StringComparison.Ordinal)
                                         .Replace("{Feature}", featureDetails.Name, StringComparison.Ordinal)
                                         .Replace("{TypeName}", fileName, StringComparison.Ordinal)
                                         .Split(".");

    string filePath = Path.Combine(filePathParts) + ".cs";

    return Path.Combine(repoRoot, "MyHomeRamen.Features", filePath);
}

static string GenerateIntegrationTestFilePath(FeatureDetails featureDetails, string fileName)
{
    string repoRoot = $@"C:\Users\{Environment.UserName}\source\repos\MyHomeRamen";
    string integrationTestFolder = featureDetails.AggregatePluralName;
    string integrationTestProjectPath = $"MyHomeRamen.{featureDetails.Module}Api.IntegrationTests";

    return Path.Combine(repoRoot, integrationTestProjectPath, integrationTestFolder, $"{fileName}.cs");
}
