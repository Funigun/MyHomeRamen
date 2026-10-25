# Authorization

This section documents current authorization implementation while Identity and
role management are being refactored.

Authentication proves identity. My Home Ramen API resolves application users,
roles, and permission grants from its own database and makes authorization
decisions server-side. Keycloak roles and OIDC scopes are not the permission
catalog.

## Pages

- [Authentication and Keycloak flows](authentication-flow.md)
- [Backend authorization](backend-authorization.md)
- [Permissions and roles](permissions-and-roles.md)
- [Permission catalog synchronization](permission-catalog-sync.md)

## Current implementation and design

These pages describe code currently in the repository. The accepted
[company and restaurant authorization ADR](../Users%20ADR/0004-company-restaurant-roles-and-authorization.md)
describes a broader membership-based design. Treat details not present in
current code as design direction, not implemented behavior. ADR-0004 supersedes
the older [application scopes ADR](../Users%20ADR/0003-application-scopes.md).

The existing [Keycloak setup guide](../Getting%20Started/0003-keycloak-configuration.md)
contains local setup steps and configuration caveats.

