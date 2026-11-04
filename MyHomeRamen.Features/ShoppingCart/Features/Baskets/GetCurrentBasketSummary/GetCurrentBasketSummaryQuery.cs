using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Common.Tenancy;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketSummary;

public sealed record GetCurrentBasketSummaryQuery : IQuery<GetCurrentBasketSummaryResponse?>;

public sealed class GetCurrentBasketSummaryAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetCurrentBasketSummaryQuery>
{
    public async Task<bool> Authorize(GetCurrentBasketSummaryQuery request, CancellationToken cancellationToken)
    {
        return await Task.FromResult(currentUser.CanManageBasket());
    }
}

public sealed record GetCurrentBasketSummaryQueryOptions(UserId UserId, Guid? RestaurantId)
    : DbQueryOptions<Basket, CurrentBasketSummaryDto>
    (
        new()
        {
            Filter = basket => basket.UserId == UserId
                && (!RestaurantId.HasValue || basket.RestaurantId == RestaurantId)
                && basket.Status == BasketStatus.Active,
            Selector = basket => new CurrentBasketSummaryDto(
                basket.Id.Value,
                basket.RestaurantId,
                basket.Items.Select(item => new BasketSummaryItemDto(
                    item.Id.Value,
                    item.Product.Name,
                    item.Product.ImageUrl,
                    item.Quantity,
                    item.Price)))
        }
    );

public sealed class GetCurrentBasketSummaryHandler(
    IShoppingCartDbContext dbContext,
    ICurrentUser currentUser,
    ICompanyContext companyContext)
    : IRequestHandler<GetCurrentBasketSummaryQuery, GetCurrentBasketSummaryResponse?>
{
    public async Task<GetCurrentBasketSummaryResponse?> Handle(
        GetCurrentBasketSummaryQuery request,
        CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        CurrentBasketSummaryDto? basket = await dbContext.Basket.Query().GetCurrentBasketSummaryAsync(
            new GetCurrentBasketSummaryQueryOptions(userId, companyContext.RestaurantId),
            cancellationToken);

        return basket is null
            ? null
            : new GetCurrentBasketSummaryResponse(
                basket.Id,
                basket.RestaurantId,
                basket.Items,
                currentUser.BasketAllowedActions());
    }
}

internal static class Mappings
{
    public static GetCurrentBasketSummaryResponse ToResponse(this Basket basket, ICurrentUser currentUser)
        => new(
            basket.Id.Value,
            basket.RestaurantId,
            basket.Items.Select(item => item.ToDto()),
            currentUser.BasketAllowedActions());

    public static BasketSummaryItemDto ToDto(this BasketItem item)
        => new(
            item.Id.Value,
            item.Product.Name,
            item.Product.ImageUrl,
            item.Quantity,
            item.Price);
}
