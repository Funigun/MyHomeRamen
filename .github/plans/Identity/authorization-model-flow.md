# Identity authorization model: flow boundaries

This file records existing behavior that the domain redesign must account for. It does not define new CRUD, role-assignment, employee-management, or company-management workflows.

## Existing permission catalog and system-role synchronization

`PermissionCatalogSynchronizer` currently:

1. Impersonates the system account.
2. Adds missing catalog permissions and removes obsolete permissions owned by catalog modules.
3. Creates or updates the Restaurant Admin, Guest, Customer, and Company Owner roles with the corresponding permission sets.
4. Saves permission and role changes.

This synchronization remains in scope for the domain redesign. Its role lookup/provisioning strategy must be revised only after deciding whether system roles are global or scoped to companies/restaurants. Do not infer role CRUD from this synchronization behavior.

## Flows requiring analysis during redesign

- Registration currently creates users using role-based APIs; trace these paths before replacing `User.Role` or `User.Roles`.
- Authorization currently consumes role/permission relationships; trace current checks before changing membership or role scope.
- Existing membership creation paths encode owner/admin variants; determine their required domain behavior before changing the membership shape.

## Deferred flows

No target flow is specified here for:

- Creating, listing, viewing, or removing memberships.
- Creating or managing roles.
- Assigning, listing, or removing user roles.
- Employee, company, or restaurant management.

Those flows require separate feature plans after their requirements are gathered.
