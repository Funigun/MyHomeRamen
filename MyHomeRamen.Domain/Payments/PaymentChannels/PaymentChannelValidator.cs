using MyHomeRamen.Domain.Common.PaymentChannel;

namespace MyHomeRamen.Domain.Payments.PaymentChannels;

internal static class PaymentChannelValidator
{
    internal static void Validate(PaymentChannel paymentChannel)
    {
        CheckName(paymentChannel);
        CheckDisplayOrder(paymentChannel.DisplayOrder);
    }

    internal static void ValidateDisplayOrder(PaymentChannel paymentChannel)
    {
        CheckDisplayOrder(paymentChannel.DisplayOrder);
    }

    private static void CheckName(PaymentChannel paymentChannel)
    {
        if (paymentChannel.Name.Length < PaymentChannelConstants.MinNameLength)
        {
            throw PaymentChannelErrors.NameTooShort();
        }

        if (paymentChannel.Name.Length > PaymentChannelConstants.MaxNameLength)
        {
            throw PaymentChannelErrors.NameTooLong();
        }
    }

    internal static void CheckDisplayOrder(int displayOrder)
    {
        if (displayOrder < PaymentChannelConstants.MinSortOrder)
        {
            throw PaymentChannelErrors.SortOrderTooLow();
        }
    }
}
