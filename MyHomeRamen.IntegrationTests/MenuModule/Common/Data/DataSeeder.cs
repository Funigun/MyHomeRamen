using MyHomeRamen.Domain.Menu.Roles;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

internal static class DataSeeder
{
    internal static async Task SeedMenuModule(IMenuDbContext dbContext)
    {
        await dbContext.Migrate(TestContext.Current.CancellationToken);
        await dbContext.Seed(TestContext.Current.CancellationToken);

        IEnumerable<Role> roles = await dbContext.Role.Specification().GetAllWithPermissions(TestContext.Current.CancellationToken);

        IEnumerable<User> users = DataGenerator.GenerateValidUsers(roles);

        dbContext.User.AddRange(users);

        List<Domain.Menu.Categories.Category> categories =
        [
            ..DataGenerator.GenerateValidCategories(3, Domain.Menu.Categories.CategoryType.Product),
            ..DataGenerator.GenerateValidCategories(3, Domain.Menu.Categories.CategoryType.Ingredient),
        ];
        List<Domain.Menu.Ingredients.Ingredient> ingredients = DataGenerator.GenerateValidIngredients(10);
        List<Domain.Menu.Products.Product> products = DataGenerator.GenerateValidProducts(20);

        dbContext.Category.AddRange(categories);
        dbContext.Ingredient.AddRange(ingredients);
        dbContext.Product.AddRange(products);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
