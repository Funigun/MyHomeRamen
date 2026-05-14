using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket;

public sealed record AddItemToBasketCommand(AddItemToBasketRequest AddItemToBasketRequest) : IRequest<AddItemToBasketResponse>;
