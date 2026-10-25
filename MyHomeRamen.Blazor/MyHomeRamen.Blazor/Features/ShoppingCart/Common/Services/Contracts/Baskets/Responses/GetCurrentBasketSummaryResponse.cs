using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.DTOs;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.Responses;

public sealed record GetCurrentBasketSummaryResponse(
    Guid Id,
    IEnumerable<BasketSummaryItemDto> Items,
    AllowedActionsDto AllowedActions);

public sealed record AllowedActionsDto(bool CanCreate, bool CanEdit, bool CanDelete);
