namespace MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;

public sealed class WorkingHours
{
    public DateOnly Day { get; private set; }

    public TimeSpan OpenTime { get; private set; }

    public TimeSpan CloseTime { get; private set; }

    private WorkingHours() { }

    public static WorkingHours Create(DateOnly day, TimeSpan openTime, TimeSpan closeTime)
    {
        WorkingHours workingHours = new()
        {
            Day = day,
            OpenTime = openTime,
            CloseTime = closeTime
        };

        WorkingHoursValidator.Validate(workingHours);
        return workingHours;
    }

    public void Update(TimeSpan openTime, TimeSpan closeTime)
    {
        OpenTime = openTime;
        CloseTime = closeTime;
        WorkingHoursValidator.Validate(this);
    }
}
