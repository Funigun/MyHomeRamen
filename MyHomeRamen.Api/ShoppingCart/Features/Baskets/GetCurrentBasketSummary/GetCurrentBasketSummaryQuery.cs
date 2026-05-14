using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketSummary;

public sealed record GetCurrentBasketSummaryQuery : IRequest<GetCurrentBasketSummaryResponse>;
