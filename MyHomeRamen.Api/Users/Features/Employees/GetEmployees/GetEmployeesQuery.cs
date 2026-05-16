using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Users.Employees.Responses;

namespace MyHomeRamen.Api.Users.Features.Employees.GetEmployees;

public sealed record GetEmployeesQuery : IRequest<GetEmployeesResponse>;
