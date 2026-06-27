using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.DeleteBasketItem;

public sealed record DeleteBasketItemCommand(BasketId BasketId, BasketItemId BasketItemId) : ICommand;
