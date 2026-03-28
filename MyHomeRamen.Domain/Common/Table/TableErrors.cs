namespace MyHomeRamen.Domain.Common.Table;

public static class TableErrors
{
    public static DomainException TableNumberTooSmall()
        => new($"Table number cannot be smaller than {TableConstants.MinTableNumber}");

    public static DomainException MinSeatsTooSmall()
        => new($"Minimum number of seats cannot be smaller than {TableConstants.MinSeats}");

    public static DomainException MaxSeatsTooLarge()
        => new($"Maximum number of seats cannot be larger than {TableConstants.MaxSeats}");

    public static DomainException MaxSeatsSmallerThanMinSeats()
        => new("Maximum number of seats cannot be smaller than minimum number of seats");
}
