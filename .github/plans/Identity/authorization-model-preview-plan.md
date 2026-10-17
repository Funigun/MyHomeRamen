# Identity authorization model: domain redesign

## Goal

Redesign the authorization domain and its persistence/runtime integrations without planning management features. Define the model and preserve required identity, permission, and system-role behavior.

## Domain changes extracted from previous draft

- Separate `CompanyMembership` access boundaries from role assignment.
- Introduce explicit `UserRole` association instead of direct `User.Roles`.
- Add role scope/type only if confirmed by the authorization requirements.
- Update dependent authorization, registration, persistence/migration, tests, and `PermissionCatalogSynchronizer`.

These are candidate domain changes, not approval of the previous draft's assumed workflows or scope rules.

## Explicitly out of scope

- Creating, listing, viewing, or removing company memberships.
- Creating or managing custom roles.
- Assigning or removing user roles, including employee role management.
- Employee-management UX/API and company/restaurant-management features.

These are separate feature plans. Domain and persistence foundations must not imply that these management operations are part of this work.

## Decisions still required

- What identifies a company and how company ownership is represented; do not assume a single company.
- Whether roles are global definitions, company-scoped records, restaurant-scoped records, or a combination.
- Whether system roles remain one synchronized record each or are provisioned per company/scope.
- What company-wide versus restaurant-specific membership means, and how it relates to current registration flows.
- Whether `Restaurant.CompanyId` is an authoritative domain relationship in this change or belongs to a separate company/restaurant feature.

See `authorization-model-backend-plan.md` for extracted candidate model shape and bounded work plan. `authorization-model-flow.md` records existing behavior and flows deferred to separate feature plans.
