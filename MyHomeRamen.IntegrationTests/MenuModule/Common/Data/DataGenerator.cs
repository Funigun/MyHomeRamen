using System.Security.Cryptography;
using Bogus;
using MyHomeRamen.Api.Menu.Features.Categories.CreateCategory.Models;
using MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient.Models;
using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models;
using MyHomeRamen.Common.Contracts.Menu.Categories;
using MyHomeRamen.Common.Contracts.Menu.Ingredients;
using MyHomeRamen.Common.Contracts.Menu.Products;
using MyHomeRamen.Domain.Common.Category;
using MyHomeRamen.Domain.Common.Ingredient;
using MyHomeRamen.Domain.Common.Product;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

internal static class DataGenerator
{
    internal static IEnumerable<Category> GeneratedCategories { get; private set; } = [];

    internal static IEnumerable<Ingredient> GeneratedIngredients { get; private set; } = [];

    internal static IEnumerable<Product> GeneratedProducts { get; private set; } = [];

    internal static IEnumerable<User> GeneratedUsers { get; private set; } = [];

    private static readonly Faker<Category> ValidCategoryFaker = new Faker<Category>()
        .CustomInstantiator(f => Category.Create(
            Guid.NewGuid(),
            f.Random.String2(CategoryConstants.MinNameLength, CategoryConstants.MaxNameLength),
            f.Random.Int(CategoryConstants.MinSortOrder, 1000),
            f.PickRandom<CategoryType>())
        );

    private static readonly Faker<Ingredient> ValidIngredientFaker = new Faker<Ingredient>()
        .CustomInstantiator(f => Ingredient.Create(
            Guid.NewGuid(),
            f.Random.String2(IngredientConstants.MinNameLength, IngredientConstants.MaxNameLength),
            f.Random.String2(IngredientConstants.MinDescriptionLength, IngredientConstants.MaxDescriptionLength),
            f.Random.Number(),
            [GetRandomIngredientCategory()])
        );

    private static readonly Faker<Product> ValidProductFaker = new Faker<Product>()
        .CustomInstantiator(f => Product.Create(
            Guid.NewGuid(),
            f.Random.String2(ProductConstants.MinNameLength, ProductConstants.MaxNameLength),
            f.Random.String2(ProductConstants.MinDescriptionLength, ProductConstants.MaxDescriptionLength),
            f.Random.Decimal(ProductConstants.MinPrice, ProductConstants.MaxPrice),
            string.Empty,
            [GetRandomIngredient()],
            [GetRandomIngredient()],
            [GetRandomProductCategory()])
        );

    internal static Category GenerateValidCategory()
    {
        Category category = ValidCategoryFaker.Generate();
        GeneratedCategories = GeneratedCategories.Append(category);
        return category;
    }

    internal static Category GenerateValidCategory(CategoryType categoryType)
    {
        Faker f = new();
        Category category = Category.Create(
            Guid.NewGuid(),
            f.Random.String2(CategoryConstants.MinNameLength, CategoryConstants.MaxNameLength),
            f.Random.Int(CategoryConstants.MinSortOrder, 1000),
            categoryType);
        GeneratedCategories = GeneratedCategories.Append(category);
        return category;
    }

    internal static List<Category> GenerateValidCategories(int count)
    {
        List<Category> categories = [];

        for (int i = 0; i < count; i++)
        {
            Category category = ValidCategoryFaker.Generate();
            categories.Add(category);
            GeneratedCategories = GeneratedCategories.Append(category);
        }

        return categories;
    }

    internal static List<Category> GenerateValidCategories(int count, CategoryType categoryType)
    {
        List<Category> categories = [];
        Faker f = new();

        for (int i = 0; i < count; i++)
        {
            Category category = Category.Create(
                Guid.NewGuid(),
                f.Random.String2(CategoryConstants.MinNameLength, CategoryConstants.MaxNameLength),
                f.Random.Int(CategoryConstants.MinSortOrder, 1000),
                categoryType);
            categories.Add(category);
        }

        GeneratedCategories = GeneratedCategories.Concat(categories);

        return categories;
    }

    internal static Ingredient GenerateValidIngredient()
    {
        Ingredient ingredient = ValidIngredientFaker.Generate();
        GeneratedIngredients = GeneratedIngredients.Append(ingredient);
        return ingredient;
    }

