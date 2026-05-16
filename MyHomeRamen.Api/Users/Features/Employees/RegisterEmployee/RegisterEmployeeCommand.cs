using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Users.Employees.Requests;

namespace MyHomeRamen.Api.Users.Features.Employees.RegisterEmployee;

public sealed record RegisterEmployeeCommand(RegisterEmployeeRequest Request) : ICommand;
