# Feature Brief — UpdateCategoriesOrder

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Accessibility** | `Manager` (Admin role only) |
| **Feature name** | `UpdateCategoriesOrder` |
| **Short backend description** | A new `PUT /api/menu/categories/order` endpoint accepts an ordered list of category IDs with their new `SortOrder` values and persists them to the database in a single batch operation. The endpoint is restricted to the `RestaurantManagerPolicy` (Admin role). |
| **Short frontend description** | Both `ProductsManagementPage` and `IngredientsManagementPage` already render a drag-and-drop `CategoryTable` that reorders items in local state via `OnItemDropped`. A "Save Order" action (button or auto-save on drop) must be wired up to call the new API endpoint and provide success/error feedback to the user. |
| **Reference feature** | `CreateCategory` (Menu module) · `GetCategoriesByType` (Menu module) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes |

---

## 3) Feature description (Backend scope)

### New API endpoint

- **Endpoint**: `PUT /api/menu/categories/order`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: `RestaurantManagerPolicy` (Admin role only)
- **Request body**: list of `{ id: Guid, sortOrder: int }` pairs covering all categories being reordered
- **Response**: `204 No Content` on success
- **Reference**: `CreateCategoryEndpoint`, `GetCategoriesByTypeEndpoint`

### Domain changes

- Extend `Category` entity with an `UpdateSortOrder(int newSortOrder)` method that validates the new value via `CategoryValidator` before applying the change (consistent with the existing `Create` factory pattern)
- Reference: `Category.cs`, `CategoryValidator.cs`

### Persistence changes

- The handler fetches all affected `Category` entities by their IDs in a single query, applies `UpdateSortOrder` to each, and calls `SaveChangesAsync` once (batch update via EF Core change tracking)
- No new migration is required — `SortOrder` column already exists
- Reference: `CreateCategoryHandler`, `IMenuDbContext`

### Validation

- Request list must not be empty
- Each `SortOrder` value must satisfy `CategoryConstants.MinSortOrder` (reuse `CategorySortOrderValidator`)
- All supplied IDs must be unique within the request
- Reference: `CreateCategoryValidator`, `CategorySortOrderValidator`

---

## 4) Feature description (Frontend scope)

### Modified pages

Both management pages share identical behaviour and should delegate the call to `MenuApiClient`:

- **`ProductsManagementPage.razor`** (`/admin/menu/products-management`)
  - Wire up a "Save Order" button (or react to `OnItemDropped`) that calls `MenuApiClient.UpdateCategoriesOrderAsync`
  - Display a success alert on `204 No Content` response
  - Display an error alert on failure, without reverting the local drag-and-drop order

- **`IngredientsManagementPage.razor`** (`/admin/menu/ingredients-management`)
  - Same behaviour as above

### Modified component

- **`CategoryTable.razor`** — expose an `OnOrderChanged` `EventCallback<List<GetCategoriesByTypeResponse>>` (or equivalent) that the parent pages can subscribe to, or add a "Save Order" button inside the component itself

### API client

- Extend `MenuApiClient` with:
  `UpdateCategoriesOrderAsync(UpdateCategoriesOrderRequest request, CancellationToken ct = default)`
- The method calls `PUT /api/menu/categories/order` and calls `EnsureSuccessStatusCode()`
- Add `UpdateCategoriesOrderRequest` model to the Blazor client-side models (list of `CategoryOrderItem { Guid Id, int SortOrder }`)
- Reference: `MenuApiClient.CreateCategoryAsync`, `MenuApiClient.GetCategoriesByTypeAsync`

---

## 5) Testing Requirements

### Unit tests

**In scope.** The new `UpdateSortOrder` domain method carries validation logic that must be tested in isolation.

Tests to create:
- `Category_UpdateSortOrder_ShouldFail_When_SortOrderIsBelowMinimum`
- `Category_UpdateSortOrder_ShouldSucceed_When_SortOrderIsValid`

Reference: `MyHomeRamen.UnitTests/MenuModule/Categories/CategoryValidatorsTests.cs`

---

### Integration tests

**In scope.** The HTTP endpoint, handler, authorization rules, and batch persistence must be tested with a real DB via TestContainers.

Tests to create:
- `UpdateCategoriesOrder_ShouldReturnNoContent_ForValidRequest` — happy path: reorder seeded categories and assert `SortOrder` values are persisted correctly
- `UpdateCategoriesOrder_ShouldReturnUnauthorized_ForUnauthenticatedUser`
- `UpdateCategoriesOrder_ShouldReturnForbidden_ForNonManagerRoles` (Employee, Customer)
- `UpdateCategoriesOrder_ShouldReturnBadRequest_ForEmptyList`
- `UpdateCategoriesOrder_ShouldReturnBadRequest_ForInvalidSortOrder` (value below minimum)

Reference: `MyHomeRamen.IntegrationTests/MenuModule/CreateCategoryTests.cs`, `MyHomeRamen.IntegrationTests/MenuModule/GetCategoriesByTypeTests.cs`

---

### Architecture tests

**Not in scope.** The feature stays entirely within the Menu module and introduces no new cross-module dependencies. Existing architecture rules already cover the Menu API boundaries.

Reference: `MyHomeRamen.ArchitectureTests/ModuleTests/Menu/ApiBoundariesTests.cs`

---

### System tests

**Not in scope.** This feature has no asynchronous cross-service flows, message broker interactions, or distributed workflows that require full Aspire orchestration. Integration tests against the API + DB are sufficient.

---

## 6) Additional Notes

- **Batch update strategy**: Prefer fetching all category entities in one `WHERE id IN (...)` query and relying on EF Core change tracking for the batch `UPDATE`, avoiding per-entity round trips.
- **Partial reorder**: The endpoint should accept a subset of categories of the same `CategoryType` if partial reordering is a valid use case; otherwise enforce that the full list for the given type is supplied (to be decided during implementation).
- **Concurrency**: If two admins reorder simultaneously, the last write wins (no optimistic concurrency required for the initial implementation).
- **Frontend UX**: Consider whether order is saved automatically on each drag-drop (`OnItemDropped`) or explicitly via a "Save Order" button. An explicit button is safer and matches the existing page pattern (form → success/error alert).
- **SortOrder gap handling**: The API should accept whatever `SortOrder` values the client supplies (e.g. 1, 2, 3 … N), normalisation is the client's responsibility.

---
