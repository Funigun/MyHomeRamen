using System.Linq;
using MyHomeRamen.Domain.Common.Order;

namespace MyHomeRamen.Domain.Orders.Orders;

internal static class OrderValidator
{
    internal static void Validate(Order order)
    {
        CheckProducts(order);
    }

    private static void CheckProducts(Order order)
    {
        if (order.ProductId.Count == 0)
        {
            throw OrderErrors.OrderMustHaveProducts();
        }

        if (order.ProductId.Count > OrderConstants.MaxProductsCount)
        {
            throw OrderErrors.TooManyProducts();
        }
    }
}
