using MyHomeRamen.Domain.Common.Restaurant;

namespace MyHomeRamen.Domain.Restaurants.Restaurants.ValueObjects;

internal static class WorkingHoursValidator
{
    internal static void Validate(WorkingHours workingHours)
    {
        if (workingHours.Day == DateOnly.MinValue)
        {
            throw RestaurantErrors.WorkingHoursDayRequired();
        }

        if (workingHours.CloseTime < workingHours.OpenTime)
        {
            throw RestaurantErrors.WorkingHoursCloseTimeBeforeOpenTime();
        }
    }
}
