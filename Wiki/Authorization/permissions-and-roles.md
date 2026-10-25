# Permissions and roles

Permissions are application-owned identifiers. Keycloak handles identity and
token issuance; it does not define the application's permission catalog.

## Definition versus grant

| Type | Meaning | Key fields |
|---|---|---|
| `PermissionDefinition` | Declares a permission available to the application and its metadata. | `Module`, `Name`, `Description`, `IsResourceScoped` |
| `PermissionGrant` | Represents a permission effective for a resolved user through assigned roles. | `Module`, `Name`, `IsResourceScoped`, optional `RestaurantId` |

A definition is not an assignment. A grant has no description; it represents
effective access, including optional restaurant scope.

## Where permissions are defined

Each module owns permission constants and its `AvailablePermissions` list.
`PermissionCatalog` combines these lists into `PermissionDefinition`s and
marks resource-scoped permissions. Stable module and permission names are used
in authorization checks; they are not localized display text.

The catalog also identifies subsets used for built-in roles, including guest
and company-owner permissions. Role-to-permission composition is persisted in
the Identity database.

## How roles grant permissions

The `Role` aggregate contains permission links and user-role links. A user may
have multiple assigned roles. `PermissionQuery.ByUserId` joins those
assignments and creates the effective `PermissionGrant` collection used by
`ICurrentUser`.

Current built-in roles are `RestaurantAdmin`, `Guest`, `Customer`, and
`CompanyOwner`. The catalog synchronizer provisions or refreshes their
permission sets. User registration assigns `Customer` or `CompanyOwner`; guest
registration assigns `Guest`. Company-owner onboarding assigns both
`Customer` and `CompanyOwner`.

`Role` supports custom-role domain operations, but current Identity feature
tree does not contain role-management endpoints or handlers. Do not document
custom-role creation or assignment as an available UI/API workflow yet.

The in-progress company setup workflow creates a Company and its initial
Restaurant after owner registration. It does not yet model company memberships
or restaurant memberships as authorization assignments.

## Scope behavior

`Role.RestaurantId` is nullable. Null represents a global role; a value
represents a restaurant-bound role. `PermissionGrant.RestaurantId` records the
scope carried by the assigned role.

For an unscoped grant, `HasPermission` accepts a permission check without a
restaurant ID. For a scoped grant, `HasPermission` requires a resource-scoped
permission and a matching restaurant ID. This is current code behavior; the
refactor's company-membership and restaurant-membership model is documented
separately in [ADR-0004](../Users%20ADR/0004-company-restaurant-roles-and-authorization.md).

## Adding a permission

1. Add a stable permission name to the owning module's constants and
   `AvailablePermissions`.
2. If permission is restaurant-scoped, include it in that module's
   resource-scoped names.
3. Add or update backend checks at the protected feature boundary. Add UI
   visibility checks only as a companion UX rule.
4. Decide which built-in role subsets should receive it in `PermissionCatalog`.
5. Add focused tests for allowed, denied, and resource-scope cases.
6. Run the database initializer to synchronize the catalog and built-in role
   permissions.

See [catalog synchronization](permission-catalog-sync.md) for what the
initializer changes.
