using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Common.User;
using MyHomeRamen.Domain.Reservations.Bookings;
using MyHomeRamen.Domain.Reservations.Users;

namespace MyHomeRamen.UnitTests.ReservationsModule.Users;

public class UserValidationTests
{
    private static readonly UserId TestUserId = new(Guid.NewGuid());
    private static readonly Guid TestRestaurantId = Guid.NewGuid();
    private const string ValidFirstName = "John";
    private const string ValidLastName = "Doe";
    private const string ValidEmail = "john.doe@example.com";
    private const string ValidPhoneNumber = "1234567890";

    private static readonly List<Permission> ValidPermissions =
    [
        Permission.Create(new PermissionId(Guid.NewGuid()), TestRestaurantId, PermissionConstants.CanAddBooking, "Permission description")
    ];

    private static readonly List<Role> ValidRoles =
    [
        Role.CreateCustomerRole(new RoleId(Guid.NewGuid()), TestRestaurantId, ValidPermissions)
    ];

    private static readonly List<Booking> ValidBookings = [];

    [Fact]
    public void Create_Should_CreateUser_When_DataIsValid()
    {
        // Act
        User user = CreateUser();

        // Assert
        Assert.NotNull(user);
        Assert.Equal(TestUserId, user.Id);
        Assert.Equal(ValidFirstName, user.FirstName);
        Assert.Equal(ValidLastName, user.LastName);
        Assert.Equal(ValidEmail, user.Email);
        Assert.Equal(ValidPhoneNumber, user.PhoneNumber);
        Assert.Equal(ValidRoles, user.Roles);
        Assert.Equal(ValidPermissions, user.Permissions);
        Assert.Empty(user.Bookings);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_FirstNameIsTooLong()
    {
        // Arrange
        string invalidFirstName = new('a', UserConstants.MaxFirstNameLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(firstName: invalidFirstName));
        Assert.Equal(UserErrors.FirstNameTooLong().Message, exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_ThrowDomainException_When_FirstNameIsInvalid(string? invalidFirstName)
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(firstName: invalidFirstName!));
        Assert.Equal(UserErrors.FirstNameRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_LastNameIsTooLong()
    {
        // Arrange
        string invalidLastName = new('a', UserConstants.MaxLastNameLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(lastName: invalidLastName));
        Assert.Equal(UserErrors.LastNameTooLong().Message, exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_ThrowDomainException_When_LastNameIsInvalid(string? invalidLastName)
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(lastName: invalidLastName!));
        Assert.Equal(UserErrors.LastNameRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_EmailIsTooLong()
    {
        // Arrange
        string invalidEmail = new('a', UserConstants.MaxEmailLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(email: invalidEmail));
        Assert.Equal(UserErrors.EmailTooLong().Message, exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_ThrowDomainException_When_EmailIsInvalid(string? invalidEmail)
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(email: invalidEmail!));
        Assert.Equal(UserErrors.EmailRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_PhoneNumberIsTooLong()
    {
        // Arrange
        string invalidPhoneNumber = new('0', UserConstants.MaxPhoneNumberLength + 1);

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(phoneNumber: invalidPhoneNumber));
        Assert.Equal(UserErrors.PhoneNumberTooLong().Message, exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_Should_ThrowDomainException_When_PhoneNumberIsInvalid(string? invalidPhoneNumber)
    {
        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(phoneNumber: invalidPhoneNumber!));
        Assert.Equal(UserErrors.PhoneNumberRequired().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_RolesListIsEmpty()
    {
        // Arrange
        List<Role> emptyRoles = [];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(roles: emptyRoles));
        Assert.Equal(UserErrors.MissingRole().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_PermissionsListIsEmpty()
    {
        // Arrange
        List<Permission> emptyPermissions = [];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(permissions: emptyPermissions));
        Assert.Equal(UserErrors.MissingPermission().Message, exception.Message);
    }

    [Fact]
    public void Create_Should_ThrowDomainException_When_PermissionNameIsInvalid()
    {
        // Arrange
        List<Permission> invalidPermissions =
        [
            Permission.Create(new PermissionId(Guid.NewGuid()), TestRestaurantId, "InvalidPermission", "Description")
        ];

        // Act & Assert
        DomainException exception = Assert.Throws<DomainException>(() => CreateUser(permissions: invalidPermissions));
        Assert.Equal(UserErrors.InvalidPermissionName().Message, exception.Message);
    }

    private static User CreateUser(
        string firstName = ValidFirstName,
        string lastName = ValidLastName,
        string email = ValidEmail,
        string phoneNumber = ValidPhoneNumber,
        List<Booking>? bookings = null,
        List<Role>? roles = null,
        List<Permission>? permissions = null)
    {
        return User.Create(
            TestUserId,
            TestRestaurantId,
            firstName,
            lastName,
            email,
            phoneNumber,
            bookings ?? ValidBookings,
            roles ?? ValidRoles,
            permissions ?? ValidPermissions);
    }
}
