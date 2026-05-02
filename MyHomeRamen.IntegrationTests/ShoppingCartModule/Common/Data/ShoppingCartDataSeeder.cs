using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.Persistance.ShoppingCart;

namespace MyHomeRamen.IntegrationTests.ShoppingCartModule.Common.Data;

internal static class ShoppingCartDataSeeder
{
    internal static async Task SeedShoppingCartModule(ShoppingCartDbContext dbContext)
    {
        await dbContext.Migrate(TestContext.Current.CancellationToken);
        await dbContext.Seed(ApiConfig.RestaurantId, TestContext.Current.CancellationToken);

        List<Product> products = DataGenerator.GenerateValidProducts(5).ToList();
        dbContext.Products.AddRange(products);

        List<User> testUsers = DataGenerator.GenerateTestUser();
        dbContext.Users.AddRange(testUsers);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        Basket basket = DataGenerator.GenerateValidBasket(testUsers[0], products);
        Basket guestBasket = DataGenerator.GenerateValidBasket(testUsers[1], products);

        dbContext.ShoppingCarts.Add(basket);
        dbContext.ShoppingCarts.Add(guestBasket);
        dbContext.BasketItems.AddRange(basket.Items);
        dbContext.BasketItems.AddRange(guestBasket.Items);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
