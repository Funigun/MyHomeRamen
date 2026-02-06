namespace MyHomeRamen.Domain.Common.PaymentProvider;

public static class PaymentProviderErrors
{
    public static DomainException NameTooShort()
    => new($"Payment Provider name is too short. Minimum length is {PaymentProviderConstants.MinNameLength}");

    public static DomainException NameTooLong()
        => new($"Payment Provider name exceeds maximum length of {PaymentProviderConstants.MaxNameLength}");
}
