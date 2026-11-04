# Backend authorization

The API has two distinct checks:

1. **Authentication:** Is the request's bearer token valid and its principal
   authenticated?
2. **Authorization:** Does the resolved application user have permission for
   this operation and resource?

```mermaid
flowchart TD
    A[HTTP request] --> B[JWT bearer validation]
    B -->|invalid or missing| C[401 Unauthorized]
    B -->|valid| D[Resolve Keycloak subject to application User]
    D --> E[Load roles and permissions from Identity database]
    E --> F[Build request-scoped ICurrentUser]
    F --> G{Endpoint requires authentication?}
    G -->|no| H[Continue endpoint]
    G -->|yes, anonymous| I[401 Unauthorized]
    G -->|yes, authenticated| J{Feature permission check passes?}
    J -->|no| K[403 Forbidden]
    J -->|yes| H
```

## Request identity and permission loading

The API validates access tokens using configured authority, audience, and
issuer. `UserLoginMiddleware` calls `AuthorizationService` for each request.
That service resolves a domain user by `domain_id`, Keycloak ID, or guest ID,
then loads the user's effective `PermissionGrant`s from assigned roles.

`PermissionQuery.ByUserId` joins user-role assignments, role-permission
assignments, roles, and permissions. The resulting grants carry explicit
`RoleScope` and optional `RestaurantId`; permissions are therefore resolved
from current database state, not copied wholesale into Keycloak tokens.

Guests are represented by application users too. For unauthenticated requests,
Blazor may forward the `guest_id` cookie to the API; the API uses it to resolve
the guest user and assigned guest permissions.

## Enforcement points

- Endpoint `.RequireAuthorization(...)` metadata requires an authenticated
  principal. The configured `AuthenticatedUser` policy uses the API's JWT
  bearer scheme.
- Feature handlers can have a request-specific `IAuthorizationPolicy<TRequest>`.
  `AddHandlers` discovers these policies and wraps matching handlers in
  `AuthorizationHandler<TRequest,TResponse>`.
- Feature logic can use `ICurrentUser.HasPermission`,
  `HasAnyPermission`, and `RestaurantIdsFor` for permission and scope checks.
- Blazor derives admin-navigation claims from the API's `GetMe` response and
  uses UI authorization policies to show or hide sections. These checks improve
  UX; API authorization remains mandatory.

An unauthenticated request is normally rejected with 401. An authenticated
user who lacks the required permission should receive 403. Specific feature
endpoints may have additional behavior.

## Company and Restaurant scope

A Company-scoped grant applies across the Company's restaurants. A
Restaurant-scoped grant applies only when permission is resource-scoped and
requested restaurant ID matches the role's `RestaurantId`. Use
`HasPermission(module, permission, restaurantId)` for a specific resource.

`HasAnyPermission` only answers whether any matching grant exists; it does not
prove access to a particular restaurant. Use `RestaurantIdsFor` to get the
restaurant IDs attached to Restaurant-scoped grants.

`CompanyContextMiddleware` validates `X-Restaurant-Id` for menu, payment, and
restaurant-scoped shopping-cart operations, then populates request-scoped
`CompanyContext`. Menu-card options and current-basket discovery do not require
the header. Basket add/checkout body restaurant IDs must match the header;
basket-ID operations must match the basket's restaurant.

The accepted [ADR-0004](../Users%20ADR/0004-company-restaurant-roles-and-authorization.md)
also defines authorization-cache invalidation behavior; verify that separately
before adding role-management workflows.
