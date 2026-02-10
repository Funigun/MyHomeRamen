using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Identity.Api.Features.Admin.GetEmployees.Models;

namespace MyHomeRamen.Identity.Api.Features.Admin.GetEmployees;

public sealed class GetEmployeesAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetEmployeesResponse>
{
    public Task<bool> IsAuthorized(GetEmployeesResponse request)
    {
        throw new NotImplementedException();
    }
}
