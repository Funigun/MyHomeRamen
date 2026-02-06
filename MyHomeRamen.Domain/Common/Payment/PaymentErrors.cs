namespace MyHomeRamen.Domain.Common.Payment;

public static class PaymentErrors
{
    public static DomainException NameTooShort()
        => new($"Payment name is too short. Minimum length is {PaymentConstants.MinNameLength}");

    public static DomainException NameTooLong()
        => new($"Payment name exceeds maximum length of {PaymentConstants.MaxNameLength}");
}
