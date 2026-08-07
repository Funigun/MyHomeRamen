using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketSummary;

public sealed record GetCurrentBasketSummaryQuery : IQuery<GetCurrentBasketSummaryResponse>;

public sealed record GetCurrentBasketSummaryQueryOptions(UserId UserId)
    : DbQueryOptions<Basket, CurrentBasketSummaryDto>
    (
        new()
        {
            Filter = basket => basket.User.Id == UserId && basket.Status == BasketStatus.Active,
            Selector = basket => new CurrentBasketSummaryDto(
                basket.Id.Value,
                basket.Items.Select(item => new BasketSummaryItemDto(
                    item.Id.Value,
                    item.Product.Name,
                    item.Product.ImageUrl,
                    item.Quantity,
                    item.Price)))
        }
    );

public sealed class GetCurrentBasketSummaryHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
                  : IQueryHandler<GetCurrentBasketSummaryQuery, GetCurrentBasketSummaryResponse>
{
    public async Task<GetCurrentBasketSummaryResponse> Handle(GetCurrentBasketSummaryQuery request, CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        User? user = await dbContext.User.Query().FindByIdAsync(userId, cancellationToken);

        if (user is null ||
             (user.IsGuest && !string.IsNullOrEmpty(currentUser.Id))
              || (!user.IsGuest && string.IsNullOrEmpty(currentUser.Id))
            )
        {
            throw new UnauthorizedAccessException("User is not authorized to access the current basket summary.");
        }

        CurrentBasketSummaryDto? basket = await dbContext.Basket.Query().GetCurrentBasketSummaryAsync(new GetCurrentBasketSummaryQueryOptions(userId), cancellationToken);

        return basket is null
            ? throw new InvalidOperationException("Basket was not found.")
            : new GetCurrentBasketSummaryResponse(basket.Id, basket.Items);
    }
}

