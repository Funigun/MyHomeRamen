using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Users.Employees.Requests;

namespace MyHomeRamen.Features.Identity.Features.Users.RegisterEmployee;

public sealed record RegisterEmployeeCommand(RegisterEmployeeRequest Request) : ICommand;

