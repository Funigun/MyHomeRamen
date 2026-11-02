using MyHomeRamen.Domain.Common.Basket;

namespace MyHomeRamen.Domain.ShoppingCart.BasketItems;

internal static class BasketItemValidator
{
    internal static void Validate(BasketItem item)
    {
        CheckOffer(item);
        CheckProduct(item);
        CheckQuantity(item);
    }

    private static void CheckOffer(BasketItem item)
    {
        if (item.RestaurantProductOfferId == Guid.Empty)
        {
            throw BasketErrors.BasketOfferRequired();
        }
    }

    private static void CheckProduct(BasketItem item)
    {
        if (item.Product is null)
        {
            throw BasketErrors.BasketItemProductRequired();
        }
    }

    private static void CheckQuantity(BasketItem item)
    {
        if (item.Quantity < BasketConstants.MinQuantity)
        {
            throw BasketErrors.BasketItemQuantityInvalid();
        }
    }
}
