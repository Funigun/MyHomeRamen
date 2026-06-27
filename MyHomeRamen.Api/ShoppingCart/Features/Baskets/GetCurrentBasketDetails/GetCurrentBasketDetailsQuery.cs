using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;

public sealed record GetCurrentBasketDetailsQuery : IQuery<GetCurrentBasketDetailsResponse?>;
