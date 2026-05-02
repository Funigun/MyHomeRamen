using Bogus;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.IntegrationTests.ShoppingCartModule.Common.Data;

internal static class DataGenerator
{
    internal static IEnumerable<Basket> GeneratedBaskets { get; private set; } = [];

    internal static Basket GenerateValidBasket(User testUser, IEnumerable<Product> products)
    {
        Faker faker = new Faker();
        Faker<BasketItem> itemsFaker = new Faker<BasketItem>()
            .CustomInstantiator(f => BasketItem.Create(
                new BasketItemId(Guid.NewGuid()),
                f.PickRandom(products),
                f.Random.Int(1, 10),
                Math.Round(f.Random.Decimal(0.5m, 100m), 2),
                null
            ));

        List<BasketItem> basketItems = itemsFaker.Generate(faker.Random.Int(1, 3));

        Basket basket = Basket.Create(new BasketId(Guid.NewGuid()), testUser);

        System.Reflection.FieldInfo basketItemsField = typeof(Basket).GetField("_items", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!;
        List<BasketItem> currentItems = (List<BasketItem>)basketItemsField.GetValue(basket)!;
        currentItems.AddRange(basketItems);

        List<Basket> baskets = GeneratedBaskets.ToList();
        baskets.Add(basket);
        GeneratedBaskets = baskets;

        return basket;
    }

    internal static List<User> GenerateTestUser() =>
    [
        User.Create(new UserId(Guid.CreateVersion7()), roles: [], permissions: [], isGuest: false),
        User.Create(new UserId(Guid.CreateVersion7()), roles: [], permissions: [], isGuest: true)
     ];

    internal static IEnumerable<Product> GenerateValidProducts(int count)
    {
        Faker<Product> productFaker = new Faker<Product>()
            .CustomInstantiator(f => Product.Create(
                new ProductId(Guid.NewGuid()),
                new ProductId(Guid.NewGuid()),
                f.Commerce.ProductName(),
                f.Random.String2(51, 100),
                Math.Round(f.Random.Decimal(1, 100), 2),
                f.Random.String2(51, 100),
                [],
                []
            ));

        return productFaker.Generate(count);
    }
}
