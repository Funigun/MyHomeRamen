using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Users.Roles.Responses;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Infrastructure.Keycloak.Dto;

namespace MyHomeRamen.Api.Users.Features.Roles.GetAvailableRoles;

public sealed class GetAvailableRolesHandler(IKeycloakAdminService keycloakAdminService) : IRequestHandler<GetAvailableRolesQuery, GetAvailableRolesResponse>
{
    public async Task<GetAvailableRolesResponse> Handle(GetAvailableRolesQuery query, CancellationToken cancellationToken)
    {
        IEnumerable<KeycloakRoleDto> roles = await keycloakAdminService.GetAvailableRoles(cancellationToken);

        return roles.ToResponse();
    }
}
