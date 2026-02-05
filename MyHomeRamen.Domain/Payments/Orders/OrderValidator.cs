using MyHomeRamen.Domain.Common.Order;

namespace MyHomeRamen.Domain.Payments.Orders;

internal static class OrderValidator
{
    internal static void Validate(Order order)
    {
        CheckAmount(order);
    }

    private static void CheckAmount(Order order)
    {
        if (order.Amount < OrderConstants.MinAmount)
        {
            throw OrderErrors.AmountTooSmall();
        }

        if (order.Amount > OrderConstants.MaxAmount)
        {
            throw OrderErrors.AmountTooLarge();
        }
    }
}
