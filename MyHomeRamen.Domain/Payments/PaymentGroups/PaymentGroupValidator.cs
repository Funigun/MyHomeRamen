using MyHomeRamen.Domain.Common.PaymentGroup;

namespace MyHomeRamen.Domain.Payments.PaymentGroups;

internal static class PaymentGroupValidator
{
    internal static void Validate(PaymentGroup paymentGroup)
    {
        CheckName(paymentGroup);
    }

    private static void CheckName(PaymentGroup paymentGroup)
    {
        if (paymentGroup.Name.Length < PaymentGroupConstants.MinNameLength)
        {
            throw PaymentGroupErrors.NameTooShort();
        }

        if (paymentGroup.Name.Length > PaymentGroupConstants.MaxNameLength)
        {
            throw PaymentGroupErrors.NameTooLong();
        }
    }
}
