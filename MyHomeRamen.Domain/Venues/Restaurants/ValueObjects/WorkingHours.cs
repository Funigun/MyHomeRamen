namespace MyHomeRamen.Domain.Venues.Restaurants.ValueObjects;

public sealed class WorkingHours
{
    public DateOnly Day { get; private set; }

    public TimeSpan OpenTime { get; private set; }

    public TimeSpan CloseTime { get; private set; }

    private WorkingHours() { }

    public static WorkingHours Create(DateOnly day, TimeSpan openTime, TimeSpan closeTime)
    {
        return new WorkingHours
        {
            Day = day,
            OpenTime = openTime,
            CloseTime = closeTime
        };
    }

    public void Update(TimeSpan openTime, TimeSpan closeTime)
    {
        OpenTime = openTime;
        CloseTime = closeTime;
    }
}
