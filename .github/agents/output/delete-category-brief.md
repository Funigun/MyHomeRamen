# Feature Brief — DeleteCategory

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Accessibility** | `Manager` (Admin role only) |
| **Feature name** | `DeleteCategory` |
| **Short backend description** | A new `DELETE /api/menu/categories/{id}` endpoint removes a single `Category` entity from the database by its ID. The endpoint is restricted to the `RestaurantManagerPolicy` (Admin role) and returns `204 No Content` on success, `404 Not Found` if the category does not exist, or `409 Conflict` if the category is still referenced by at least one Product or Ingredient. |
| **Short frontend description** | The `CategoryTable` component already renders a Delete `MudIconButton` per row, but it is not yet wired up. The button must be connected to an `OnDelete` callback that the parent pages (`ProductsManagementPage`, `IngredientsManagementPage`) handle by calling the new `MenuApiClient.DeleteCategoryAsync` method and showing success or error feedback. |
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

- **Endpoint**: `DELETE /api/menu/categories/{id}`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: `RestaurantManagerPolicy` (Admin role only)
- **Route parameter**: `id` — the `Guid` of the category to delete
- **Response**:
  - `204 No Content` — category deleted successfully
  - `404 Not Found` — no category with the supplied `id` exists
  - `409 Conflict` — category is still referenced by one or more Products (for `CategoryType.Product`) or Ingredients (for `CategoryType.Ingredient`)
- **Reference**: `CreateCategoryEndpoint`, `GetCategoriesByTypeEndpoint`, `CategoriesGroup.cs`

### Domain changes

- The handler requires a `UpdateSortOrder(int sortOrder)` method on the `Category` entity to reassign contiguous sort-order values to the remaining categories after deletion. If the `UpdateCategoriesOrder` feature has already been implemented, this method is already available and should be reused. If `DeleteCategory` is implemented first, the method must be introduced here following the same validation pattern (delegate to `CategoryValidator`). No other domain change is needed.
- Reference: `Category.cs`, `CategoryValidator.cs`, `update-categories-order-brief.md`

### Persistence changes

- The handler queries the `Category` by its `CategoryId`. If not found, it returns a not-found result.
- Before deletion, the handler queries the relevant child table (`Products` or `Ingredients`, resolved from `category.CategoryType`) to check whether any records reference the category's ID. If at least one reference exists, the handler returns a conflict result.
- If both checks pass, the handler:
  1. Calls `DbContext.Remove(category)` to stage the delete.
  2. Fetches all remaining categories of the same `CategoryType`, ordered by their current `SortOrder` ascending.
  3. Calls `UpdateSortOrder` on each remaining category, assigning contiguous values starting from `1` (i.e. 1, 2, 3 … N).
  4. Calls `SaveChangesAsync` once — EF Core change tracking batches the `DELETE` and all `UPDATE` statements into a single round-trip, making the operation atomic.
- No migration is required — no schema change is involved.
- Reference: `CreateCategoryHandler`, `IMenuDbContext`, `MenuDbContext`

### Validation

- Route `id` must be a non-empty `Guid` (request-level, handled by FluentValidation)
- Category with the given `id` must exist — if not found, return `404 Not Found` (handler-level guard)
- Category must not be referenced by any Product (when `CategoryType.Product`) or Ingredient (when `CategoryType.Ingredient`) — if in use, return `409 Conflict` (handler-level guard, evaluated after the existence check)
- Reference: `GetCategoriesByTypeValidator`, `CreateCategoryValidator`

---

## 4) Feature description (Frontend scope)

### Modified component

- **`CategoryTable.razor`** (`MyHomeRamen.Blazor/Features/Menu/Categories/Components/`)
  - Add an `OnDelete` `EventCallback<Guid>` parameter
  - Wire the existing Delete `MudIconButton` to invoke `OnDelete` with the category's `Id`
  - Optionally add a confirmation dialog (e.g. `MudDialog`) before firing the callback to prevent accidental deletes

### Modified pages

Both management pages share identical behaviour and should delegate the call to `MenuApiClient`:

- **`ProductsManagementPage.razor`** (`/admin/menu/products-management`)
  - Subscribe to `CategoryTable.OnDelete`
  - On callback: call `MenuApiClient.DeleteCategoryAsync(id)`, then call `LoadCategoriesAsync` to reload the full list from the API and display a success alert
  - The page must **not** remove the item from local state manually — it must always reload from the server so that the updated, server-assigned `SortOrder` values are reflected correctly in the UI
  - Display an error alert on failure without modifying the local list

- **`IngredientsManagementPage.razor`** (`/admin/menu/ingredients-management`)
  - Same behaviour as above

### API client

- Extend `MenuApiClient` with:
  `DeleteCategoryAsync(Guid id, CancellationToken ct = default)`
- The method calls `DELETE /api/menu/categories/{id}` and calls `EnsureSuccessStatusCode()`
- Reference: `MenuApiClient.CreateCategoryAsync`, `MenuApiClient.GetCategoriesByTypeAsync`

---

## 5) Testing Requirements

### Unit tests

**Not in scope.** The delete operation carries no domain validation logic beyond a presence check. There is no domain method with invariants to test in isolation.

Reference: `MyHomeRamen.UnitTests/MenuModule/Categories/CategoryValidationTests.cs`

---

### Integration tests

**In scope.** The HTTP endpoint, handler, authorization rules, and persistence must be tested with a real DB via TestContainers.

Tests to create:
- `DeleteCategory_ShouldReturnNoContent_ForValidId` — happy path: seed multiple categories of the same type with no child references, delete one, assert `204`; assert the deleted record no longer exists in the DB; assert the remaining categories have contiguous `SortOrder` values starting from `1`
- `DeleteCategory_ShouldReturnNotFound_ForNonExistentId` — non-existent or wrong `id` must return `404`
- `DeleteCategory_ShouldReturnConflict_WhenCategoryIsUsedByProduct` — seed a `CategoryType.Product` category referenced by at least one Product; assert `409 Conflict`
- `DeleteCategory_ShouldReturnConflict_WhenCategoryIsUsedByIngredient` — seed a `CategoryType.Ingredient` category referenced by at least one Ingredient; assert `409 Conflict`
- `DeleteCategory_ShouldReturnUnauthorized_ForUnauthenticatedUser`
- `DeleteCategory_ShouldReturnForbidden_ForNonManagerRoles` (Employee, Customer)
- `DeleteCategory_ShouldReturnBadRequest_ForEmptyGuid`

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

- **Referential integrity**: The handler must perform an explicit in-use check before the delete to avoid relying on DB-level FK constraint errors. The check is type-aware: for `CategoryType.Product` it queries the `Products` table; for `CategoryType.Ingredient` it queries the `Ingredients` table. A `409 Conflict` response with a human-readable message (e.g. `"Category is still in use and cannot be deleted."`) is returned when references are found. Cascade-delete must **not** be configured for this relationship.
- **Soft delete**: The current `Category` entity does not implement soft-delete. A hard delete is assumed for the initial implementation; soft-delete can be considered as a follow-up if audit history is required.
- **Confirmation UX**: An in-component confirmation dialog (e.g. `MudDialog` with "Cancel" / "Delete" actions) is recommended to prevent accidental data loss, consistent with common admin UI patterns.
- **SortOrder compaction**: The backend compacts `SortOrder` values for the remaining categories of the same type atomically within the same `SaveChangesAsync` call as the delete. The frontend must always reload from the server after a successful delete to display the up-to-date order numbers — client-side list manipulation is insufficient and must be avoided.

---
