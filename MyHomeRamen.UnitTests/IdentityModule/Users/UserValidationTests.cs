using MyHomeRamen.Domain.Identity.Users;

namespace MyHomeRamen.UnitTests.IdentityModule.Users;

public class UserValidationTests
{
    private const string ValidKeycloakUserId = "keycloak-user-id";
    private const string ValidUserName = "testuser";
    private const string ValidFirstName = "Test";
    private const string ValidLastName = "User";
    private const string ValidEmail = "test@example.com";
    private const string ValidPhoneNumber = "123456789";

    [Fact]
    public void Create_Should_SetEmail_When_DataIsValid()
    {
        // Act
        User user = User.Create(ValidKeycloakUserId, ValidUserName, ValidFirstName, ValidLastName, ValidEmail, ValidPhoneNumber);

        // Assert
        Assert.Equal(ValidEmail, user.Email);
    }

    [Fact]
    public void Create_Should_SetFirstName_When_DataIsValid()
    {
        // Act
        User user = User.Create(ValidKeycloakUserId, ValidUserName, ValidFirstName, ValidLastName, ValidEmail, ValidPhoneNumber);

        // Assert
        Assert.Equal(ValidFirstName, user.FirstName);
    }

    [Fact]
    public void Create_Should_SetLastName_When_DataIsValid()
    {
        // Act
        User user = User.Create(ValidKeycloakUserId, ValidUserName, ValidFirstName, ValidLastName, ValidEmail, ValidPhoneNumber);

        // Assert
        Assert.Equal(ValidLastName, user.LastName);
    }

    [Fact]
    public void Create_Should_SetUserName_When_DataIsValid()
    {
        // Act
        User user = User.Create(ValidKeycloakUserId, ValidUserName, ValidFirstName, ValidLastName, ValidEmail, ValidPhoneNumber);

        // Assert
        Assert.Equal(ValidUserName, user.UserName);
    }

    [Fact]
    public void Create_ShouldNotAssignRoles_When_DataIsValid()
    {
        User user = User.Create(ValidKeycloakUserId, ValidUserName, ValidFirstName, ValidLastName, ValidEmail, ValidPhoneNumber);

        Assert.Empty(user.UserRoles);
    }
}
