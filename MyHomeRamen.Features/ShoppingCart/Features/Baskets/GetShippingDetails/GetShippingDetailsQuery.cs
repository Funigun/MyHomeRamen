using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetShippingDetails;

public sealed record GetShippingDetailsQuery(BasketId BasketId, UserId UserId) : IQuery<ShippingDetailsResponse>;

