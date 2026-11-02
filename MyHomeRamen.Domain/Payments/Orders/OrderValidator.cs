using MyHomeRamen.Domain.Common.Order;

namespace MyHomeRamen.Domain.Payments.Orders;

internal static class OrderValidator
{
    internal static void Validate(Order order)
    {
        if (order.CompanyId == Guid.Empty)
        {
            throw OrderErrors.CompanyRequired();
        }

        CheckAmount(order);
    }

    private static void CheckAmount(Order order)
    {
        if (order.Amount < OrderConstants.MinTotalAmount)
        {
            throw OrderErrors.AmountTooSmall();
        }

        if (order.Amount > OrderConstants.MaxTotalAmount)
        {
            throw OrderErrors.AmountTooLarge();
        }
    }
}
