using Bogus;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Common.Contracts.Basket;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MenuDataGenerator = MyHomeRamen.IntegrationTests.MenuModule.Common.Data.DataGenerator;
using MenuProduct = MyHomeRamen.Domain.Menu.Products.Product;

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

    internal static AddItemToBasketRequest ValidAddItemToBasketRequest()
    {
        MenuProduct menuProduct = GetRandomMenuProduct();

        List<BasketIngredientDto> baseIngredients = menuProduct.BaseIngredients
            .Select(i => new BasketIngredientDto(i.Id, 1))
            .ToList();

        List<BasketIngredientDto> customIngredients = menuProduct.CustomIngredients
            .Select(i => new BasketIngredientDto(i.Id, 1))
            .ToList();

        return new AddItemToBasketRequest(
            menuProduct.Id,
            1,
            baseIngredients,
            customIngredients,
            null);
    }

    public static TheoryData<AddItemToBasketRequest> InvalidAddItemToBasketRequests()
    {
        MenuProduct menuProduct = GetRandomMenuProduct();

        List<BasketIngredientDto> validBaseIngredients = menuProduct.BaseIngredients
            .Select(i => new BasketIngredientDto(i.Id, 1))
            .ToList();

        List<BasketIngredientDto> validCustomIngredients = menuProduct.CustomIngredients
            .Select(i => new BasketIngredientDto(i.Id, 1))
            .ToList();

        string tooLongComment = new('a', BasketItemCommentValidator.MaxCommentLength + 1);

        return
        [
            // Quantity: below minimum
            new AddItemToBasketRequest(menuProduct.Id, BasketItemQuantityValidator.MinQuantity - 1, validBaseIngredients, validCustomIngredients, null),

            // Quantity: above maximum
            new AddItemToBasketRequest(menuProduct.Id, BasketItemQuantityValidator.MaxQuantity + 1, validBaseIngredients, validCustomIngredients, null),

            // ProductId: empty
            new AddItemToBasketRequest(Guid.Empty, 1, validBaseIngredients, validCustomIngredients, null),

            // Comment: too long
            new AddItemToBasketRequest(menuProduct.Id, 1, validBaseIngredients, validCustomIngredients, tooLongComment),
        ];
    }

    private static MenuProduct GetRandomMenuProduct()
    {
        List<MenuProduct> products = MenuDataGenerator.GeneratedProducts.ToList();
        return products[System.Security.Cryptography.RandomNumberGenerator.GetInt32(products.Count)];
    }
}
