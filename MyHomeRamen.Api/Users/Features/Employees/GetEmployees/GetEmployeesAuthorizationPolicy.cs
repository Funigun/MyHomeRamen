using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Users.Features.Employees.GetEmployees.Models;

namespace MyHomeRamen.Api.Users.Features.Employees.GetEmployees;

public sealed class GetEmployeesAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetEmployeesResponse>
{
    public async Task<bool> IsAuthorized(GetEmployeesResponse request)
    {
        return true;
    }
}
