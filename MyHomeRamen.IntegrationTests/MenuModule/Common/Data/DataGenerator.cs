using System.Security.Cryptography;
using Bogus;
using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models;
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

    internal static IEnumerable<User> GeneratedUsers { get; private set; }   = [];


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

    internal static List<Category> GenerateValidCategories(int count)
    {
        List<Category>? categories = [];

        for (int i = 0; i < count; i++)
        {
            Category category = ValidCategoryFaker.Generate();
            categories.Add(category);
            GeneratedCategories = GeneratedCategories.Append(category);
        }

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
        List<Ingredient>? ingredients = [];

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
        return ValidProductFaker.Generate();
    }

    internal static List<Product> GenerateValidProducts(int count, List<Category> categories, List<Ingredient> ingredients)
    {
        List<Product>? products = [];

        for (int i = 0; i < count; i++)
        {
            Product product = ValidProductFaker.Generate();
            products.Add(product);
        }

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
}
