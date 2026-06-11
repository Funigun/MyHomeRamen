using MyHomeRamen.Domain.Common.PaymentGateway;

namespace MyHomeRamen.Domain.Payments.PaymentGateways;

internal static class PaymentGatewayValidator
{
    internal static void Validate(PaymentGateway paymentGateway)
    {
        CheckName(paymentGateway);
    }

    private static void CheckName(PaymentGateway paymentGateway)
    {
        if (paymentGateway.Name.Length < PaymentGatewayConstants.MinNameLength)
        {
            throw PaymentGatewayErrors.NameTooShort();
        }

        if (paymentGateway.Name.Length > PaymentGatewayConstants.MaxNameLength)
        {
            throw PaymentGatewayErrors.NameTooLong();
        }
    }
}
