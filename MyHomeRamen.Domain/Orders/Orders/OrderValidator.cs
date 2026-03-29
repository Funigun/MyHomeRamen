using MyHomeRamen.Domain.Common.Order;

namespace MyHomeRamen.Domain.Orders.Orders;

internal static class OrderValidator
{
    internal static void Validate(Order order)
    {
        CheckProducts(order);
        CheckAmount(order);
        CheckDeliveryFee(order);
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
        if (order.Type == OrderType.Delivery && order.TotalOriginalAmount < OrderConstants.MinDeliveryAmount)
        {
            throw OrderErrors.DeliveryAmountTooSmall();
        }
    }
}
