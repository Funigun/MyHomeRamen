using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Roles.Responses;
using MyHomeRamen.Features.Users.Services;
using MyHomeRamen.Features.Users.Services.Dto;

namespace MyHomeRamen.Features.Users.Features.Roles.GetAvailableRoles;

public sealed class GetAvailableRolesHandler(IKeycloakAdminService keycloakAdminService) : IQueryHandler<GetAvailableRolesQuery, GetAvailableRolesResponse>
{
    public async Task<GetAvailableRolesResponse> Handle(GetAvailableRolesQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<KeycloakRoleDto> roles = await keycloakAdminService.GetAvailableRoles(cancellationToken);

        return roles.ToResponse();
    }
}

