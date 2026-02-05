using MyHomeRamen.Domain.Common.PaymentProvider;

namespace MyHomeRamen.Domain.Payments.PaymentProviders;

internal static class PaymentProviderValidator
{
    internal static void Validate(PaymentProvider paymentProvider)
    {
        CheckName(paymentProvider);
    }

    private static void CheckName(PaymentProvider paymentProvider)
    {
        if (paymentProvider.Name.Length < PaymentProviderConstants.MinNameLength)
        {
            throw PaymentProviderErrors.NameTooShort();
        }

        if (paymentProvider.Name.Length > PaymentProviderConstants.MaxNameLength)
        {
            throw PaymentProviderErrors.NameTooLong();
        }
    }
}
