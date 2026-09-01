using Bogus;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Ingredients;
using MyHomeRamen.Domain.ShoppingCart.PaymentDetails;
using MyHomeRamen.Domain.ShoppingCart.Products;
using MyHomeRamen.Domain.ShoppingCart.ShippingDetails;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;

internal sealed class DataGenerator
{
    internal static Ingredient CreateIngredient()
        => new Faker<Ingredient>()
           .CustomInstantiator(f =>
           {
               string productName = f.ShoppingCart().IngredientName();

               return Ingredient.Create
                      (
                          Guid.NewGuid(),
                          Guid.NewGuid(),
                          productName,
                          f.ShoppingCart().IngredientDescription(productName),
                          f.Random.Number(),
                          1
                      );
           }).Generate();

    internal static Product CreateProduct(IEnumerable<Ingredient> baseIngredients, IEnumerable<Ingredient> customIngredients)
        => new Faker<Product>()
           .CustomInstantiator(f =>
           {
               string productName = f.ShoppingCart().ProductName();

               return Product.Create
                        (
                             Guid.NewGuid(),
                             f.ShoppingCart().ProductOriginalId(productName),
                             productName,
                             f.ShoppingCart().ProductDescription(productName),
                             f.Random.Number(1, 45),
                             "",
                             baseIngredients.ToList(),
                             customIngredients.ToList()
                        );
           }).Generate();

    internal static Product CreateInvalidProduct(IEnumerable<Ingredient> baseIngredients, IEnumerable<Ingredient> customIngredients)
    => new Faker<Product>()
       .CustomInstantiator(f =>
       {
           string productName = f.ShoppingCart().ProductName();

           return Product.Create
                    (
                         Guid.NewGuid(),
                         Guid.NewGuid(),
                         productName,
                         f.ShoppingCart().ProductDescription(productName),
                         f.Random.Number(),
                         "",
                         baseIngredients.ToList(),
                         customIngredients.ToList()
                    );
       }).Generate();

    internal static BasketItem CreateBasketItem(Product product)
    => new Faker<BasketItem>()
       .CustomInstantiator(f => BasketItem.Create(Guid.NewGuid(), product, 1, ""))
       .Generate();

    internal static Basket CreateBasket(IEnumerable<BasketItem> basketItems, UserId user, ShippingDetails? shippingDetails = null, PaymentDetails? paymentDetails = null)
        => new Faker<Basket>()
           .CustomInstantiator(f =>
           {
               Basket basket = Basket.Create(Guid.NewGuid(), user);

               foreach (BasketItem item in basketItems)
               {
                   basket.AddItem(item);
               }

               if (shippingDetails is not null)
               {
                   basket.UpdateShippingDetails(shippingDetails);
               }

               if (paymentDetails is not null)
               {
                   basket.UpdatePaymentDetails(paymentDetails);
               }

               return basket;
           }).Generate();
}
