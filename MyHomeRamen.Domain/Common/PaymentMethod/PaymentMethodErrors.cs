namespace MyHomeRamen.Domain.Common.PaymentMethod;

public static class PaymentMethodErrors
{
    public static DomainException NameTooShort()
        => new($"Payment method name is too short. Minimum length is {PaymentMethodConstants.MinNameLength}");

    public static DomainException NameTooLong()
        => new($"Payment method name exceeds maximum length of {PaymentMethodConstants.MaxNameLength}");

    public static DomainException SortOrderTooLow()
    => new($"Payment method sort order cannot be less than {PaymentMethodConstants.MinSortOrder}");
}
