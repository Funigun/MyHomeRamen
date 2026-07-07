using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;

namespace MyHomeRamen.Features.Identity.Features.Users.Register;

public sealed record RegisterCommand(RegisterRequest Request) : ICommand;

