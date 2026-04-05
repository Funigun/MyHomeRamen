# Feature Brief — DeleteIngredient

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Accessibility** | `Manager` (Admin role) |
| **Feature name** | `DeleteIngredient` |
| **Short backend description** | A `DELETE /api/menu/ingredients/{id}` endpoint that removes an ingredient by its ID. Validates that the ingredient exists and is not referenced by any product (as a base ingredient or as a custom/additional ingredient). Returns `204 No Content` on success. |
| **Short frontend description** | No new pages or components needed. The delete flow is driven entirely by the `IngredientTable` component (Delete icon button + `MudDialog` confirmation) implemented as part of `GetIngredientsForManage`. The `OnIngredientDeletedAsync` placeholder handler in `IngredientsManagementPage` must be wired up to call this endpoint and refresh the ingredient list. |
| **Reference feature** | `DeleteCategory` (Menu module) · `GetIngredientsForManage` (Menu module) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes (wiring only — no new components) |

---

## 3) Feature description (Backend scope)

### New API endpoint

- **Endpoint**: `DELETE /api/menu/ingredients/{id}`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: `RestaurantManagerPolicy` (Admin role)
- **Route parameter**: `id` — `Guid` — the ingredient's unique identifier
- **Response**: `204 No Content` on success
- **Reference**: `DeleteCategoryEndpoint`

### Validation rules

| Rule | Error message |
|---|---|
| `id` is not empty | `"Ingredient ID must not be empty."` |
| Ingredient with the given ID exists | `"Ingredient with the specified ID does not exist."` |
| Ingredient is not used by any product as a base ingredient | `"Ingredient is used as a base ingredient by one or more products and cannot be deleted."` |
| Ingredient is not used by any product as a custom ingredient | `"Ingredient is used as an additional ingredient by one or more products and cannot be deleted."` |

The validator follows the same `ChildRules` pattern as `DeleteCategoryValidator`.

### New files

| File | Purpose |
|---|---|
| `Menu/Features/Ingredients/DeleteIngredient/DeleteIngredientEndpoint.cs` | Maps `DELETE ingredients/{id}` |
| `Menu/Features/Ingredients/DeleteIngredient/DeleteIngredientHandler.cs` | Loads and removes the ingredient, saves changes |
| `Menu/Features/Ingredients/DeleteIngredient/Models/DeleteIngredientRequest.cs` | Request record with `Guid Id` |
| `Menu/Features/Ingredients/DeleteIngredient/Policies/DeleteIngredientValidator.cs` | FluentValidation: existence + not-in-use checks |

### Persistence

- Add `IsIngredientUsedByProductAsync(IngredientId ingredientId, CancellationToken)` extension on `IQueryable<Product>` in `MyHomeRamen.Persistance/Common/DbExtensions.cs`
  - Returns `true` if any product has the ingredient in its `BaseIngredients` **or** `CustomIngredients` collection
- Reuse the generic `ExistsByIdAsync` and `GetBySelectorAsync` extensions for the existence check and entity load respectively
- **Reference**: `IsCategoryUsedByProductAsync`, `IsCategoryUsedByIngredientAsync` in `DbExtensions.cs`

---

## 4) Feature description (Frontend scope)

### Page change — `IngredientsManagementPage`

- **File**: `MyHomeRamen.Blazor/Features/Menu/Ingredients/IngredientsManagementPage.razor`
- Implement the `OnIngredientDeletedAsync(Guid id)` placeholder (added during `GetIngredientsForManage`) to:
  1. Call `MenuApiClient.DeleteIngredientAsync(id)` 
  2. On success, call `LoadIngredientsAsync()` to refresh the ingredient list
  3. Remove the `TODO` comment from the handler
- **Reference**: the equivalent delete handler in `CategoriesManagementPage.razor` if one exists

### Blazor API client — `MenuApiClient`

- Add `DeleteIngredientAsync(Guid id)` method to `MenuApiClient.cs`
- Sends `DELETE /api/menu/ingredients/{id}`
- Returns `bool` (or `HttpResponseMessage`) indicating success
- **Reference**: existing delete methods in `MenuApiClient.cs`

---

## 5) Testing Requirements

### Unit tests

**Not in scope.** No domain logic is exercised beyond the removal — no resequencing or domain events.

---

### Integration tests

**In scope.** The HTTP endpoint, persistence removal, validation guards, and authorisation rules must be tested against a real database via TestContainers.

Tests to create in `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/DeleteIngredientTests.cs`:

| Test | Expected result |
|---|---|
| `DeleteIngredient_ShouldReturnNoContent_ForValidId` | `204 No Content`, ingredient no longer exists in DB |
| `DeleteIngredient_ShouldReturnUnauthorized_ForUnauthenticatedUser` | `401 Unauthorized` |
| `DeleteIngredient_ShouldReturnForbidden_ForNonAdminRole` (Employee, Customer) | `403 Forbidden` |
| `DeleteIngredient_ShouldReturnBadRequest_ForNonExistentId` | `400 Bad Request` |
| `DeleteIngredient_ShouldReturnBadRequest_WhenIngredientIsUsedAsBaseIngredient` | `400 Bad Request` |
| `DeleteIngredient_ShouldReturnBadRequest_WhenIngredientIsUsedAsCustomIngredient` | `400 Bad Request` |

Reference: `MyHomeRamen.IntegrationTests/MenuModule/Categories/DeleteCategoryTests.cs`, `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/GetIngredientsForDropdownTests.cs`
