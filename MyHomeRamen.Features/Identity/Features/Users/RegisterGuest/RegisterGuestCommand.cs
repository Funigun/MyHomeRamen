using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Common.Contracts.Users.Account.Responses;

namespace MyHomeRamen.Features.Identity.Features.Users.RegisterGuest;

public sealed record RegisterGuestCommand(RegisterGuestRequest Request) : ICommand<RegisterGuestResponse>;

