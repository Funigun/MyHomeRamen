using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Users.Features.Account.RegisterGuest.Models;

public record RegisterGuestRequest(Guid? ExistingGuestId) : IRequest<RegisterGuestResponse>;
