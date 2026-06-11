namespace MyHomeRamen.Domain.Common.PaymentChannel;

public static class PaymentChannelErrors
{
    public static DomainException NameTooShort()
        => new($"Payment channel name is too short. Minimum length is {PaymentChannelConstants.MinNameLength}");

    public static DomainException NameTooLong()
        => new($"Payment channel name exceeds maximum length of {PaymentChannelConstants.MaxNameLength}");

    public static DomainException SortOrderTooLow()
        => new($"Payment channel sort order cannot be less than {PaymentChannelConstants.MinSortOrder}");
}
