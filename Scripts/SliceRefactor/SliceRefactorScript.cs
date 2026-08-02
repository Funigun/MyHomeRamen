using System.Text;
using System.Text.RegularExpressions;


string repoRoot = @"C:\Users\stepn\source\repos\MyHomeRamen";

Dictionary<string, string[]> modules = new()
{
    ["Menu"] = new[]
    {
        //"Products/CreateProduct",
        //"Products/UpdateProduct",
        //"Products/GetProductById",
        //"Products/GetProductByIdForManage",
        //"Products/GetProductsByCategory",
        //"Products/GetProductsForManage",
        "Categories/CreateCategory",
        "Categories/GetCategoriesByType",
        "Categories/GetMenuCategories",
        "Categories/UpdateCategoriesOrder",
        //"Ingredients/CreateIngredient",
        //"Ingredients/GetIngredientById",
        //"Ingredients/GetIngredientsForDropdown",
        //"Ingredients/GetIngredientsForManage",
        //"Ingredients/UpdateIngredient",
    },
    ["Identity"] = new[]
    {
        //"Users/CreateAddress",
        //"Users/DeleteAddress",
        //"Users/GetAddresses",
        //"Users/GetDetails",
        //"Users/GetEmployees",
        //"Users/GetId",
        //"Users/Register",
        //"Users/RegisterEmployee",
        //"Users/RegisterGuest",
        //"Users/UpdateAddress",
        //"Roles/GetAvailableRoles",
    },
    ["ShoppingCart"] = new[]
    {
        //"Baskets/AddItemToBasket",
        //"Baskets/ClearBasket",
        //"Baskets/DeleteBasketItem",
        //"Baskets/GetCurrentBasketDetails",
        //"Baskets/GetCurrentBasketSummary",
        //"Baskets/GetPaymentDetails",
        //"Baskets/GetShippingDetails",
        //"Baskets/UpdatePaymentDetails",
        //"Baskets/UpdateShippingDetails",
    },
    ["Payments"] = new[]
    {
        //"PaymentMethods/GetAvailableMethods",
    },
};

foreach ((string module, string[] features) in modules)
{
    foreach (string featurePath in features)
    {
        RefactorFeature(module, featurePath);
    }
}

Console.WriteLine("Done.");

void RefactorFeature(string module, string featurePath)
{
    string featureDir = Path.Combine(repoRoot, "MyHomeRamen.Features", module, "Features", featurePath);
    if (!Directory.Exists(featureDir))
    {
        Console.WriteLine($"SKIP (missing dir): {featurePath}");
        return;
    }

    string featureName = Path.GetFileName(featureDir);
    string? commandFile = FindFile(featureDir, "Command.cs");
    string? queryFile = FindFile(featureDir, "Query.cs");
    string? handlerFile = FindFile(featureDir, "Handler.cs");
    string mappingsFile = Path.Combine(featureDir, "Mappings.cs");
    string? endpointFile = FindFile(featureDir, "Endpoint.cs");
    string? validatorFile = FindFile(featureDir, "Validator.cs");

    string? primaryFile = commandFile ?? queryFile;
    if (primaryFile is null || handlerFile is null || endpointFile is null)
    {
        Console.WriteLine($"SKIP {featurePath}: missing primary/handler/endpoint");
        return;
    }

    bool isCommand = commandFile is not null;
    string featureNamespace = ReadNamespace(primaryFile);

    // -------------------- 1. Merged Command/Query file --------------------
    StringBuilder mergedBuilder = new StringBuilder();
    mergedBuilder.AppendLine($"namespace {featureNamespace};");
    mergedBuilder.AppendLine();

    mergedBuilder.AppendLine(ExtractBody(primaryFile));
    mergedBuilder.AppendLine();
    mergedBuilder.AppendLine(ExtractBody(handlerFile));
    mergedBuilder.AppendLine();

    if (File.Exists(mappingsFile))
    {
        mergedBuilder.AppendLine(ExtractBody(mappingsFile));
        mergedBuilder.AppendLine();
    }

    string mergedContent = mergedBuilder.ToString();
    mergedContent = StripCommonContractsUsings(mergedContent);

    string mergedFileName = isCommand ? $"{featureName}Command.cs" : $"{featureName}Query.cs";
    string mergedFilePath = Path.Combine(featureDir, mergedFileName);

    // -------------------- 2. Merged Endpoint file --------------------
    StringBuilder endpointBuilder = new StringBuilder();
    endpointBuilder.AppendLine($"namespace {featureNamespace};");
    endpointBuilder.AppendLine();

    List<string> typesToInline = FindCommonContractTypes(primaryFile)
        .Concat(FindCommonContractTypes(handlerFile))
        .Concat(FindCommonContractTypes(endpointFile))
        .Concat(FindCommonContractTypes(mappingsFile))
        .Distinct()
        .ToList();

    Dictionary<string, string> typeRenames = new();

    foreach (string typeName in typesToInline)
    {
        string? contractFile = FindCommonContractFile(module, typeName);
        if (contractFile is null)
            continue;

        string body = ExtractBody(contractFile);
        string newTypeName = typeName;

        // Rename DTOs with feature prefix to avoid collisions
        if (typeName.EndsWith("Dto"))
        {
            newTypeName = featureName + typeName;
        }

        if (newTypeName != typeName)
        {
            body = ReplaceWholeWord(body, typeName, newTypeName);
            typeRenames[typeName] = newTypeName;
        }

        endpointBuilder.AppendLine(body);
        endpointBuilder.AppendLine();
    }

    endpointBuilder.AppendLine(ExtractBody(endpointFile));
    string endpointContent = endpointBuilder.ToString();

    // Strip Common.Contracts usings from endpoint file
    endpointContent = StripCommonContractsUsings(endpointContent);

    // Apply renames inside both files
    foreach ((string oldName, string newName) in typeRenames)
    {
        endpointContent = ReplaceWholeWord(endpointContent, oldName, newName);
        mergedContent = ReplaceWholeWord(mergedContent, oldName, newName);
    }

    string mergedEndpointFilePath = Path.Combine(featureDir, $"{featureName}Endpoint.cs");

    // -------------------- Write + cleanup --------------------
    File.WriteAllText(mergedFilePath, mergedContent.Trim() + Environment.NewLine, Encoding.UTF8);
    Console.WriteLine($"WRITE {mergedFilePath}");

    File.WriteAllText(mergedEndpointFilePath, endpointContent.Trim() + Environment.NewLine, Encoding.UTF8);
    Console.WriteLine($"WRITE {mergedEndpointFilePath}");

    SafeDelete(primaryFile);
    SafeDelete(handlerFile);
    SafeDelete(mappingsFile);
    SafeDelete(endpointFile);
}

