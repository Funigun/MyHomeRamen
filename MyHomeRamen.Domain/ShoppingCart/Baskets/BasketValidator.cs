using MyHomeRamen.Domain.Common.Basket;

namespace MyHomeRamen.Domain.ShoppingCart.Baskets;

internal static class BasketValidator
{
    internal static void Validate(Basket basket)
    {
        CheckUser(basket);
        CheckRestaurant(basket);
    }

    private static void CheckUser(Basket basket)
    {
        if (basket.UserId.Value == Guid.Empty)
        {
            throw BasketErrors.BasketUserRequired();
        }
    }

    private static void CheckRestaurant(Basket basket)
    {
        if (basket.RestaurantId == Guid.Empty)
        {
            throw BasketErrors.BasketRestaurantRequired();
        }
    }
}
