using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Employees.Responses;
using MyHomeRamen.Features.Users.Services;
using MyHomeRamen.Features.Users.Services.Dto;

namespace MyHomeRamen.Features.Users.Features.Employees.GetEmployees;

public sealed class GetEmployeesHandler(IKeycloakAdminService adminService) : IQueryHandler<GetEmployeesQuery, GetEmployeesResponse>
{
    public async Task<GetEmployeesResponse> Handle(GetEmployeesQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<KeycloakUserDto> users = await adminService.GetEmployees(cancellationToken);

        return new GetEmployeesResponse(users.Select(u => u.ToResponse()));
    }
}

