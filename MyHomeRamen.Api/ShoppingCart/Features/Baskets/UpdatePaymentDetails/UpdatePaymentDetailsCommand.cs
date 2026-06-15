using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.UpdatePaymentDetails;

public record UpdatePaymentDetailsCommand(BasketId BasketId, UserId UserId, UpdatePaymentDetailsRequest Request) : ICommand;
