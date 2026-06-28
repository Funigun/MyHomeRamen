using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Features.ShoppingCart.Features.Baskets.GetPaymentDetails;

public sealed record GetPaymentDetailsQuery(BasketId BasketId, UserId UserId) : IQuery<PaymentDetailsResponse>;

