using MyHomeRamen.Domain.Common.PaymentMethod;

namespace MyHomeRamen.Domain.Payments.PaymentMethods;

internal static class PaymentMethodValidator
{
    internal static void Validate(PaymentMethod paymentMethod)
    {
        CheckName(paymentMethod);
        CheckDisplayOrder(paymentMethod.DisplayOrder);
    }

    internal static void ValidateDisplayOrder(PaymentMethod paymentMethod)
    {
        CheckDisplayOrder(paymentMethod.DisplayOrder);
    }

    private static void CheckName(PaymentMethod paymentMethod)
    {
        if (paymentMethod.Name.Length < PaymentMethodConstants.MinNameLength)
        {
            throw PaymentMethodErrors.NameTooShort();
        }

        if (paymentMethod.Name.Length > PaymentMethodConstants.MaxNameLength)
        {
            throw PaymentMethodErrors.NameTooLong();
        }
    }

    internal static void CheckDisplayOrder(int displayOrder)
    {
        if (displayOrder < PaymentMethodConstants.MinSortOrder)
        {
            throw PaymentMethodErrors.SortOrderTooLow();
        }
    }
}
