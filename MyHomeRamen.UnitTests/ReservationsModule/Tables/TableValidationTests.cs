using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.Table;
using MyHomeRamen.Domain.Reservations.Tables;

namespace MyHomeRamen.UnitTests.ReservationsModule.Tables;

public class TableValidationTests
{
    private static readonly TableId DefaultId = new(Guid.NewGuid());
    private static readonly Guid DefaultRestaurantId = Guid.NewGuid();
    private const int DefaultTableNumber = 1;
    private const int DefaultMinSeats = 2;
    private const int DefaultMaxSeats = 4;

    [Fact]
    public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
    {
        // Act
        Table table = Table.Create(DefaultId, DefaultRestaurantId, DefaultTableNumber, DefaultMinSeats, DefaultMaxSeats);

        // Assert
        Assert.Equal(DefaultId, table.Id);
        Assert.Equal(DefaultTableNumber, table.TableNumber);
        Assert.Equal(DefaultMinSeats, table.MinNumberOfSeats);
        Assert.Equal(DefaultMaxSeats, table.MaxNumberOfSeats);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_TableNumberIsTooSmall()
    {
        // Arrange
        int tableNumber = TableConstants.MinTableNumber - 1;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateTable(tableNumber: tableNumber));
        Assert.Equal(TableErrors.TableNumberTooSmall().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_MinSeatsIsTooSmall()
    {
        // Arrange
        int minSeats = TableConstants.MinSeats - 1;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateTable(minSeats: minSeats));
        Assert.Equal(TableErrors.MinSeatsTooSmall().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_MaxSeatsIsTooLarge()
    {
        // Arrange
        int maxSeats = TableConstants.MaxSeats + 1;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateTable(maxSeats: maxSeats));
        Assert.Equal(TableErrors.MaxSeatsTooLarge().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_MaxSeatsIsSmallerThanMinSeats()
    {
        // Arrange
        int minSeats = 4;
        int maxSeats = 3;

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateTable(minSeats: minSeats, maxSeats: maxSeats));
        Assert.Equal(TableErrors.MaxSeatsSmallerThanMinSeats().Message, exception.Message);
    }

    private static Table CreateTable(
        int? tableNumber = null,
        int? minSeats = null,
        int? maxSeats = null)
    {
        return Table.Create(
            DefaultId,
            DefaultRestaurantId,
            tableNumber ?? DefaultTableNumber,
            minSeats ?? DefaultMinSeats,
            maxSeats ?? DefaultMaxSeats);
    }
}
