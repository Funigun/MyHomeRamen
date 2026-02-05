namespace MyHomeRamen.Domain.Common.PaymentProvider;

public static class PaymentProviderErrors
{
    public static DomainException NameTooShort()
    => new($"Payment name is too short. Minimum length is {PaymentProviderConstants.MinNameLength}");

    public static DomainException NameTooLong()
        => new($"Payment name exceeds maximum length of {PaymentProviderConstants.MaxNameLength}");
}
