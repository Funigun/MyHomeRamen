using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Employees.Responses;

namespace MyHomeRamen.Api.Users.Features.Employees.GetEmployees;

public sealed record GetEmployeesQuery : IQuery<GetEmployeesResponse>;
