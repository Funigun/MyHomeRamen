---
title: "ADR-0004: Company and Restaurant Scoped Roles and Authorization"
status: "Accepted"
date: "2026-09-11"
authors: "Funigun"
tags: ["domain", "identity", "authorization", "roles", "multi-tenancy", "architecture"]
---

### Status

**Accepted**

### Context

The Identity module must support Company-wide and Restaurant-specific access.
Each database serves one Company with multiple Restaurants. This application
does not support cross-company tenancy or a Company selector; Company context
is the Company served by the current database.

The system also needs to distinguish:

- Global user identity from Company and Restaurant-scoped role assignments.
- Company-level ownership from restaurant-level administration.
- Company-wide Guest and Customer capabilities from restaurant administration.
- Immutable system roles from restaurant-defined operational roles.
- Authentication claims from current, tenant-scoped authorization.

The design must preserve modular boundaries. Identity may reference
`CompanyId` and `RestaurantId`, but must not directly reference Company or
Restaurant domain entities owned by the Restaurants module.

This decision clarifies and supersedes conflicting assumptions in
`0003-application-scopes.md` about global `Admin`, `Employee`, and
`Customer` roles and using token scopes as the primary authorization source.

### Decision

#### 1. Tenant hierarchy

The authorization hierarchy is:

```text
Company
├── Company-scoped role assignments
└── Restaurant
    └── Restaurant-scoped role assignments
```

Each database serves exactly one Company, which is the tenant boundary for that
deployment. Restaurants belong to that Company. Company context comes from the
database/deployment, not from a Company selected by the user. Do not support
cross-company access within a database.

Identity stores only strongly typed references to `CompanyId` and `RestaurantId`.
Company and Restaurant aggregates remain owned by the Restaurants module.

#### 2. User and role-assignment model

`User` remains a global identity linked to Keycloak. Employee is not modeled as
a User subtype. `UserRole` links a User to an assigned Role; authorization uses
that assignment together with the role scope and validated request context.

Each database serves one Company, so Company-scoped role assignments apply to
that Company. Restaurant-scoped assignments are evaluated only for the
validated Restaurant context. Do not add cross-company selection.

#### 3. Role scopes

Roles have explicit scope:

- `Company`
- `Restaurant`

The following immutable system roles are provisioned:

- `CompanyOwner`: company-scoped; manages company members, restaurants, and
  RestaurantAdmin assignments.
- `RestaurantAdmin`: restaurant-scoped; manages employees and custom roles for
  one restaurant.
- `Customer`: company-scoped immutable role with Guest capabilities plus
  authenticated customer features, such as address management and future order
  history.
- `Guest`: company-scoped immutable role whose capabilities apply across all
  restaurants in that Company.

The CompanyOwner role is assigned to the registering user as part of Company
and first-Restaurant setup. Provision Company-scoped Guest and Customer roles
once for the Company, not once per Restaurant; registered guest accounts are
assigned the Guest role. Restaurant creation provisions a RestaurantAdmin role
for that Restaurant. Do not create restaurant-specific copies of Guest or
Customer.

Restaurant Admin cannot manage Company-level role assignments or assign
RestaurantAdmin. CompanyOwner assigns RestaurantAdmin. Restaurant Admin may
manage ordinary employee role assignments and custom restaurant roles.

Custom restaurant roles use the fixed system permission catalog. Users cannot
create permissions. Customer authorization includes Guest capabilities; do not
duplicate Guest behavior in a separate restaurant-scoped role.

#### 4. Role invariants

- Role names are case-insensitively unique within their company or restaurant
  scope.
- A role has an explicit `Company` or `Restaurant` scope. A Company-scoped role
  belongs to the sole Company served by the database; a Restaurant-scoped role
  belongs to one Restaurant in that Company. Nullable `RestaurantId` alone is
  not the definition of Company ownership.
- A role assignment must match the role's scope and validated Company or
  Restaurant context.
- System roles cannot be renamed, edited, or removed.
- Custom roles are archived instead of physically deleted once used.
- At least one user must retain the CompanyOwner role for the Company served by
  the database.
- Multiple RestaurantAdmins are allowed, but the final RestaurantAdmin
  assignment cannot be removed.
- One `(UserId, RoleId)` role assignment may exist.
- Role assignment mutations use optimistic concurrency through an expected
  version or ETag.
- Invitations and provisioning commands are idempotent.

#### 5. Authorization context

Authentication remains delegated to Keycloak. Authorization is resolved by the
application from UserRole assignments, role scope, and permissions.

Restaurant requests require a `Restaurant-Id` header. The server must validate:

1. The restaurant exists and is active.
2. The restaurant belongs to the sole Company served by the database.
3. The caller has a UserRole assignment for a role valid in the request's
   Company or Restaurant context.
4. The caller has the required scoped permission.

Company endpoints operate on the Company served by the database. If a route
contains `{companyId}`, validate that it identifies only that Company; never
use a caller-supplied CompanyId to select another tenant. The UI does not offer
a Company selector. Features outside the Restaurants module use server-side
`CompanyContext` for Company-scoped operations.

The application must not place all company and restaurant permissions in a
token. Tokens may contain stable identity claims, while effective permissions
are resolved server-side and cached with invalidation.

### Aggregates and ownership

#### User

Identity owns global user identity, profile data, Keycloak linkage, and account
lifecycle. User does not own company or restaurant role assignments.

#### Role

Identity owns role definitions, scope ownership, system/custom status,
lifecycle, and permission links.

#### UserRole

