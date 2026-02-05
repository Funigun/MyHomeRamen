using MyHomeRamen.Domain.Common.Payment;

namespace MyHomeRamen.Domain.Payments.Payments;

internal static class PaymentValidator
{
    internal static void Validate(Payment payment)
    {
        CheckName(payment);
    }

    private static void CheckName(Payment payment)
    {
        if (payment.Name.Length < PaymentConstants.MinNameLength)
        {
            throw PaymentErrors.NameTooShort();
        }

        if (payment.Name.Length > PaymentConstants.MaxNameLength)
        {
            throw PaymentErrors.NameTooLong();
        }
    }
}
