using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Employees.Responses;

namespace MyHomeRamen.Features.Identity.Features.Users.GetEmployees;

public sealed record GetEmployeesQuery : IQuery<GetEmployeesResponse>;

