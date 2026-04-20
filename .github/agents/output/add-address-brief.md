# Feature Brief — AddAddress

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Users` |
| **Accessibility** | `Customer · Employee · Admin` (any authenticated user) |
| **Feature name** | `AddAddress` |
| **Short backend description** | New `POST /users/me/addresses` endpoint in the Identity.Api that creates a new address for the authenticated user. Enforces a maximum of 5 addresses per user. No Keycloak involvement — purely a database write via `IUsersDbContext`. |
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

- **Endpoint**: `POST /users/me/addresses`
- **Group**: `Account`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: any authenticated role (`Customer`, `Employee`, `Admin`)
- **Request body**: `street`, `building`, `apartment`, `city`, `zipCode`, `isDefault` (bool)
- **Response**: `201 Created` with the new address DTO; `400 Bad Request` for validation errors (handled by `ValidationPolicy`); `401 Unauthorized` for unauthenticated requests
- **Note**: no `409 Conflict` — the max-5 limit is a domain-level rule that raises a domain exception surfaced as `400 Bad Request`
- **Reference**: `RegisterEndpoint`, `RegisterEmployeeEndpoint`

### Domain changes

- Add `IsDefault` flag to the `Address` entity (bool property with `SetAsDefault()` / `UnsetDefault()` domain methods)
- Add an `AddAddress` method to `User` entity that:
  1. Enforces the max-5 business rule — throws a domain exception if the limit is already reached
  2. If the new address has `IsDefault = true`, first unsets the current default address (if any), then adds the new one as default
  3. If the new address has `IsDefault = false` and the user has no existing addresses, it may be set as default automatically (implementation decision)
- Reference: `User.Create`, `Address.Create`

### Persistence changes

- No migration required — `Addresses` table already exists, but a new `IsDefault` (bit/bool) column must be added via EF Core migration
- Handler loads `User` with addresses, calls `User.AddAddress`, and saves

### Contracts

- New request model: `AddAddressRequest` record with fields: `street`, `building`, `apartment`, `city`, `zipCode`, `isDefault` (bool)
- Request validation via FluentValidation — request body is placed in `ValidationPolicy` so validation errors return `400 Bad Request` automatically
- Reuse or create `AddressResponse` DTO that includes `id`, `street`, `building`, `apartment`, `city`, `zipCode`, `isDefault`
- Reference: `RegisterRequest`, `RegisterRequestValidator`

---

## 4) Feature description (Frontend scope)

Not in scope for this iteration.

---

## 5) Testing Requirements

### Unit tests

**In scope.** Validate domain `Address.IsDefault` behaviour, `User.AddAddress` max-5 rule, default-swap logic, and handler.

Tests to create:
- `UserAddAddressTests_ShouldAddAddress_WhenUnderLimit`
- `UserAddAddressTests_ShouldThrow_WhenMaxAddressesReached`
- `UserAddAddressTests_ShouldSetNewAddressAsDefault_AndUnsetPreviousDefault`
- `UserAddAddressTests_ShouldAllowNonDefaultAddress_WhenDefaultAlreadyExists`
- `AddAddressHandler_ShouldCreateAddress_WhenValid`
- `AddAddressHandler_ShouldFail_WhenLimitExceeded`

Reference: `MyHomeRamen.UnitTests` — existing domain and handler test patterns

---

### Integration tests

**In scope.** Verify the HTTP endpoint, auth enforcement, limit enforcement, and persistence.

Tests to create:
- `AddAddress_ShouldReturn201_WithNewAddress` (happy path, `isDefault: false`)
- `AddAddress_ShouldReturn201_AndSwapDefault_WhenIsDefaultTrue`
- `AddAddress_ShouldReturn400_WhenUserHas5Addresses`
- `AddAddress_ShouldReturn401_WhenUnauthenticated`
- `AddAddress_ShouldReturn400_WhenPayloadInvalid`

Reference: `MyHomeRamen.IntegrationTests` — existing endpoint test patterns

---

### Architecture tests

**Not in scope.** No new cross-module boundaries introduced.

---

### System tests

**Not in scope.** Single-service CRUD operation — no distributed flow.

---

## 6) Additional Notes

- The max-5 address limit is a business rule enforced at the domain level (`User.AddAddress`), surfaced as `400 Bad Request` — no `409 Conflict` response is used.
- There can be **at most one** default address at any time. When a new address is added with `isDefault: true`, the `User.AddAddress` domain method atomically unsets the previous default before setting the new one, so no inconsistent state can be persisted.
- `IsDefault` is part of the `AddressResponse` DTO so consumers can identify the default without a separate query.
- A new EF Core migration is required to add the `IsDefault` column to the `Addresses` table in the `identity` schema.

---
