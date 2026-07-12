namespace MyHomeRamen.Domain.Common.PaymentGateway;

public static class PaymentGatewayErrors
{
    public static DomainException NameTooShort()
        => new($"Payment gateway name is too short. Minimum length is {PaymentGatewayConstants.MinNameLength}");

    public static DomainException NameTooLong()
        => new($"Payment gateway name exceeds maximum length of {PaymentGatewayConstants.MaxNameLength}");
}