    internal static List<Ingredient> GenerateValidIngredients(int count)
    {
        List<Ingredient> ingredients = [];

        for (int i = 0; i < count; i++)
        {
            Ingredient ingredient = ValidIngredientFaker.Generate();
            ingredients.Add(ingredient);
            GeneratedIngredients = GeneratedIngredients.Append(ingredient);
        }

        return ingredients;
    }

    internal static Product GenerateValidProduct()
    {
        Product product = ValidProductFaker.Generate();
        GeneratedProducts = GeneratedProducts.Append(product);
        return product;
    }

    internal static List<Product> GenerateValidProducts(int count)
    {
        List<Product> products = [];

        for (int i = 0; i < count; i++)
        {
            Product product = ValidProductFaker.Generate();
            products.Add(product);
        }

        GeneratedProducts = GeneratedProducts.Concat(products);

        return products;
    }

    internal static Ingredient GetRandomIngredient()
    {
        List<Ingredient> ingredients = GeneratedIngredients.ToList();
        return ingredients[RandomNumberGenerator.GetInt32(ingredients.Count)];
    }

    internal static Category GetRandomProductCategory()
    {
        List<Category> productCategories = GeneratedCategories.Where(c => c.CategoryType == CategoryType.Product).ToList();
        return productCategories[RandomNumberGenerator.GetInt32(productCategories.Count)];
    }

    internal static Category GetRandomIngredientCategory()
    {
        List<Category> ingredientCategories = GeneratedCategories.Where(c => c.CategoryType == CategoryType.Ingredient).ToList();
        return ingredientCategories[RandomNumberGenerator.GetInt32(ingredientCategories.Count)];
    }

    internal static IEnumerable<User> GenerateValidUsers(IEnumerable<Role> roles)
    {
        if (GeneratedUsers.Any())
        {
            return GeneratedUsers;
        }

        List<Role> rolesList = roles.ToList();

        Role adminRole = rolesList.First(r => r.Name == RoleConstants.Admin);
        User admin = User.Create(new UserId(Guid.NewGuid()), [adminRole], adminRole.Permissions.ToList());

        IEnumerable<string> employeeRoles = [RoleConstants.Employee, RoleConstants.Waiter, RoleConstants.Chef];
        List<Role> employeeRole = rolesList.Where(r => employeeRoles.Contains(r.Name)).ToList();
        User employee = User.Create(new UserId(Guid.NewGuid()), employeeRole, employeeRole.SelectMany(role => role.Permissions).ToList());

        Role customerRole = rolesList.First(r => r.Name == RoleConstants.Customer);
        User customer = User.Create(new UserId(Guid.NewGuid()), [customerRole], customerRole.Permissions.ToList());

        GeneratedUsers = GeneratedUsers.Append(admin);
        GeneratedUsers = GeneratedUsers.Append(employee);
        GeneratedUsers = GeneratedUsers.Append(customer);

        return GeneratedUsers;
    }

    public static TheoryData<UpdateCategoriesOrderRequest> InvalidUpdateCategoriesOrderRequests()
    {
        Guid validId = Guid.NewGuid();

        return
        [
            // Empty list
            new UpdateCategoriesOrderRequest([]),

            // Sort order below minimum
            new UpdateCategoriesOrderRequest([new CategoryOrderItemDto(validId, CategorySortOrderValidator.MinSortOrder - 1)]),

            // Duplicate IDs
            new UpdateCategoriesOrderRequest([
                new CategoryOrderItemDto(validId, CategorySortOrderValidator.MinSortOrder),
                new CategoryOrderItemDto(validId, CategorySortOrderValidator.MinSortOrder + 1),
            ]),
        ];
    }

    public static TheoryData<CreateCategoryRequest> InvalidCreateCategoryRequests()
    {
        Faker faker = new();
        const int validCategoryType = (int)CategoryType.Product;

        return
        [
            // Name: empty
            new CreateCategoryRequest(string.Empty, validCategoryType),

            // Name: too short
            new CreateCategoryRequest(faker.Random.String2(1, CategoryNameValidator.MinLength - 1), validCategoryType),

            // Name: too long
            new CreateCategoryRequest(faker.Random.String2(CategoryNameValidator.MaxLength + 1, CategoryNameValidator.MaxLength + 10), validCategoryType),

            // CategoryType: invalid
            new CreateCategoryRequest(faker.Random.String2(CategoryNameValidator.MinLength, CategoryNameValidator.MaxLength), 999),
        ];
    }

