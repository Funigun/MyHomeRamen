using MyHomeRamen.Domain.Common.Booking;

namespace MyHomeRamen.Domain.Reservations.Bookings;

internal static class BookingValidator
{
    internal static void Validate(Booking booking)
    {
        CheckTables(booking);
    }

    private static void CheckTables(Booking booking)
    {
        if (booking.Tables.Count == 0)
        {
            throw BookingErrors.NoTablesAssigned();
        }

        if (booking.Tables.Count > BookingConstants.MaxTables)
        {
            throw BookingErrors.TooManyTables();
        }

        if (booking.Tables.DistinctBy(t => t.Id).Count() != booking.Tables.Count)
        {
            throw BookingErrors.TablesNotUnique();
        }
    }
}
