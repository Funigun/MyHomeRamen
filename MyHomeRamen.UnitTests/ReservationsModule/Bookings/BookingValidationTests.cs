using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Booking;
using MyHomeRamen.Domain.Reservations.Bookings;
using MyHomeRamen.Domain.Reservations.Tables;

namespace MyHomeRamen.UnitTests.ReservationsModule.Bookings;

public class BookingValidationTests
{
    private static readonly BookingId DefaultId = new(Guid.NewGuid());
    private static readonly Guid DefaultRestaurantId = Guid.NewGuid();

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Arrange
        Table table = CreateTable();
        List<Table> tables = [table];

        // Act
        Booking booking = Booking.Create(DefaultId, tables);

        // Assert
        Assert.Equal(DefaultId, booking.Id);
        Assert.Equal(tables, booking.Tables);
        Assert.Equal(BookingStatus.Created, booking.Status);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_NoTablesAssigned()
    {
        // Arrange
        List<Table> tables = [];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => Booking.Create(DefaultId, tables));
        Assert.Equal(BookingErrors.NoTablesAssigned().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_TooManyTables()
    {
        // Arrange
        List<Table> tables = Enumerable.Range(0, BookingConstants.MaxTables + 1)
            .Select(_ => CreateTable())
            .ToList();

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => Booking.Create(DefaultId, tables));
        Assert.Equal(BookingErrors.TooManyTables().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_TablesNotUnique()
    {
        // Arrange
        Table table = CreateTable();
        List<Table> tables = [table, table];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => Booking.Create(DefaultId, tables));
        Assert.Equal(BookingErrors.TablesNotUnique().Message, exception.Message);
    }

    [Fact]
    public void Confirm_Should_ChangeStatusToConfirmed()
    {
        // Arrange
        Booking booking = CreateBooking();

        // Act
        booking.Confirm();

        // Assert
        Assert.Equal(BookingStatus.Confirmed, booking.Status);
    }

    [Fact]
    public void Cancel_Should_ChangeStatusToCancelled()
    {
        // Arrange
        Booking booking = CreateBooking();

        // Act
        booking.Cancel();

        // Assert
        Assert.Equal(BookingStatus.Cancelled, booking.Status);
    }

    [Fact]
    public void MarkAsCompleted_Should_ChangeStatusToPaid()
    {
        // Arrange
        Booking booking = CreateBooking();

        // Act
        booking.MarkAsCompleted();

        // Assert
        Assert.Equal(BookingStatus.Paid, booking.Status);
    }

    private static Booking CreateBooking()
    {
        return Booking.Create(DefaultId, [CreateTable()]);
    }

    private static Table CreateTable()
    {
        return Table.Create(new TableId(Guid.NewGuid()), 1, 2, 4);
    }
}
