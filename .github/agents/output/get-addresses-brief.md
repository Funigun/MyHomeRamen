# Feature Brief — GetAddresses

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Users` |
| **Accessibility** | `Customer · Employee · Admin` (any authenticated user) |
| **Feature name** | `GetAddresses` |
| **Short backend description** | New `GET /users/me/addresses` endpoint in the Identity.Api that returns all addresses associated with the authenticated user. No Keycloak involvement — purely a database read from `IUsersDbContext`. |
| **Short frontend description** | Not in scope for this iteration. |
| **Reference feature** | `Register` (Identity.Api — `Features/Account/Register`) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | no |

---

## 3) Feature description (Backend scope)

### New API endpoint

- **Endpoint**: `GET /users/me/addresses`
- **Group**: `Account`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: any authenticated role (`Customer`, `Employee`, `Admin`)
- **Request**: no body — user identity extracted from JWT claims
- **Response**: `200 OK` with a list of address DTOs (`id`, `street`, `building`, `apartment`, `city`, `zipCode`, `isDefault`)
- **Reference**: `RegisterEndpoint`

### Domain changes

- No domain entity changes required — `User` already has an `Addresses` navigation property and `Address` entity exists with all needed fields

### Persistence changes

- No migration required — `Addresses` table already exists in the `identity` schema
- Query loads `User` with `.Include(u => u.Addresses)` or queries `Addresses` filtered by `UserId`

### Contracts

- New response model: `AddressResponse` record in the feature folder
- Reference: `RegisterRequest` models pattern

---

## 4) Feature description (Frontend scope)

Not in scope for this iteration.

---

## 5) Testing Requirements

### Unit tests

**In scope.** Validate the handler returns the correct mapped addresses.

Tests to create:
- `GetAddressesHandler_ShouldReturnAddresses_ForAuthenticatedUser`
- `GetAddressesHandler_ShouldReturnEmptyList_WhenNoAddresses`

Reference: `MyHomeRamen.UnitTests` — existing handler tests pattern

---

### Integration tests

**In scope.** Verify the HTTP endpoint, auth enforcement, and correct data retrieval.

Tests to create:
- `GetAddresses_ShouldReturn200_WithAddressList`
- `GetAddresses_ShouldReturn401_WhenUnauthenticated`
- `GetAddresses_ShouldReturnEmptyList_WhenUserHasNoAddresses`

Reference: `MyHomeRamen.IntegrationTests` — existing endpoint test patterns

---

### Architecture tests

**Not in scope.** No new cross-module boundaries introduced.

---

### System tests

**Not in scope.** This is a simple read-only endpoint within a single service — no distributed flow to test.

---

## 6) Additional Notes

- This endpoint is a prerequisite for the address management UI in the Blazor frontend (future iteration).

---
