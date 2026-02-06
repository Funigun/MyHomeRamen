namespace MyHomeRamen.Domain.Common.PaymentGroup;

public static class PaymentGroupErrors
{
    public static DomainException NameTooShort()
        => new($"Payment group name is too short. Minimum length is {PaymentGroupConstants.MinNameLength}");

    public static DomainException NameTooLong()
        => new($"Payment group name exceeds maximum length of {PaymentGroupConstants.MaxNameLength}");
}
