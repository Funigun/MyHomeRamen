using MyHomeRamen.Domain.Common.Basket;

namespace MyHomeRamen.Domain.ShoppingCart.BasketItems;

internal static class BasketItemValidator
{
    internal static void Validate(BasketItem item)
    {
        CheckProduct(item);
        CheckQuantity(item);
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
