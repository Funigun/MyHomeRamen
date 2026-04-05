using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Domain.Menu.Users;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.Persistance.Menu;

namespace MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

internal static class DataSeeder
{
    internal static async Task SeedMenuModule(MenuDbContext dbContext)
    {
        await dbContext.Migrate(TestContext.Current.CancellationToken);
        await dbContext.Seed(ApiConfig.RestaurantId, TestContext.Current.CancellationToken);

        IEnumerable<Role> roles = await dbContext.Roles.Include(r => r.Permissions).ToListAsync(TestContext.Current.CancellationToken);
        IEnumerable<User> users = DataGenerator.GenerateValidUsers(roles);

        dbContext.Users.AddRange(users);

        List<Domain.Menu.Categories.Category> categories =
        [
            ..DataGenerator.GenerateValidCategories(3, Domain.Menu.Categories.CategoryType.Product),
            ..DataGenerator.GenerateValidCategories(3, Domain.Menu.Categories.CategoryType.Ingredient),
        ];
        List<Domain.Menu.Ingredients.Ingredient> ingredients = DataGenerator.GenerateValidIngredients(10);
        List<Domain.Menu.Products.Product> products = DataGenerator.GenerateValidProducts(20);

        dbContext.Categories.AddRange(categories);
        dbContext.Ingredients.AddRange(ingredients);
        dbContext.Products.AddRange(products);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