    public static TheoryData<CreateProductRequest> InvalidCreateProductRequests()
    {
        Faker faker = new();
        Guid validCategoryId = GetRandomProductCategory().Id;
        Guid[] validIngredientIds = [GetRandomIngredient().Id];
        string validName = faker.Random.String2(ProductNameValidator.MinLength, ProductNameValidator.MaxLength);
        string validDescription = faker.Random.String2(ProductDescriptionValidator.MinLength, ProductDescriptionValidator.MaxLength);

        return
        [
            // Name: empty
            new CreateProductRequest(string.Empty, validDescription, ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds),

            // Name: too short
            new CreateProductRequest(faker.Random.String2(1, ProductNameValidator.MinLength - 1), validDescription, ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds),

            // Name: too long
            new CreateProductRequest(faker.Random.String2(ProductNameValidator.MaxLength + 1, ProductNameValidator.MaxLength + 10), validDescription, ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds),

            // Description: empty
            new CreateProductRequest(validName, string.Empty, ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds),

            // Description: too short
            new CreateProductRequest(validName, faker.Random.String2(1, ProductDescriptionValidator.MinLength - 1), ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds),

            // Description: too long
            new CreateProductRequest(validName, faker.Random.String2(ProductDescriptionValidator.MaxLength + 1, ProductDescriptionValidator.MaxLength + 10), ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds),

            // Price: below minimum
            new CreateProductRequest(validName, validDescription, ProductPriceValidator.MinPrice - 0.01m, validCategoryId, validIngredientIds),

            // Price: above maximum
            new CreateProductRequest(validName, validDescription, ProductPriceValidator.MaxPrice + 0.01m, validCategoryId, validIngredientIds),

            // CategoryId: empty
            new CreateProductRequest(validName, validDescription, ProductPriceValidator.MinPrice, Guid.Empty, validIngredientIds),

            // IngredientIds: empty
            new CreateProductRequest(validName, validDescription, ProductPriceValidator.MinPrice, validCategoryId, []),
        ];
    }

    public static TheoryData<UpdateIngredientRequest> InvalidUpdateIngredientRequests()
    {
        Faker faker = new();
        string validName = faker.Random.String2(IngredientNameValidator.MinLength, IngredientNameValidator.MaxLength);
        string validDescription = faker.Random.String2(IngredientDescriptionValidator.MinLength, IngredientDescriptionValidator.MaxLength);
        decimal validPrice = faker.Finance.Amount(IngredientPriceValidator.MinPrice, IngredientPriceValidator.MaxPrice);
        IEnumerable<Guid> validCategoryIds = [Guid.NewGuid()];

        return
        [
            // Name: empty
            new UpdateIngredientRequest(string.Empty, validDescription, validPrice, validCategoryIds),

            // Name: too short
            new UpdateIngredientRequest(faker.Random.String2(1, IngredientNameValidator.MinLength - 1), validDescription, validPrice, validCategoryIds),

            // Name: too long
            new UpdateIngredientRequest(faker.Random.String2(IngredientNameValidator.MaxLength + 1, IngredientNameValidator.MaxLength + 10), validDescription, validPrice, validCategoryIds),

            // Description: too long
            new UpdateIngredientRequest(validName, faker.Random.String2(IngredientDescriptionValidator.MaxLength + 1, IngredientDescriptionValidator.MaxLength + 10), validPrice, validCategoryIds),

            // Price: below minimum
            new UpdateIngredientRequest(validName, validDescription, IngredientPriceValidator.MinPrice - 0.01m, validCategoryIds),

            // Price: above maximum
            new UpdateIngredientRequest(validName, validDescription, IngredientPriceValidator.MaxPrice + 0.01m, validCategoryIds),

            // CategoryIds: empty
            new UpdateIngredientRequest(validName, validDescription, validPrice, []),
        ];
    }
}
