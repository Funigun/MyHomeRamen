# Plan: Identity authorization domain redesign

## Purpose and boundaries

Redesign authorization domain foundations and only the directly dependent persistence/runtime behavior. This plan does **not** implement membership management, role management, or employee user-role management. Those features require separate plans and must not be inferred from the domain model.

## Current model facts

- `CompanyMembership` currently stores `UserId`, required `CompanyId` and `RestaurantId`, `RoleId`, `IsActive`, and `IdempotencyKey`; owner and restaurant-admin factories create different variants.
- `Role` currently stores a global name/description and permission links. It has built-in factories for Restaurant Admin, Guest, Customer, and Company Owner, plus custom-role factories.
- `User` currently stores a `Role` string and a many-to-many `Roles` relationship.
- `PermissionCatalogSynchronizer` synchronizes the permission catalog and creates/updates four system roles: Restaurant Admin, Guest, Customer, and Company Owner. Role synchronization is existing required behavior, not a role-management feature.

## Target scope

### Candidate domain shape extracted from the previous draft

Treat these as a proposal to validate against requirements, not settled feature behavior:

- **CompanyMembership:** `UserId`, `CompanyId`, and possibly nullable `RestaurantId`; no role assignment in the membership itself. The earlier proposal also removed `IsActive` and `IdempotencyKey`; defer that removal until their existing behavior and data requirements are checked.
- **Role:** retain permission links; earlier proposal added `CompanyId`, optional `RestaurantId`, and `Public`/`Staff` kind. Confirm scope and kind semantics before committing to fields, indexes, or role factories.
- **UserRole:** explicit `UserId`/`RoleId` association, potentially with optional restaurant scope. Confirm assignment scope and uniqueness before defining invariants.
- **User:** remove the `Roles` navigation and `AddRole` only if `UserRole` replaces their current uses; separately trace the legacy `Role` string.
- **Restaurant:** earlier proposal added `CompanyId`. Treat company ownership as a dependency requiring confirmation, not as a one-company shortcut.

### Domain

1. **CompanyMembership**
   - Separate membership/access boundary from role assignment.
   - Decide and document membership scope before fixing its shape. A nullable `RestaurantId` is one possible representation, not an assumed decision.
   - Remove membership-owned role assignment only if the agreed model makes role assignment independent.
   - Do not add CRUD/lifecycle endpoints or imply that users can manage memberships in this change.
   - Resolve the purpose of `IsActive` and `IdempotencyKey` before removing either; preserve behavior until dependent registration flows and stored data are understood.

2. **Role**
   - Decide role identity and scope (global/company/restaurant) before changing role fields, uniqueness, or factories.
   - Keep permission membership explicit through `RolePermission`.
   - Preserve distinction between synchronized system roles and future custom-role management. Custom-role endpoints and workflows are out of scope.

3. **UserRole**
   - Decide whether an explicit association entity replaces `User.Roles`, and define assignment scope and uniqueness only after role scope is decided.
   - Treat assignment-management operations as out of scope; only model/persistence/runtime foundations may be included here.

4. **User**
   - Trace all uses of `User.Role`, `User.Roles`, and `AddRole` before changing/removing them.
   - Keep authentication identity data separate from application authorization unless an existing caller requires compatibility during migration.

5. **Validation and errors**
   - Add only invariants implied by the agreed domain decisions.
   - Keep cross-module existence/company checks in an appropriate application/persistence boundary; do not make Identity directly depend on Restaurants.

### Required dependent behavior

- **Permission catalog and system-role synchronization:** update `PermissionCatalogSynchronizer` to match the agreed role identity/scope model. Preserve its current responsibility to add/remove catalog permissions and create/update the four known system roles. Determine how role lookups and synchronization behave across company/scope; do not silently switch to per-company role creation.
- **Registration and authorization:** identify and update existing registration flows and permission evaluation that depend on `User.Roles`, membership role fields, or global role names. Keep behavior explicit while applying only decisions approved for this redesign.
- **Persistence and migration:** update Identity configurations, relationship mappings, indexes/converters, and migration/backfill only after target model decisions. Do not use a single-company backfill assumption. Document any data that cannot be preserved on downgrade.
- **Tests:** add focused domain tests for agreed invariants and persistence/integration coverage for migration, system-role synchronization, registration, and authorization behavior affected by the redesign.

## Deferred feature plans

Plan these separately; this document does not specify their endpoints, permissions, request/response contracts, or workflows:

- Company-membership create/list/view/remove.
- Role create/update/delete/list and custom-role policy.
- User-role assignment/removal/list, including employee management.
- Company and restaurant administration, including authoritative restaurant-to-company ownership.

## Work sequence

1. Resolve the domain decisions below and record target invariants.
2. Trace current callers/data dependencies for `CompanyMembership`, `Role`, `User.Role`, and `User.Roles`.
3. Specify domain shape, validators, and error behavior without adding management features.
4. Specify persistence and migration/backfill against actual data; do not assume one company or discard fields without a compatibility decision.
5. Update system-role synchronization, registration, and authorization integrations to preserve agreed behavior.
6. Add focused tests and verify existing callers no longer depend on removed model behavior.

## Open decisions (blocking model details)

- Company identity source, multiplicity, and ownership relationship.
- Role scope and whether system role records are global or replicated per company/scope.
- Membership scope semantics, including company-wide access and restaurant coverage.
- How existing registrations create memberships/role assignments, and whether current idempotency behavior remains required.
- Fate of `IsActive`, `IdempotencyKey`, `User.Role`, and legacy role data.
- Whether Restaurant-to-Company ownership is part of this redesign or a separate feature.

Do not convert any open decision into a requirement by implementation convenience. Management-feature behavior belongs in its own plan.
