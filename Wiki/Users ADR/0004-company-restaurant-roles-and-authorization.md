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

The Identity module currently models users and roles, but role ownership is not
yet suitable for a SaaS-style restaurant system. A user may work with multiple
companies and restaurants, while each company needs independent owners and each
restaurant needs independent administrators and employees.

The system also needs to distinguish:

- Global user identity from company and restaurant membership.
- Company-level ownership from restaurant-level administration.
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
└── Restaurant
    └── RestaurantMembership
        └── Role assignments
```

Company is the SaaS tenant boundary. Restaurants belong to one Company.

Identity stores only strongly typed references to `CompanyId` and `RestaurantId`.
Company and Restaurant aggregates remain owned by the Restaurants module.

#### 2. User and membership model

`User` remains a global identity linked to Keycloak. Employee is not modeled as
a User subtype.

Identity introduces two membership concepts:

- **CompanyMembership**: links User to Company and owns company-scoped role
  assignments.
- **RestaurantMembership**: links User to Restaurant and owns restaurant-scoped
  role assignments and employee metadata.

One User may have memberships in multiple companies and multiple restaurants.
Membership lifecycle is:

```text
Invited -> Active -> Suspended
                    -> Archived
```

Inactive memberships grant no effective permissions.

#### 3. Role scopes

Roles have explicit scope:

- `Company`
- `Restaurant`

The following immutable system roles are provisioned:

- `CompanyOwner`: company-scoped; manages company members, restaurants, and
  RestaurantAdmin assignments.
- `RestaurantAdmin`: restaurant-scoped; manages employees and custom roles for
  one restaurant.
- `Customer`: restaurant-scoped immutable role.
- `Guest`: restaurant-scoped immutable role.

Restaurant creation provisions immutable RestaurantAdmin, Customer, and Guest
roles for that restaurant. Company creation provisions the CompanyOwner role
and assigns it to the registering user.

Restaurant Admin cannot manage Company memberships or assign
RestaurantAdmin. CompanyOwner assigns RestaurantAdmin. Restaurant Admin may
manage ordinary employee memberships and custom restaurant roles.

Custom restaurant roles use the fixed system permission catalog. Users cannot
create permissions.

#### 4. Role and membership invariants

- Role names are case-insensitively unique within their company or restaurant
  scope.
- A role cannot be assigned outside its owning company or restaurant.
- System roles cannot be renamed, edited, or removed.
- Custom roles are archived instead of physically deleted once used.
- One RestaurantMembership may have multiple active roles.
- At least one active CompanyOwner must remain for every company.
- Multiple active RestaurantAdmins are allowed, but the final active
  RestaurantAdmin cannot be suspended or removed.
- One `(UserId, CompanyId)` CompanyMembership may exist.
- One `(UserId, RestaurantId)` RestaurantMembership may exist.
- Role and membership mutations use optimistic concurrency through an expected
  version or ETag.
- Invitations and provisioning commands are idempotent.

#### 5. Authorization context

Authentication remains delegated to Keycloak. Authorization is resolved by the
application from active memberships, roles, and permissions.

Restaurant requests require a `Restaurant-Id` header. The server must validate:

1. The restaurant exists and is active.
2. The restaurant belongs to the requested company context where applicable.
3. The caller has an active matching CompanyMembership or
   RestaurantMembership.
4. The caller has the required scoped permission.

Company endpoints use `{companyId}` route segments.

The application must not place all company and restaurant permissions in a
token. Tokens may contain stable identity claims, while effective permissions
are resolved server-side and cached with invalidation.

### Aggregates and ownership

#### User

Identity owns global user identity, profile data, Keycloak linkage, and account
lifecycle. User does not own company or restaurant role assignments.

#### CompanyMembership

Identity owns the User-to-Company relationship, company membership lifecycle,
and CompanyOwner assignments. Company itself remains owned by the Restaurants
module.

#### RestaurantMembership

Identity owns the User-to-Restaurant relationship, employee metadata, lifecycle,
and restaurant role assignments. Restaurant itself remains owned by the
Restaurants module.

#### Role

Identity owns role definitions, scope ownership, system/custom status,
lifecycle, and permission links.

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

Produced by the Restaurants/company workflow after company registration.
Identity consumes it to create the initial CompanyMembership and assign
CompanyOwner. Processing is retried and protected by a unique membership
constraint.

#### RestaurantRegistered

Produced by the Restaurants module after restaurant creation. Identity consumes
it to provision RestaurantAdmin, Customer, and Guest roles. Provisioning is
idempotent and exposes failure or pending status.

#### MembershipChanged

Produced by Identity after membership lifecycle or role-assignment changes.
Consumers invalidate authorization caches and update relevant read models.
Events contain a membership version so consumers can ignore older events.

#### RoleChanged

Produced by Identity after custom role permission or lifecycle changes.
Consumers invalidate authorization caches and reload permissions from the
source of truth.

### API contract baseline

#### Company registration

`POST /api/companies`

Creates a company and assigns the registering user as its first active
CompanyOwner.

- Requires authenticated User.
- Requires `Idempotency-Key`.
- Returns company ID, owner membership ID, and provisioning status.
- Emits `CompanyRegistered`.

#### Company role listing

`GET /api/identity/companies/{companyId}/roles`

Lists company-scoped system and custom roles for an authorized CompanyOwner.

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

RestaurantAdmin manages ordinary employee memberships. CompanyOwner manages
RestaurantAdmin assignments and company-level oversight. Mutations emit
`MembershipChanged`.

#### Employee role assignment

`PUT /api/identity/restaurants/{restaurantId}/employees/{userId}/roles`

Replaces active role assignments atomically. Role IDs must belong to the same
restaurant. CompanyOwner-only restrictions apply to RestaurantAdmin assignment.

#### Effective authorization

`GET /api/identity/me/access`

Returns effective roles and permissions for the authenticated user and the
restaurant identified by `Restaurant-Id`. The response includes UserId,
CompanyId, RestaurantId, membership status, role names, permission names, and
authorization version.

### Error and concurrency behavior

- `400 Bad Request`: invalid request, scope, or permission.
- `401 Unauthorized`: missing authentication.
- `403 Forbidden`: missing permission or invalid tenant context.
- `404 Not Found`: missing resource.
- `409 Conflict`: duplicate role or membership, invalid lifecycle transition,
  or final-owner/final-admin protection.
- `412 Precondition Failed`: stale version or ETag.
- `202 Accepted`: asynchronous company or restaurant role provisioning.

### Consequences

#### Positive

- **POS-001: Tenant isolation:** Company and restaurant scope are explicit in
  every membership, role, and authorization decision.
- **POS-002: Reusable identity:** One User can work across companies and
  restaurants without duplicating profile data.
- **POS-003: Least privilege:** Restaurant administrators cannot manage other
  restaurants or company ownership.
- **POS-004: Safer authorization:** Current permissions are resolved server-side
  instead of relying on a large, stale token permission set.
- **POS-005: Modular boundaries:** Identity references CompanyId and
  RestaurantId without directly depending on Restaurants domain entities.
- **POS-006: Reliable provisioning:** Integration events and idempotency support
  retries without duplicate system roles or memberships.

#### Negative

- **NEG-001: Authorization complexity:** Every protected request must establish
  and validate company or restaurant context.
- **NEG-002: Eventual consistency:** Company and restaurant role provisioning
  may be pending after the source aggregate is created.
- **NEG-003: Cache invalidation:** Role and membership changes require reliable
  authorization cache invalidation.
- **NEG-004: More aggregates:** User, CompanyMembership,
  RestaurantMembership, and Role require separate lifecycle and concurrency
  rules.

