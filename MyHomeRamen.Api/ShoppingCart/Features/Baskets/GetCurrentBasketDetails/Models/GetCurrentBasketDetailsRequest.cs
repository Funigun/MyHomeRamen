using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails.Models;

public sealed record GetCurrentBasketDetailsRequest : IRequest<GetCurrentBasketDetailsResponse?>;
