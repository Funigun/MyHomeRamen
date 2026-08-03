using System.Text;
using System.Text.RegularExpressions;

string repoRoot = FindRepoRoot(Directory.GetCurrentDirectory());

IEnumerable<FileConfig> configurations =
[
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\CreateAddress\\CreateAddressCommand.cs", ["MyHomeRamen.Features\\Identity\\Features\\Users\\CreateAddress\\CreateAddressHandler.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\CreateAddress\\CreateAddressEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Users\\Account\\Requests\\CreateAddressRequest.cs", "MyHomeRamen.Common.Contracts\\Users\\Account\\Responses\\CreateAddressResponse.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\DeleteAddress\\DeleteAddressCommand.cs", ["MyHomeRamen.Features\\Identity\\Features\\Users\\DeleteAddress\\DeleteAddressHandler.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\DeleteAddress\\DeleteAddressEndpoint.cs", []),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\GetAddresses\\GetAddressesQuery.cs", ["MyHomeRamen.Features\\Identity\\Features\\Users\\GetAddresses\\GetAddressesHandler.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\GetAddresses\\GetAddressesEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Users\\Account\\Responses\\GetAddressesResponse.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\GetDetails\\GetDetailsQuery.cs", ["MyHomeRamen.Features\\Identity\\Features\\Users\\GetDetails\\GetDetailsHandler.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\GetDetails\\GetDetailsEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Users\\Account\\Responses\\GetDetailsResponse.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\GetEmployees\\GetEmployeesQuery.cs", ["MyHomeRamen.Features\\Identity\\Features\\Users\\GetEmployees\\GetEmployeesHandler.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\GetEmployees\\GetEmployeesEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Users\\Employees\\Responses\\GetEmployeesResponse.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\GetId\\GetMyIdQuery.cs", ["MyHomeRamen.Features\\Identity\\Features\\Users\\GetId\\GetMyIdHandler.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\GetId\\GetMyIdEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Users\\Account\\Responses\\GetMyIdResponse.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\Register\\RegisterCommand.cs", ["MyHomeRamen.Features\\Identity\\Features\\Users\\Register\\RegisterHandler.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\Register\\RegisterEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Users\\Account\\Requests\\RegisterRequest.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\RegisterEmployee\\RegisterEmployeeCommand.cs", ["MyHomeRamen.Features\\Identity\\Features\\Users\\RegisterEmployee\\RegisterEmployeeHandler.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\RegisterEmployee\\RegisterEmployeeEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Users\\Employees\\Requests\\RegisterEmployeeRequest.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\RegisterGuest\\RegisterGuestCommand.cs", ["MyHomeRamen.Features\\Identity\\Features\\Users\\RegisterGuest\\RegisterGuestHandler.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\RegisterGuest\\RegisterGuestEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Users\\Account\\Requests\\RegisterGuestRequest.cs", "MyHomeRamen.Common.Contracts\\Users\\Account\\Responses\\RegisterGuestResponse.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\UpdateAddress\\UpdateAddressCommand.cs", ["MyHomeRamen.Features\\Identity\\Features\\Users\\UpdateAddress\\UpdateAddressHandler.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Users\\UpdateAddress\\UpdateAddressEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Users\\Account\\Requests\\UpdateAddressRequest.cs", "MyHomeRamen.Common.Contracts\\Users\\Account\\Responses\\UpdateAddressResponse.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Roles\\GetAvailableRoles\\GetAvailableRolesQuery.cs", ["MyHomeRamen.Features\\Identity\\Features\\Roles\\GetAvailableRoles\\GetAvailableRolesHandler.cs"]),
    new("MyHomeRamen.Features\\Identity\\Features\\Roles\\GetAvailableRoles\\GetAvailableRolesEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Users\\Roles\\Responses\\GetAvailableRolesResponse.cs"]),

    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\AddItemToBasket\\AddItemToBasketCommand.cs", ["MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\AddItemToBasket\\AddItemToBasketHandler.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\AddItemToBasket\\AddItemToBasketEndpoint.cs", ["MyHomeRamen.Common.Contracts\\ShoppingCart\\Baskets\\Requests\\AddItemToBasketRequest.cs", "MyHomeRamen.Common.Contracts\\ShoppingCart\\Baskets\\Responses\\AddItemToBasketResponse.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\ClearBasket\\ClearBasketCommand.cs", ["MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\ClearBasket\\ClearBasketHandler.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\ClearBasket\\ClearBasketEndpoint.cs", []),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\DeleteBasketItem\\DeleteBasketItemCommand.cs", ["MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\DeleteBasketItem\\DeleteBasketItemHandler.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\DeleteBasketItem\\DeleteBasketItemEndpoint.cs", []),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\GetCurrentBasketDetails\\GetCurrentBasketDetailsQuery.cs", ["MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\GetCurrentBasketDetails\\GetCurrentBasketDetailsHandler.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\GetCurrentBasketDetails\\GetCurrentBasketDetailsEndpoint.cs", ["MyHomeRamen.Common.Contracts\\ShoppingCart\\Baskets\\Responses\\GetCurrentBasketDetailsResponse.cs", "MyHomeRamen.Common.Contracts\\ShoppingCart\\Baskets\\DTOs\\BasketDetailsItemDto.cs", "MyHomeRamen.Common.Contracts\\ShoppingCart\\Baskets\\DTOs\\BasketDetailsItemProductDto.cs", "MyHomeRamen.Common.Contracts\\ShoppingCart\\Baskets\\DTOs\\BasketDetailsIngredientDto.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\GetCurrentBasketSummary\\GetCurrentBasketSummaryQuery.cs", ["MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\GetCurrentBasketSummary\\GetCurrentBasketSummaryHandler.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\GetCurrentBasketSummary\\GetCurrentBasketSummaryEndpoint.cs", ["MyHomeRamen.Common.Contracts\\ShoppingCart\\Baskets\\Responses\\GetCurrentBasketSummaryResponse.cs", "MyHomeRamen.Common.Contracts\\ShoppingCart\\Baskets\\DTOs\\BasketSummaryItemDto.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\GetPaymentDetails\\GetPaymentDetailsQuery.cs", ["MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\GetPaymentDetails\\GetPaymentDetailsHandler.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\GetPaymentDetails\\GetPaymentDetailsEndpoint.cs", ["MyHomeRamen.Common.Contracts\\ShoppingCart\\Baskets\\Responses\\PaymentDetailsResponse.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\GetShippingDetails\\GetShippingDetailsQuery.cs", ["MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\GetShippingDetails\\GetShippingDetailsHandler.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\GetShippingDetails\\GetShippingDetailsEndpoint.cs", ["MyHomeRamen.Common.Contracts\\ShoppingCart\\Baskets\\Responses\\ShippingDetailsResponse.cs", "MyHomeRamen.Common.Contracts\\ShoppingCart\\Baskets\\DTOs\\ShippingAddressDto.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\UpdatePaymentDetails\\UpdatePaymentDetailsCommand.cs", ["MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\UpdatePaymentDetails\\UpdatePaymentDetailsHandler.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\UpdatePaymentDetails\\UpdatePaymentDetailsEndpoint.cs", ["MyHomeRamen.Common.Contracts\\ShoppingCart\\Baskets\\Requests\\UpdatePaymentDetailsRequest.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\UpdateShippingDetails\\UpdateShippingDetailsCommand.cs", ["MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\UpdateShippingDetails\\UpdateShippingDetailsHandler.cs"]),
    new("MyHomeRamen.Features\\ShoppingCart\\Features\\Baskets\\UpdateShippingDetails\\UpdateShippingDetailsEndpoint.cs", ["MyHomeRamen.Common.Contracts\\ShoppingCart\\Baskets\\Requests\\UpdateShippingDetailsRequest.cs"]),

    new("MyHomeRamen.Features\\Payments\\Features\\PaymentMethods\\GetAvailableMethods\\GetAvailableMethodsQuery.cs", ["MyHomeRamen.Features\\Payments\\Features\\PaymentMethods\\GetAvailableMethods\\GetAvailableMethodsHandler.cs"]),
    new("MyHomeRamen.Features\\Payments\\Features\\PaymentMethods\\GetAvailableMethods\\GetAvailableMethodsEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Payments\\PaymentMethods\\Responses\\GetAvailableMethodsResponse.cs", "MyHomeRamen.Common.Contracts\\Payments\\PaymentMethods\\DTOs\\AvailableChannelDto.cs"]),

    new("MyHomeRamen.Features\\Menu\\Features\\Ingredients\\CreateIngredient\\CreateIngredientCommand.cs", ["MyHomeRamen.Features\\Menu\\Features\\Ingredients\\CreateIngredient\\CreateIngredientHandler.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Ingredients\\CreateIngredient\\CreateIngredientEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Menu\\Ingredients\\Requests\\CreateIngredientRequest.cs", "MyHomeRamen.Common.Contracts\\Menu\\Ingredients\\Responses\\CreateIngredientResponse.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Ingredients\\DeleteIngredient\\DeleteIngredientCommand.cs", ["MyHomeRamen.Features\\Menu\\Features\\Ingredients\\DeleteIngredient\\DeleteIngredientHandler.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Ingredients\\DeleteIngredient\\DeleteIngredientEndpoint.cs", []),
    new("MyHomeRamen.Features\\Menu\\Features\\Ingredients\\GetIngredientById\\GetIngredientByIdQuery.cs", ["MyHomeRamen.Features\\Menu\\Features\\Ingredients\\GetIngredientById\\GetIngredientByIdHandler.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Ingredients\\GetIngredientById\\GetIngredientByIdEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Menu\\Ingredients\\Responses\\GetIngredientByIdResponse.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Ingredients\\GetIngredientsForDropdown\\GetIngredientsForDropdownQuery.cs", ["MyHomeRamen.Features\\Menu\\Features\\Ingredients\\GetIngredientsForDropdown\\GetIngredientsForDropdownHandler.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Ingredients\\GetIngredientsForDropdown\\GetIngredientsForDropdownEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Menu\\Ingredients\\Responses\\GetIngredientsForDropdownResponse.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Ingredients\\GetIngredientsForManage\\GetIngredientsForManageQuery.cs", ["MyHomeRamen.Features\\Menu\\Features\\Ingredients\\GetIngredientsForManage\\GetIngredientsForManageHandler.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Ingredients\\GetIngredientsForManage\\GetIngredientsForManageEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Menu\\Ingredients\\Requests\\GetIngredientsForManageRequest.cs", "MyHomeRamen.Common.Contracts\\Menu\\Ingredients\\Responses\\GetIngredientsForManageResponse.cs", "MyHomeRamen.Common.Contracts\\Menu\\Ingredients\\DTOs\\IngredientForManageDto.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Ingredients\\UpdateIngredient\\UpdateIngredientCommand.cs", ["MyHomeRamen.Features\\Menu\\Features\\Ingredients\\UpdateIngredient\\UpdateIngredientHandler.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Ingredients\\UpdateIngredient\\UpdateIngredientEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Menu\\Ingredients\\Requests\\UpdateIngredientRequest.cs", "MyHomeRamen.Common.Contracts\\Menu\\Ingredients\\Responses\\UpdateIngredientResponse.cs"]),

    new("MyHomeRamen.Features\\Menu\\Features\\Products\\CreateProduct\\CreateProductCommand.cs", ["MyHomeRamen.Features\\Menu\\Features\\Products\\CreateProduct\\CreateProductHandler.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Products\\CreateProduct\\CreateProductEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Menu\\Products\\Requests\\CreateProductRequest.cs", "MyHomeRamen.Common.Contracts\\Menu\\Products\\Responses\\CreateProductResponse.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Products\\GetProductById\\GetProductByIdQuery.cs", ["MyHomeRamen.Features\\Menu\\Features\\Products\\GetProductById\\GetProductByIdHandler.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Products\\GetProductById\\GetProductByIdEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Menu\\Products\\Responses\\GetProductByIdResponse.cs", "MyHomeRamen.Common.Contracts\\Menu\\Products\\DTOs\\IngredientDto.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Products\\GetProductByIdForManage\\GetProductByIdForManageQuery.cs", ["MyHomeRamen.Features\\Menu\\Features\\Products\\GetProductByIdForManage\\GetProductByIdForManageHandler.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Products\\GetProductByIdForManage\\GetProductByIdForManageEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Menu\\Products\\Responses\\GetProductByIdForManageResponse.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Products\\GetProductsByCategory\\GetProductsByCategoryQuery.cs", ["MyHomeRamen.Features\\Menu\\Features\\Products\\GetProductsByCategory\\GetProductsByCategoryHandler.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Products\\GetProductsByCategory\\GetProductsByCategoryEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Menu\\Products\\Requests\\GetProductsByCategoryRequest.cs", "MyHomeRamen.Common.Contracts\\Menu\\Products\\Responses\\GetProductsByCategoryResponse.cs", "MyHomeRamen.Common.Contracts\\Menu\\Products\\DTOs\\ProductIngredientDto.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Products\\GetProductsForManage\\GetProductsForManageQuery.cs", ["MyHomeRamen.Features\\Menu\\Features\\Products\\GetProductsForManage\\GetProductsForManageHandler.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Products\\GetProductsForManage\\GetProductsForManageEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Menu\\Products\\Requests\\GetProductsForManageRequest.cs", "MyHomeRamen.Common.Contracts\\Menu\\Products\\Responses\\GetProductsForManageResponse.cs", "MyHomeRamen.Common.Contracts\\Menu\\Products\\DTOs\\ProductForManageDto.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Products\\UpdateProduct\\UpdateProductCommand.cs", ["MyHomeRamen.Features\\Menu\\Features\\Products\\UpdateProduct\\UpdateProductHandler.cs"]),
    new("MyHomeRamen.Features\\Menu\\Features\\Products\\UpdateProduct\\UpdateProductEndpoint.cs", ["MyHomeRamen.Common.Contracts\\Menu\\Products\\Requests\\UpdateProductRequest.cs", "MyHomeRamen.Common.Contracts\\Menu\\Products\\Responses\\UpdateProductResponse.cs"])
];

StringBuilder outputBuilder = new();

foreach (FileConfig config in configurations)
{
    outputBuilder.Clear();
    List<string> usings = [];
    List<string> contentParts = [];

    string targetFilePath = Path.Combine(repoRoot, config.TargetFile.Replace('/', '\\'));
    string targetDirectory = Path.GetDirectoryName(targetFilePath)!;
    string namespaceName = ToNamespaceName(config.TargetFile);

    Directory.CreateDirectory(targetDirectory);

    if(!config.TargetFile.EndsWith("Endpoint.cs", StringComparison.Ordinal) && File.Exists(targetFilePath))
    {
        string existingContent = File.ReadAllText(targetFilePath);
        string existingNamespace = ExtractNamespaceName(existingContent);
        if (!string.IsNullOrWhiteSpace(existingNamespace))
        {
            namespaceName = existingNamespace;
        }

        AddContent(existingContent, usings, contentParts);
    }

    foreach (string filePath in config.FilesToMerge)
    {
        string fullFilePath = Path.Combine(repoRoot, filePath.Replace('/', '\\'));
        if (!File.Exists(fullFilePath))
        {
            Console.WriteLine($"[slice-refactor] missing source file: {filePath}");
            continue;
        }

        AddContent(File.ReadAllText(fullFilePath), usings, contentParts);
    }

    RemoveMergedHandlers(config.FilesToMerge, repoRoot);

    if (config.TargetFile.EndsWith("Endpoint.cs", StringComparison.Ordinal) && File.Exists(targetFilePath))
    {
        string existingContent = File.ReadAllText(targetFilePath);
        string existingNamespace = ExtractNamespaceName(existingContent);
        if (!string.IsNullOrWhiteSpace(existingNamespace))
        {
            namespaceName = existingNamespace;
        }

        AddContent(existingContent, usings, contentParts);
    }

    foreach (string usingLine in usings)
    {
        outputBuilder.AppendLine(usingLine);
    }

    outputBuilder.AppendLine();
    outputBuilder.AppendLine($"namespace {namespaceName};");
    outputBuilder.AppendLine();

    for (int i = 0; i < contentParts.Count; i++)
    {
        string part = contentParts[i];
        if (string.IsNullOrWhiteSpace(part))
        {
            continue;
        }

        if (i > 0)
        {
            outputBuilder.AppendLine();
            outputBuilder.AppendLine();
        }

        outputBuilder.Append(part);
    }

    outputBuilder.AppendLine();
    File.WriteAllText(targetFilePath, outputBuilder.ToString(), new UTF8Encoding(false));
    Console.WriteLine($"[slice-refactor] wrote {config.TargetFile}");
}

Console.WriteLine("Done.");

static string ExtractNamespaceName(string content)
{
    Match match = Regex.Match(content, @"^\s*namespace\s+([A-Za-z0-9_.]+)\s*;", RegexOptions.Multiline);
    return match.Success ? match.Groups[1].Value : string.Empty;
}

static string ToNamespaceName(string filePath)
{
    string normalized = filePath.Replace('\\', '/').Replace(".cs", string.Empty);
    string directory = Path.GetDirectoryName(normalized)!;
    return directory.Replace('/', '.').Trim('.');
}

static void AddContent(string fileContent, List<string> usings, List<string> contentParts)
{
    string normalizedContent = fileContent.Replace("\r\n", "\n");
    string[] fileLines = normalizedContent.Split('\n');

    bool namespaceFound = false;
    string bodyContent = string.Empty;

    foreach (string line in fileLines)
    {
        string trimmed = line.Trim();

        if (!namespaceFound)
        {
            if (string.IsNullOrWhiteSpace(trimmed))
            {
                continue;
            }

            if (trimmed.StartsWith("using ", StringComparison.Ordinal))
            {
                if (!usings.Contains(trimmed, StringComparer.Ordinal))
                {
                    usings.Add(trimmed);
                }

                continue;
            }

            if (trimmed.StartsWith("namespace ", StringComparison.Ordinal))
            {
                namespaceFound = true;
                continue;
            }

            bodyContent = line;
            namespaceFound = true;
            continue;
        }

        if (bodyContent.Length == 0)
        {
            bodyContent = line;
        }
        else
        {
            bodyContent += Environment.NewLine + line;
        }
    }

    if (!string.IsNullOrWhiteSpace(bodyContent))
    {
        contentParts.Add(bodyContent);
    }
}

static void RemoveMergedHandlers(IEnumerable<string> filePaths, string repoRoot)
{
    foreach (string filePath in filePaths)
    {
        if (!filePath.EndsWith("Handler.cs", StringComparison.Ordinal))
        {
            continue;
        }

        string fullFilePath = Path.Combine(repoRoot, filePath.Replace('/', '\\'));
        if (File.Exists(fullFilePath))
        {
            File.Delete(fullFilePath);
            Console.WriteLine($"[slice-refactor] removed handler {filePath}");
        }
    }
}

static string FindRepoRoot(string startDirectory)
{
    DirectoryInfo? current = new(startDirectory);
    while (current is not null)
    {
        if (Directory.Exists(Path.Combine(current.FullName, ".git")) ||
            File.Exists(Path.Combine(current.FullName, "MyHomeRamen.slnx")) ||
            File.Exists(Path.Combine(current.FullName, "MyHomeRamen.sln")))
        {
            return current.FullName;
        }

        current = current.Parent;
    }

    return startDirectory;
}

record FileConfig(string TargetFile, string[] FilesToMerge);
