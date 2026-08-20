namespace MyHomeRamen.Domain.Common.ClosingPeriod;

public static class ClosingPeriodErrors
{
    public static DomainException StartTimeRequired() => new("Closing period start time is required");
    public static DomainException EndTimeRequired() => new("Closing period end time is required");
    public static DomainException EndTimeBeforeStartTime() => new("Closing period end time cannot be before start time");
    public static DomainException ReasonRequired() => new("Closing period reason is required");
    public static DomainException ReasonTooLong() => new($"Closing period reason cannot be longer than {ClosingPeriodConstants.MaxReasonLength} characters");
}
