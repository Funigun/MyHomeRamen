using MyHomeRamen.Domain.Common.Basket;

namespace MyHomeRamen.Domain.ShoppingCart.Baskets;

internal static class BasketValidator
{
    internal static void Validate(Basket basket)
    {
        CheckUser(basket);
    }

    private static void CheckUser(Basket basket)
    {
        if (basket.User is null)
        {
            throw BasketErrors.BasketUserRequired();
        }
    }
}
