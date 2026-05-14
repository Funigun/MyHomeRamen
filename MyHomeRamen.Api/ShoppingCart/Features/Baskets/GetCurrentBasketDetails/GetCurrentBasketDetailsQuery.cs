using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;

public sealed record GetCurrentBasketDetailsQuery : IRequest<GetCurrentBasketDetailsResponse?>;
