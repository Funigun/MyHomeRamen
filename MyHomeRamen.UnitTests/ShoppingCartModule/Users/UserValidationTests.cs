using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.UnitTests.ShoppingCartModule.Users;

public class UserValidationTests
{
    private static readonly UserId DefaultId = new(Guid.NewGuid());

    [Fact]
    public void Create_Should_SetIsGuest_When_IsGuestFlagProvided()
    {
        // Arrange
        bool isGuest = true;

        // Act
        User user = User.Create(DefaultId, [], [], isGuest);

        // Assert
        Assert.True(user.IsGuest);
    }

    [Fact]
    public void Create_Should_DefaultIsGuestToFalse_When_NotProvided()
    {
        // Act
        User user = User.Create(DefaultId, [], []);

        // Assert
        Assert.False(user.IsGuest);
    }
}
