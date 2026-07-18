using System.Collections.ObjectModel;
using Bogus;
using Microsoft.Data.SqlClient;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Validators;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Validators;
using MyHomeRamen.Domain.Common.Category;
using MyHomeRamen.Domain.Common.Ingredient;
using MyHomeRamen.Domain.Common.Product;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

internal static class DataGenerator
{
    internal static Category CreateProductCategory(string? name = null, int sortOrder = 1)
        => new Faker<Category>()
           .CustomInstantiator(f =>
           {
               return Category.Create
                              (
                                  Guid.NewGuid(),
                                  name ?? f.RamenMenu().ProductCategory(),
                                  sortOrder,
                                  CategoryType.Product
                              );
           }).Generate();

    internal static IEnumerable<Category> CreateProductCategories(int count)
    {
        List<Category> categories = [];

        for (int  i = 0; i < count; i++)
        {
            categories.Add(CreateProductCategory($"Product Category {i + 1}", i + 1));
        }

        return categories;
    } 
        
    internal static Category CreateIngredientCategory(string? name = null)
    => new Faker<Category>()
       .CustomInstantiator(f =>
       {
           return Category.Create
                          (
                              Guid.NewGuid(),
                              name ?? f.RamenMenu().IngredientCategory(),
                              1,
                              CategoryType.Ingredient
                          );
       }).Generate();

    internal static Ingredient CreateIngredient(Category category)
        => new Faker<Ingredient>()
           .CustomInstantiator(f =>
           {
               string productName = f.RamenMenu().ProductName();

               return Ingredient.Create
                      (
                          Guid.NewGuid(),
                          productName,
                          f.RamenMenu().ProductDescription(productName),
                          f.Random.Number(),
                          [category]
                      );
           }).Generate();

    internal static Product CreateProduct(Collection<Ingredient> baseIngredients, Collection<Ingredient> customIngredients, Category category)
    => new Faker<Product>()
       .CustomInstantiator(f =>
       {
           string productName = f.RamenMenu().ProductName();

           return Product.Create
                  (
                      Guid.CreateVersion7(),
                      productName,
                      f.RamenMenu().ProductDescription(productName),
                      f.Random.Decimal(ProductConstants.MinPrice, ProductConstants.MaxPrice),
                      "",
                      baseIngredients,
                      customIngredients,
                      [category]
                  );
       }).Generate();

    internal static TheoryData<CreateProductRequest> InvalidCreateProductRequests()
    {
        Faker faker = new();
        Guid validCategoryId = CreateProductCategory().Id;
        Guid[] validIngredientIds = [CreateIngredient(CreateIngredientCategory()).Id];
        string validName = faker.Random.String2(ProductNameValidator.MinLength, ProductNameValidator.MaxLength);
        string validDescription = faker.Random.String2(ProductDescriptionValidator.MinLength, ProductDescriptionValidator.MaxLength);

        return
        [
            new CreateProductRequest(string.Empty, validDescription, ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(faker.Random.String2(1, ProductNameValidator.MinLength - 1), validDescription, ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(faker.Random.String2(ProductNameValidator.MaxLength + 1, ProductNameValidator.MaxLength + 10), validDescription, ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(validName, string.Empty, ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(validName, faker.Random.String2(1, ProductDescriptionValidator.MinLength - 1), ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(validName, faker.Random.String2(ProductDescriptionValidator.MaxLength + 1, ProductDescriptionValidator.MaxLength + 10), ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(validName, validDescription, ProductPriceValidator.MinPrice - 0.01m, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(validName, validDescription, ProductPriceValidator.MaxPrice + 0.01m, validCategoryId, validIngredientIds, []),
            new CreateProductRequest(validName, validDescription, ProductPriceValidator.MinPrice, Guid.Empty, validIngredientIds, []),
            new CreateProductRequest(validName, validDescription, ProductPriceValidator.MinPrice, validCategoryId, [], []),
            new CreateProductRequest(validName, validDescription, ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds, [Guid.Empty]),
            new CreateProductRequest(validName, validDescription, ProductPriceValidator.MinPrice, validCategoryId, validIngredientIds, validIngredientIds),
        ];
    }

    internal static TheoryData<UpdateProductRequest> InvalidUpdateProductRequests()
    {
        Faker faker = new();
        Guid validCategoryId = CreateProductCategory().Id;
        Guid[] validIngredientIds = [CreateIngredient(CreateIngredientCategory()).Id];
        string validName = faker.Random.String2(ProductNameValidator.MinLength, ProductNameValidator.MaxLength);
        string validDescription = faker.Random.String2(ProductDescriptionValidator.MinLength, ProductDescriptionValidator.MaxLength);
        decimal validPrice = faker.Finance.Amount(ProductPriceValidator.MinPrice, ProductPriceValidator.MaxPrice);

        return
        [
            new UpdateProductRequest(string.Empty, validDescription, validPrice, validCategoryId, validIngredientIds, []),
            new UpdateProductRequest(faker.Random.String2(1, ProductNameValidator.MinLength - 1), validDescription, validPrice, validCategoryId, validIngredientIds, []),
            new UpdateProductRequest(faker.Random.String2(ProductNameValidator.MaxLength + 1, ProductNameValidator.MaxLength + 10), validDescription, validPrice, validCategoryId, validIngredientIds, []),
            new UpdateProductRequest(validName, faker.Random.String2(ProductDescriptionValidator.MaxLength + 1, ProductDescriptionValidator.MaxLength + 10), validPrice, validCategoryId, validIngredientIds, []),
            new UpdateProductRequest(validName, validDescription, ProductPriceValidator.MinPrice - 0.01m, validCategoryId, validIngredientIds, []),
            new UpdateProductRequest(validName, validDescription, ProductPriceValidator.MaxPrice + 0.01m, validCategoryId, validIngredientIds, []),
            new UpdateProductRequest(validName, validDescription, validPrice, Guid.Empty, validIngredientIds, []),
            new UpdateProductRequest(validName, validDescription, validPrice, validCategoryId, [], []),
            new UpdateProductRequest(validName, validDescription, validPrice, validCategoryId, validIngredientIds, [Guid.Empty]),
            new UpdateProductRequest(validName, validDescription, validPrice, validCategoryId, validIngredientIds, validIngredientIds),
        ];
    }
}
