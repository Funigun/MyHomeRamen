using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetCurrentBasketSummary;

public sealed record GetCurrentBasketSummaryQuery : IQuery<GetCurrentBasketSummaryResponse>;