// -------------------- Helpers --------------------

string? FindFile(string dir, string endsWith) =>
    Directory.EnumerateFiles(dir, "*.cs")
        .FirstOrDefault(f => Path.GetFileName(f).EndsWith(endsWith, StringComparison.OrdinalIgnoreCase));

string ReadNamespace(string path)
{
    foreach (string line in File.ReadLines(path))
    {
        string t = line.Trim();
        if (t.StartsWith("namespace "))
            return t["namespace ".Length..].TrimEnd(';');
    }
    throw new InvalidOperationException($"No namespace in {path}");
}

string ExtractBody(string path)
{
    string[] lines = File.ReadAllLines(path);
    StringBuilder sb = new();
    bool insideNamespace = false;

    foreach (string line in lines)
    {
        string trimmed = line.Trim();

        if (trimmed.StartsWith("namespace "))
        {
            insideNamespace = true;
            continue;
        }

        if (insideNamespace && trimmed == "{")
            continue;

        if (insideNamespace && trimmed == "}")
            continue;

        sb.AppendLine(line);
    }

    return sb.ToString().Trim();
}

string StripCommonContractsUsings(string content)
{
    StringBuilder sb = new();
    foreach (string line in content.Split(Environment.NewLine))
    {
        if (line.TrimStart().StartsWith("using MyHomeRamen.Common.Contracts"))
            continue;
        sb.AppendLine(line);
    }
    return sb.ToString();
}

IEnumerable<string> FindCommonContractTypes(string filePath)
{
    if (!File.Exists(filePath))
        yield break;

    string text = File.ReadAllText(filePath);
    MatchCollection matches = Regex.Matches(text, @"MyHomeRamen\.Common\.Contracts\.[A-Za-z0-9_.]+\.([A-Za-z0-9_]+)");

    foreach (Match m in matches.DistinctBy(x => x.Value))
        yield return m.Groups[1].Value;
}

string? FindCommonContractFile(string module, string typeName)
{
    string contractsDir = Path.Combine(repoRoot, "MyHomeRamen.Common.Contracts", module);
    if (!Directory.Exists(contractsDir))
        return null;

    foreach (string file in Directory.EnumerateFiles(contractsDir, "*.cs", SearchOption.AllDirectories))
    {
        if (Path.GetFileNameWithoutExtension(file) == typeName)
            return file;
    }

    return null;
}

string ReplaceWholeWord(string text, string oldValue, string newValue) =>
    Regex.Replace(text, $@"\b{Regex.Escape(oldValue)}\b", newValue);

void SafeDelete(string? file)
{
    if (file is null || !File.Exists(file))
        return;
    File.Delete(file);
    Console.WriteLine($"DELETE {file}");
}
