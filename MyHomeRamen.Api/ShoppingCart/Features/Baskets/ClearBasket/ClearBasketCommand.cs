using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.ClearBasket;

public sealed record ClearBasketCommand(BasketId BasketId, UserId UserId) : ICommand;
