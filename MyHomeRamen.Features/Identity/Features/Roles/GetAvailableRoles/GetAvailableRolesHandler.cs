using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Users.Roles.Responses;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.Features.Identity.Services.Dto;

namespace MyHomeRamen.Features.Identity.Features.Roles.GetAvailableRoles;

public sealed class GetAvailableRolesHandler(IKeycloakAdminService keycloakAdminService) : IQueryHandler<GetAvailableRolesQuery, GetAvailableRolesResponse>
{
    public async Task<GetAvailableRolesResponse> Handle(GetAvailableRolesQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<KeycloakRoleDto> roles = await keycloakAdminService.GetAvailableRoles(cancellationToken);

        return roles.ToResponse();
    }
}

