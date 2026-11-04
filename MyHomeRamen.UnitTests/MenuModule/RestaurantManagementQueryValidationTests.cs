using MyHomeRamen.Domain.Menu.Offers;
using MyHomeRamen.Features.Common.Endpoints.Models;
using MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardsForManage;
using MyHomeRamen.Features.Menu.Features.Offers.GetRestaurantOffersForManage;
using MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.GetRestaurantIngredientPricesForManage;

namespace MyHomeRamen.UnitTests.MenuModule;

public sealed class RestaurantManagementQueryValidationTests
{
    [Fact]
    public void GetIngredientPricesForManage_ShouldRejectInvalidPageAndFilters()
    {
        GetRestaurantIngredientPricesForManageValidator validator = new();
        GetRestaurantIngredientPricesForManageQuery query = new(
            new PageParameters(0, 101),
            new GetRestaurantIngredientPricesForManageRequest(
                new string('x', 201),
                "unknown",
                "up"));

        FluentValidation.Results.ValidationResult result = validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Equal(5, result.Errors.Count);
    }

    [Fact]
    public void GetOffersForManage_ShouldRejectInvalidPageStatusAndFilters()
    {
        GetRestaurantOffersForManageValidator validator = new();
        GetRestaurantOffersForManageQuery query = new(
            new PageParameters(1, 101),
            new GetRestaurantOffersForManageRequest(
                Guid.Empty,
                new string('x', 201),
                (RestaurantProductOfferStatus)99,
                "unknown",
                "up"));

        FluentValidation.Results.ValidationResult result = validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Equal(6, result.Errors.Count);
    }

    [Fact]
    public void GetMenuCardsForManage_ShouldRejectInvalidPageAndFilters()
    {
        GetMenuCardsForManageValidator validator = new();
        GetMenuCardsForManageQuery query = new(
            new PageParameters(0, 10),
            new GetMenuCardsForManageRequest(
                new string('x', 201),
                null,
                "unknown",
                "up"));

        FluentValidation.Results.ValidationResult result = validator.Validate(query);

        Assert.False(result.IsValid);
        Assert.Equal(4, result.Errors.Count);
    }
}
