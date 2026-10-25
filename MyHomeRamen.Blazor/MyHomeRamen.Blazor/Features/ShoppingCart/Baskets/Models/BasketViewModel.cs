using MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.BasketMenu;
using MyHomeRamen.Blazor.Features.ShoppingCart.Common.Services.Contracts.Baskets.Responses;

namespace MyHomeRamen.Blazor.Features.ShoppingCart.Baskets.Models;

public sealed record BasketViewModel(
    Guid Id,
    IReadOnlyList<BasketItemTableModel> Items,
    AllowedActionsModel AllowedActions)
{
    public decimal Total => Items.Sum(item => item.Price * item.Quantity);

    public static BasketViewModel FromResponse(GetCurrentBasketSummaryResponse response) =>
        new(response.Id, response.Items.Select(BasketItemTableModel.FromResponse).ToList(), AllowedActionsModel.FromResponse(response.AllowedActions));
}

public sealed record AllowedActionsModel(bool CanCreate, bool CanEdit, bool CanDelete)
{
    public static AllowedActionsModel FromResponse(AllowedActionsDto actions) =>
        new(actions.CanCreate, actions.CanEdit, actions.CanDelete);
}
