using MyHomeRamen.Domain.Common.Order;

namespace MyHomeRamen.Domain.Orders.Orders;

internal static class OrderValidator
{
    internal static void Validate(Order order)
    {
        CheckRestaurantAndCurrency(order);
        CheckProducts(order);
        CheckAmount(order);
        CheckDeliveryFee(order);
    }

    private static void CheckRestaurantAndCurrency(Order order)
    {
        if (order.UserId.Value == Guid.Empty)
        {
            throw OrderErrors.UserRequired();
        }

        if (order.RestaurantId == Guid.Empty)
        {
            throw OrderErrors.RestaurantRequired();
        }

        if (order.Currency.Length != 3 || order.Currency.Any(character => !char.IsAsciiLetter(character)))
        {
            throw OrderErrors.CurrencyRequired();
        }
    }

    private static void CheckProducts(Order order)
    {
        if (order.Products.Count == 0)
        {
            throw OrderErrors.OrderMustHaveProducts();
        }

        if (order.Products.Count > OrderConstants.MaxProductsCount)
        {
            throw OrderErrors.TooManyProducts();
        }

        if (order.Products.Any(product =>
            product.RestaurantProductOfferId == Guid.Empty
            || product.Quantity < 1
            || product.FinalChargedLinePrice < 0))
        {
            throw OrderErrors.InvalidOrderLine();
        }
    }

    private static void CheckAmount(Order order)
    {
        if (order.TotalOriginalAmount < OrderConstants.MinTotalAmount)
        {
            throw OrderErrors.AmountTooSmall();
        }

        if (order.TotalOriginalAmount > OrderConstants.MaxTotalAmount)
        {
            throw OrderErrors.AmountTooLarge();
        }
    }

    private static void CheckDeliveryFee(Order order)
    {
        if (order.Type == OrderType.Delivery && order.DeliveryAddress is null)
        {
            throw OrderErrors.DeliveryAddressRequired();
        }

        if (order.Type == OrderType.Delivery && order.TotalOriginalAmount < OrderConstants.MinDeliveryAmount)
        {
            throw OrderErrors.DeliveryAmountTooSmall();
        }
    }
}
