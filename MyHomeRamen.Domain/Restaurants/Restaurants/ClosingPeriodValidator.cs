using MyHomeRamen.Domain.Common.ClosingPeriod;

namespace MyHomeRamen.Domain.Restaurants.Restaurants;

internal static class ClosingPeriodValidator
{
    internal static void Validate(ClosingPeriod closingPeriod)
    {
        if (closingPeriod.StartTime == DateTimeOffset.MinValue)
        {
            throw ClosingPeriodErrors.StartTimeRequired();
        }

        if (closingPeriod.EndTime == DateTimeOffset.MinValue)
        {
            throw ClosingPeriodErrors.EndTimeRequired();
        }

        if (closingPeriod.EndTime < closingPeriod.StartTime)
        {
            throw ClosingPeriodErrors.EndTimeBeforeStartTime();
        }

        if (string.IsNullOrWhiteSpace(closingPeriod.Reason))
        {
            throw ClosingPeriodErrors.ReasonRequired();
        }

        if (closingPeriod.Reason.Length > ClosingPeriodConstants.MaxReasonLength)
        {
            throw ClosingPeriodErrors.ReasonTooLong();
        }
    }
}
