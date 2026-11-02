using MyHomeRamen.Domain.Common;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.RestaurantIngredientPrices;

namespace MyHomeRamen.UnitTests.MenuModule.RestaurantIngredientPrices;

public sealed class RestaurantIngredientPriceTests
{
    [Fact]
    public void Create_ShouldAllowZeroPrice_WhenExtraIsFree()
    {
        RestaurantIngredientPrice price = RestaurantIngredientPrice.Create(
            new RestaurantIngredientPriceId(Guid.CreateVersion7()),
            Guid.CreateVersion7(),
            CreateIngredient(),
            0m);

        Assert.Equal(0m, price.Price);
    }

    [Fact]
    public void Create_ShouldThrowDomainException_WhenPriceIsNegative()
    {
        DomainException exception = Assert.Throws<DomainException>(() =>
            RestaurantIngredientPrice.Create(
                new RestaurantIngredientPriceId(Guid.CreateVersion7()),
                Guid.CreateVersion7(),
                CreateIngredient(),
                -0.01m));

        Assert.Equal(RestaurantIngredientPriceErrors.PriceMustNotBeNegative().Message, exception.Message);
    }

    private static Ingredient CreateIngredient()
        => Ingredient.Create(
            new IngredientId(Guid.CreateVersion7()),
            Guid.CreateVersion7(),
            "Extra ingredient",
            "A valid ingredient description.",
            []);
}
