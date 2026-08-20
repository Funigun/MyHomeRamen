
string solutionRoot = args[0];
string moduleName = args[1];
string aggregatePath = args[2];
string aggregateName = args[3];

const string featuresLocationTemplate = @"MyHomeRamen.Features\{Module}\Features\{AggregatePath}\Common\I{Aggregate}{Type}.cs";
const string persistanceLocationTemplate = @"MyHomeRamen.Persistance\{Module}\{AggregatePath}\{Aggregate}{Type}.cs";

const string featureRepositoryTemplate =
"""
using MyHomeRamen.Domain.{Module}.{AggregatePath};
using MyHomeRamen.Features.Common.Repository;

namespace MyHomeRamen.Features.{Module}.Features.{AggregatePath}.Common;

public interface I{Aggregate}Repository : IRepository<{Aggregate}, {Aggregate}Id>
{
    I{Aggregate}Query Query();

    I{Aggregate}Loader Load();
}

""";

const string featureQueryTemplate =
"""
using MyHomeRamen.Domain.{Module}.{AggregatePath};

namespace MyHomeRamen.Features.{Module}.Features.{AggregatePath}.Common;

public interface I{Aggregate}Query
{

}

""";

const string featureLoadTemplate =
"""
using MyHomeRamen.Domain.{Module}.{AggregatePath};

namespace MyHomeRamen.Features.{Module}.Features.{AggregatePath}.Common;

public interface I{Aggregate}Loader
{

}

""";

const string persistanceRepositoryTemplate =
"""
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MyHomeRamen.Domain.{Module}.{AggregatePath};
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.{Module}.Features.{AggregatePath}.Common;

namespace MyHomeRamen.Persistance.{Module};

public partial class {Aggregate}Repository : I{Aggregate}Repository
{
    I{Aggregate}Query I{Aggregate}Repository.Query() => this;

    I{Aggregate}Loader I{Aggregate}Repository.Load() => this;
}
""";

const string persistanceQueryTemplate =
"""
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.{Module}.{AggregatePath};
using MyHomeRamen.Features.{Module}.Features.{AggregatePath}.Common;

namespace MyHomeRamen.Persistance.{Module};

public partial class {Aggregate}Repository : I{Aggregate}Query
{

}

""";

const string persIstanceLoaderTemplate =
"""
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.{Module}.{AggregatePath};
using MyHomeRamen.Features.{Module}.Features.{AggregatePath}.Common;

namespace MyHomeRamen.Persistance.{Module};

public partial class {Aggregate}Repository : I{Aggregate}Loader
{

}

""";

IEnumerable<string> fileTypes = ["Repository", "Query", "Load"];


string PrepareFilePath(string templatePath, string type)
{
    string filePath = templatePath.Replace("{Module}", moduleName)
                                  .Replace("{AggregatePath}", aggregatePath)
                                  .Replace("{Aggregate}", aggregateName)
                                  .Replace("{Type}", type);

    return Path.Combine(solutionRoot, filePath);
}

string PrepareFileContent(string templateContent, string type)
{
    string fileContent = templateContent.Replace("{Module}", moduleName)
                                        .Replace("{AggregatePath}", aggregatePath)
                                        .Replace("{Aggregate}", aggregateName)
                                        .Replace("{Aggregates}", $"{aggregateName}s");
    return fileContent;
}

foreach (var fileType in fileTypes)
{
    string calculatedFeatureFileTemplate = fileType switch
    {
        "Repository" => featureRepositoryTemplate,
        "Query" => featureQueryTemplate,
        "Load" => featureLoadTemplate,
        _ => throw new InvalidOperationException($"Unknown file type: {fileType}")
    };

    string calculatedPersistanceFileTemplate = fileType switch
    {
        "Repository" => persistanceRepositoryTemplate,
        "Query" => persistanceQueryTemplate,
        "Load" => persIstanceLoaderTemplate,
        _ => throw new InvalidOperationException($"Unknown file type: {fileType}")
    };

    string featureFilePath = Path.Combine(solutionRoot, PrepareFilePath(featuresLocationTemplate, fileType));
    string persistanceFilePath = Path.Combine(solutionRoot, PrepareFilePath(persistanceLocationTemplate, fileType));

    string featureFileContent = PrepareFileContent(calculatedFeatureFileTemplate, fileType);
    string persistanceFileContent = PrepareFileContent(calculatedPersistanceFileTemplate, fileType);

    if (!Path.Exists(featureFilePath))
    {
        Directory.CreateDirectory(Path.GetDirectoryName(featureFilePath)!);
        File.WriteAllText(featureFilePath, featureFileContent);
        Console.WriteLine($"Created feature file: {featureFilePath}");
    }

    if (!Path.Exists(persistanceFilePath))
    {
        Directory.CreateDirectory(Path.GetDirectoryName(persistanceFilePath)!);
        File.WriteAllText(persistanceFilePath, persistanceFileContent);
        Console.WriteLine($"Created persistance file: {persistanceFilePath}");
    }
}
