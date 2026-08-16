using MyHomeRamen.Domain.Abstractions;

namespace MyHomeRamen.Domain.Venues.Restaurants;

public sealed class ClosingPeriod : AuditableEntity, IEntity<ClosingPeriodId>
{
    public ClosingPeriodId Id { get; private set; }

    public DateTimeOffset StartTime { get; private set; } = DateTimeOffset.MinValue;

    public DateTimeOffset EndTime { get; private set; } = DateTimeOffset.MinValue;

    public string Reason { get; private set; } = string.Empty;

    public bool IsActive { get; private set; }
    
    private ClosingPeriod()
    {
    }

    public static ClosingPeriod Create(DateTimeOffset startTime, DateTimeOffset endTime, string reason)
    {
        return new ClosingPeriod
        {
            Id = new ClosingPeriodId(Guid.CreateVersion7()),
            StartTime = startTime,
            EndTime = endTime,
            Reason = reason,
            IsActive = true
        };
    }

    public void Deactivate() => IsActive = false;
    
    public void UpdatePeriod(DateTimeOffset startTime, DateTimeOffset endTime, string reason)
    {
        StartTime = startTime;
        EndTime = endTime;
        Reason = reason;
    }
}