Identity owns User-to-Role assignments. Company and Restaurant aggregates
remain owned by the Restaurants module; request context and role scope govern
where each assignment is effective.

### Permission catalog

The initial fixed catalog is:

- `Company.View`
- `Company.Update`
- `Company.Member.View`
- `Company.Member.Invite`
- `Company.Owner.Assign`
- `Restaurant.View`
- `Restaurant.Update`
- `Restaurant.Member.View`
- `Restaurant.Member.Invite`
- `Restaurant.Member.Update`
- `Restaurant.Member.Archive`
- `Restaurant.Role.View`
- `Restaurant.Role.Create`
- `Restaurant.Role.Update`
- `Restaurant.Role.Archive`
- `Restaurant.Role.Assign`
- `UserProfile.View`
- `UserProfile.Update`

Roles compose permissions. Restaurant users cannot define new permissions.

### Integration events

#### CompanyRegistered

Produced by the Company and first-Restaurant registration workflow. Identity
consumes it to assign CompanyOwner to the registering user and provision the
Company's Guest and Customer roles. Processing is retried and protected by
unique role-assignment and role constraints.

#### RestaurantRegistered

Produced by the Restaurants module after restaurant creation. Identity consumes
it to provision that Restaurant's RestaurantAdmin role. Provisioning is
idempotent and exposes failure or pending status. It does not create
Restaurant-specific Guest or Customer roles.

#### UserRoleChanged

Produced by Identity after role-assignment changes.
Consumers invalidate authorization caches and update relevant read models.
Events contain an assignment version so consumers can ignore older events.

#### RoleChanged

Produced by Identity after custom role permission or lifecycle changes.
Consumers invalidate authorization caches and reload permissions from the
source of truth.

### API contract baseline

#### Company registration

`POST /api/companies`

Runs as part of initial Company and first-Restaurant setup, creating the
Company and assigning the registering user the CompanyOwner role. The
database/deployment is associated with this Company; the request does not
select among Companies.

- Requires authenticated User.
- Requires `Idempotency-Key`.
- Returns company ID, first restaurant ID, owner role-assignment ID, and provisioning
  status.
- Emits `CompanyRegistered`.

#### Company role listing

`GET /api/identity/companies/{companyId}/roles`

Lists Company-scoped system and custom roles for an authorized CompanyOwner.
`companyId` must identify the sole Company served by the database.

#### Restaurant role management

```text
GET    /api/identity/restaurants/{restaurantId}/roles
POST   /api/identity/restaurants/{restaurantId}/roles
PATCH  /api/identity/restaurants/{restaurantId}/roles/{roleId}
DELETE /api/identity/restaurants/{restaurantId}/roles/{roleId}
```

RestaurantAdmin manages custom restaurant roles. System roles are returned as
read-only. Mutations require expected version or `If-Match` and emit
`RoleChanged`.

#### Restaurant employee management

```text
GET   /api/identity/restaurants/{restaurantId}/employees
POST  /api/identity/restaurants/{restaurantId}/employees
GET   /api/identity/restaurants/{restaurantId}/employees/{userId}
PATCH /api/identity/restaurants/{restaurantId}/employees/{userId}
```

RestaurantAdmin manages ordinary employee role assignments. CompanyOwner
manages RestaurantAdmin assignments and company-level oversight. Mutations
emit `UserRoleChanged`.

#### Employee role assignment

`PUT /api/identity/restaurants/{restaurantId}/employees/{userId}/roles`

Replaces role assignments atomically. Role IDs must have Restaurant scope for
the route restaurant. CompanyOwner-only restrictions apply to RestaurantAdmin
assignment.

#### Effective authorization

`GET /api/identity/me/access`

Returns effective Company roles and permissions, plus Restaurant roles and
permissions when `Restaurant-Id` is supplied. The response includes UserId,
CompanyId, optional RestaurantId, role names, permission names, and
authorization version.

### Error and concurrency behavior

- `400 Bad Request`: invalid request, scope, or permission.
- `401 Unauthorized`: missing authentication.
- `403 Forbidden`: missing permission or invalid tenant context.
- `404 Not Found`: missing resource.
- `409 Conflict`: duplicate role or role assignment, or final-owner/final-admin
  protection.
- `412 Precondition Failed`: stale version or ETag.
- `202 Accepted`: asynchronous company or restaurant role provisioning.

### Consequences

#### Positive

- **POS-001: Tenant isolation:** Company and restaurant scope are explicit in
  every role-assignment and authorization decision, while each database
  serves one Company.
- **POS-002: Reusable identity:** One User can work across restaurants in the
  Company's database without duplicating profile data.
- **POS-003: Least privilege:** Restaurant administrators cannot manage other
  restaurants or company ownership.
- **POS-004: Safer authorization:** Current permissions are resolved server-side
  instead of relying on a large, stale token permission set.
- **POS-005: Modular boundaries:** Identity references CompanyId and
  RestaurantId without directly depending on Restaurants domain entities.
- **POS-006: Reliable provisioning:** Integration events and idempotency support
  retries without duplicate system roles or role assignments.

#### Negative

- **NEG-001: Authorization complexity:** Every protected request must establish
  and validate company or restaurant context.
- **NEG-002: Eventual consistency:** Company and restaurant role provisioning
  may be pending after the source aggregate is created.
- **NEG-003: Cache invalidation:** Role and role-assignment changes require reliable
  authorization cache invalidation.
- **NEG-004: Assignment consistency:** Role assignments and permission changes
  require concurrency checks and reliable authorization cache invalidation.
