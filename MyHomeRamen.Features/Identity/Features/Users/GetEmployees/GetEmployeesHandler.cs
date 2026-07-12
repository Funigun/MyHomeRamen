using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Employees.Responses;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.Features.Identity.Services.Dto;

namespace MyHomeRamen.Features.Identity.Features.Users.GetEmployees;

public sealed class GetEmployeesHandler(IKeycloakAdminService adminService) : IQueryHandler<GetEmployeesQuery, GetEmployeesResponse>
{
    public async Task<GetEmployeesResponse> Handle(GetEmployeesQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<KeycloakUserDto> users = await adminService.GetEmployees(cancellationToken);

        return new GetEmployeesResponse(users.Select(u => u.ToResponse()));
    }
}

