using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Employees.Responses;

namespace MyHomeRamen.Features.Users.Features.Employees.GetEmployees;

public sealed record GetEmployeesQuery : IQuery<GetEmployeesResponse>;

