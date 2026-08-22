using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.UnitTests.UsersModule.Users;

public class UserValidationTests
{
    private const string ValidKeycloakUserId = "keycloak-user-id";
    private const string ValidUserName = "testuser";
    private const string ValidFirstName = "Test";
    private const string ValidLastName = "User";
    private const string ValidEmail = "test@example.com";
    private const string ValidPhoneNumber = "123456789";
    private static readonly Role ValidRole = Role.Create("customer", "customer role");

    [Fact]
    public void Create_Should_SetEmail_When_DataIsValid()
    {
        // Act
        User user = User.Create(ValidKeycloakUserId, ValidUserName, ValidFirstName, ValidLastName, ValidEmail, ValidPhoneNumber, ValidRole);

        // Assert
        Assert.Equal(ValidEmail, user.Email);
    }

    [Fact]
    public void Create_Should_SetFirstName_When_DataIsValid()
    {
        // Act
        User user = User.Create(ValidKeycloakUserId, ValidUserName, ValidFirstName, ValidLastName, ValidEmail, ValidPhoneNumber, ValidRole);

        // Assert
        Assert.Equal(ValidFirstName, user.FirstName);
    }

    [Fact]
    public void Create_Should_SetLastName_When_DataIsValid()
    {
        // Act
        User user = User.Create(ValidKeycloakUserId, ValidUserName, ValidFirstName, ValidLastName, ValidEmail, ValidPhoneNumber, ValidRole);

        // Assert
        Assert.Equal(ValidLastName, user.LastName);
    }

    [Fact]
    public void Create_Should_SetUserName_When_DataIsValid()
    {
        // Act
        User user = User.Create(ValidKeycloakUserId, ValidUserName, ValidFirstName, ValidLastName, ValidEmail, ValidPhoneNumber, ValidRole);

        // Assert
        Assert.Equal(ValidUserName, user.UserName);
    }

    [Fact]
    public void Create_Should_SetRole_When_DataIsValid()
    {
        // Act
        User user = User.Create(ValidKeycloakUserId, ValidUserName, ValidFirstName, ValidLastName, ValidEmail, ValidPhoneNumber, ValidRole);

        // Assert
        Assert.Contains(user.Roles, r => r.Name == ValidRole.Name);
    }
}
