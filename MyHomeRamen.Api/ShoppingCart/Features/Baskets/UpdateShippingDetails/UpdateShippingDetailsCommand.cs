using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Users;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.UpdateShippingDetails;

public record UpdateShippingDetailsCommand(BasketId BasketId, UserId UserId, UpdateShippingDetailsRequest Request) : ICommand;
