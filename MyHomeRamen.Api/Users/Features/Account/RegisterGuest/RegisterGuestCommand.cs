using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Api.Users.Features.Account.RegisterGuest;

public sealed record RegisterGuestCommand(RegisterGuestRequest Request) : IRequest<RegisterGuestResponse>;
