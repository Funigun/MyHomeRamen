# Feature Brief — UpdateAddress

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Users` |
| **Accessibility** | `Customer · Employee · Admin` (any authenticated user) |
| **Feature name** | `UpdateAddress` |
| **Short backend description** | New `PUT /users/me/addresses/{id}` endpoint in the Identity.Api that updates an existing address for the authenticated user. No Keycloak involvement — purely a database update via `IUsersDbContext`. The handler verifies the address belongs to the requesting user before applying changes. |
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

- **Endpoint**: `PUT /users/me/addresses/{id}`
- **Group**: `Account`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: any authenticated role (`Customer`, `Employee`, `Admin`)
- **Request body**: `street`, `building`, `apartment`, `city`, `zipCode`, `isDefault` (bool)
- **Response**: `200 OK` with the updated address DTO; `404 Not Found` if the address does not exist or does not belong to the user; `400 Bad Request` for validation errors (handled by `ValidationPolicy`); `401 Unauthorized` for unauthenticated requests
- **Reference**: `RegisterEndpoint`

### Domain changes

- No domain model changes required — `Address.IsDefault`, `SetAsDefault()`, `UnsetDefault()` and `User.AddAddress` default-swap logic are introduced by the `AddAddress` feature
- The `Address` entity needs an `Update` method that accepts new field values (`street`, `building`, `apartment`, `city`, `zipCode`) and applies them
- The `User` entity needs an `UpdateAddress` method that: finds the target address by ID, applies field changes via `Address.Update`, and if `isDefault: true` is requested — unsets the current default (if different) and sets the target address as default
- Reference: `Address.Create`, `Address.SetAsDefault`, `Address.UnsetDefault`, `User.AddAddress` (default-swap pattern)

### Persistence changes

- No migration required — `Addresses` table schema already includes the `IsDefault` column (added by the `AddAddress` feature migration)
- Handler loads `User` with addresses, calls `User.UpdateAddress`, and saves

### Contracts

- New request model: `UpdateAddressRequest` record with fields: `street`, `building`, `apartment`, `city`, `zipCode`, `isDefault` (bool)
- Validation rules are identical to `AddAddressRequest` except there is no max-addresses-limit check — request body is placed in `ValidationPolicy` so validation errors return `400 Bad Request` automatically
- Reuse `AddressResponse` DTO (includes `id`, `street`, `building`, `apartment`, `city`, `zipCode`, `isDefault`)
- Reference: `AddAddressRequest`, `AddAddressRequestValidator`

---

## 4) Feature description (Frontend scope)

Not in scope for this iteration.

---

## 5) Testing Requirements

### Unit tests

**In scope.** Validate domain `Address.Update`, `User.UpdateAddress` default-swap logic, and handler.

Tests to create:
- `AddressUpdateTests` — valid update scenarios, empty/null field validation
- `UserUpdateAddressTests_ShouldSetAsDefault_AndUnsetPreviousDefault`
- `UserUpdateAddressTests_ShouldNotChangeDefault_WhenIsDefaultFalse`
- `UpdateAddressHandler_ShouldUpdateAddress_WhenValid`
- `UpdateAddressHandler_ShouldFail_WhenAddressNotFound`
- `UpdateAddressHandler_ShouldFail_WhenAddressBelongsToAnotherUser`

Reference: `MyHomeRamen.UnitTests` — existing domain and handler test patterns

---

### Integration tests

**In scope.** Verify the HTTP endpoint, auth enforcement, ownership check, and persistence.

Tests to create:
- `UpdateAddress_ShouldReturn200_WithUpdatedAddress` (happy path)
- `UpdateAddress_ShouldReturn200_AndSwapDefault_WhenIsDefaultTrue`
- `UpdateAddress_ShouldReturn404_WhenAddressNotFound`
- `UpdateAddress_ShouldReturn401_WhenUnauthenticated`
- `UpdateAddress_ShouldReturn400_WhenPayloadInvalid`

Reference: `MyHomeRamen.IntegrationTests` — existing endpoint test patterns

---

### Architecture tests

**Not in scope.** No new cross-module boundaries introduced.

---

### System tests

**Not in scope.** Single-service CRUD operation — no distributed flow.

---

## 6) Additional Notes

- Ownership validation is critical — the handler must verify the address belongs to the authenticated user before allowing any changes.
- The default-swap logic mirrors the `AddAddress` feature: setting `isDefault: true` on an address atomically unsets the current default (if it's a different address) before marking the target as default.
- Setting `isDefault: false` on an address that is currently the default is allowed — it simply unsets the default, leaving the user with no default address.
- Validation rules are intentionally kept in sync with `AddAddressRequest` — the only difference is the absence of the max-5 addresses check.

---
