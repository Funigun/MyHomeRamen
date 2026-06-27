using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;

namespace MyHomeRamen.Api.Users.Features.Account.Register;

public sealed record RegisterCommand(RegisterRequest Request) : ICommand;
