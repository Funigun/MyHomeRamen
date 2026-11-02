using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;
using MyHomeRamen.Features.Identity.Permissions;
using MyHomeRamen.Features.Menu.ExternalApi;

namespace MyHomeRamen.Features.Menu.Features.Offers.GetOfferCustomization;

public sealed record OfferIngredientDto(Guid Id, string Name, decimal Price);

public sealed record GetOfferCustomizationQuery(Guid Id) : IQuery<GetOfferCustomizationResponse?>;

public sealed class GetOfferCustomizationAuthorizationPolicy(
    ICurrentUser currentUser,
    ICompanyContext companyContext)
    : IAuthorizationPolicy<GetOfferCustomizationQuery>
{
    public Task<bool> Authorize(
        GetOfferCustomizationQuery request,
        CancellationToken cancellationToken)
        => Task.FromResult(
            companyContext.RestaurantId.HasValue
            && currentUser.HasPermission(
                "Menu",
                MenuPermissionConstants.CanViewOffer,
                companyContext.RestaurantId));
}

public sealed class GetOfferCustomizationHandler(
    IMenuService menuService,
    ICompanyContext companyContext)
    : IRequestHandler<GetOfferCustomizationQuery, GetOfferCustomizationResponse?>
{
    public async Task<GetOfferCustomizationResponse?> Handle(
        GetOfferCustomizationQuery query,
        CancellationToken cancellationToken)
    {
        Guid? restaurantId = companyContext.RestaurantId;
        if (!restaurantId.HasValue)
        {
            return null;
        }

        MenuOfferResult? offer = await menuService.GetOfferAsync(
            restaurantId.Value,
            query.Id,
            cancellationToken);
        if (offer is null)
        {
            return null;
        }

        return new GetOfferCustomizationResponse(
            offer.OfferId,
            offer.ProductId,
            offer.Name,
            offer.Description,
            offer.Price,
            offer.ImageUrl,
            offer.BaseIngredients.Select(ingredient =>
                new OfferIngredientDto(ingredient.Id, ingredient.Name, ingredient.Price)),
            offer.AvailableExtras.Select(ingredient =>
                new OfferIngredientDto(ingredient.Id, ingredient.Name, ingredient.Price)));
    }
}
