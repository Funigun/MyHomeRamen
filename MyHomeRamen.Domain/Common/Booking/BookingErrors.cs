namespace MyHomeRamen.Domain.Common.Booking;

public static class BookingErrors
{
    public static DomainException NoTablesAssigned()
        => new("Booking must have at least one table assigned.");

    public static DomainException TooManyTables()
        => new($"Booking cannot have more than {BookingConstants.MaxTables} tables.");

    public static DomainException TablesNotUnique()
        => new("Booking tables must be unique.");
}
