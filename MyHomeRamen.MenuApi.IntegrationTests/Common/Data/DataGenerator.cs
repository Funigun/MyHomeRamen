using System.Collections.ObjectModel;
using Bogus;
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

    internal static IEnumerable<Category> CreateIngredientCategories()
    {
        List<Category> categories = [];

        Faker faker = new();

        foreach(string name in faker.RamenMenu().IngredientCategoryNames())
        {
            categories.Add(CreateIngredientCategory(name));
        }

        return categories;
    }

    internal static Ingredient CreateIngredient(Category category, string? name = null)
        => new Faker<Ingredient>()
           .CustomInstantiator(f =>
           {
               string ingName = name ?? f.RamenMenu().IngredientName();
               string ingDescription = name is null ? f.RamenMenu().IngredientDescription(ingName) : $"{name} description";

               return Ingredient.Create
                      (
                          Guid.NewGuid(),
                          ingName,
                          ingDescription,
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
                      Math.Round(f.Random.Decimal(ProductConstants.MinPrice, ProductConstants.MaxPrice), 2),
                      "",
                      baseIngredients,
                      customIngredients,
                      [category]
                  );
       }).Generate();
}
