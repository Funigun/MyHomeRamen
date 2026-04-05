# Feature Brief — GetIngredientsForManage

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Accessibility** | `Manager` (Admin role) |
| **Feature name** | `GetIngredientsForManage` |
| **Short backend description** | A `GET /api/menu/ingredients/manage` endpoint that returns a filtered list of ingredients for the admin management view. Supports optional filtering by name (contains, case-insensitive) and by category IDs (list). The response includes `Id`, `Name`, and `Description` only — categories are intentionally excluded. |
| **Short frontend description** | A new `IngredientTable` Blazor component that displays the ingredient list with `Name`, `Description`, and an `Actions` column containing `Edit` and `Delete` icon buttons. The component is wired into the existing `IngredientsManagementPage` at the bottom of the page, below the existing Ingredients header and Add Ingredient button. |
| **Reference feature** | `GetIngredientsForDropdown` (Menu module) · `GetCategoriesByType` (Menu module) · `CreateIngredient` (Menu module) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes |

---

## 3) Feature description (Backend scope)

### New API endpoint

- **Endpoint**: `GET /api/menu/ingredients/manage`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: `RestaurantManagerPolicy` (Admin role)
- **Query parameters** (all optional):
  - `name` — `string?`: case-insensitive contains filter on `Ingredient.Name`
  - `categoryIds` — `IEnumerable<Guid>?`: restricts results to ingredients that belong to **all** of the specified categories (intersection), or **any** (union) — to be decided during implementation; document the chosen semantic in the endpoint description
- **Response**: `200 OK` with `IEnumerable<GetIngredientsForManageResponse>`
- **Reference**: `GetIngredientsForDropdownEndpoint`, `GetCategoriesByTypeEndpoint`

### Response model

```
GetIngredientsForManageResponse(Guid Id, string Name, string Description)
```

Categories are **not** included in the response — they are fetched separately (e.g. via the existing `GetIngredientsForDropdown` endpoint) if needed.

### New files

| File | Purpose |
|---|---|
| `Menu/Features/Ingredients/GetIngredientsForManage/GetIngredientsForManageEndpoint.cs` | Maps `GET ingredients/manage`, binds query params via `[AsParameters]` |
| `Menu/Features/Ingredients/GetIngredientsForManage/GetIngredientsForManageHandler.cs` | Queries `IMenuDbContext.Ingredients` applying the filters |
| `Menu/Features/Ingredients/GetIngredientsForManage/Models/GetIngredientsForManageRequest.cs` | Request record with `string? Name` and `IEnumerable<Guid>? CategoryIds` |
| `Menu/Features/Ingredients/GetIngredientsForManage/Models/GetIngredientsForManageResponse.cs` | Response record `(Guid Id, string Name, string Description)` |
| `Menu/Features/Ingredients/GetIngredientsForManage/Models/Mappings.cs` | `ToResponse()` extension on `Ingredient` |
| `Menu/Features/Ingredients/GetIngredientsForManage/Policies/GetIngredientsForManageValidator.cs` | FluentValidation: optional name max-length guard |

### Persistence

- Add a `ForManage` extension method on `DbSet<Ingredient>` (or `IQueryable<Ingredient>`) in `MyHomeRamen.Persistance/Common/DbExtensions.cs`
- The method accepts `string? name` and `IEnumerable<Guid>? categoryIds` parameters and applies `AsNoTracking()`, name contains filter (when provided), category filter (when provided), and a stable ordering (e.g. `OrderBy(i => i.Name)`)
- **Reference**: existing `ForDropdown` extension on `DbSet<Ingredient>` in `DbExtensions.cs`

---

## 4) Feature description (Frontend scope)

### New component — `IngredientTable`

- **File**: `MyHomeRamen.Blazor/Features/Menu/Ingredients/Components/IngredientTable.razor`
- **Parameters**:

| Parameter | Type | Required | Description |
|---|---|---|---|
| `Items` | `List<GetIngredientsForManageResponse>` | yes | Ingredient list to display |
| `IsLoading` | `bool` | no | Shows a progress bar when true |
| `OnEdit` | `EventCallback<Guid>` | no | Fired when the Edit button is clicked, passes ingredient `Id` |
| `OnDelete` | `EventCallback<Guid>` | no | Fired after delete confirmation, passes ingredient `Id` |

- **Columns**: `Name`, `Description`, `Actions`
- **Actions column**: Edit `MudIconButton` (`Icons.Material.Filled.Edit`) and Delete `MudIconButton` (`Icons.Material.Filled.Delete`, `Color.Error`) — Delete should show a `MudDialog` confirmation before firing `OnDelete`, following the same pattern as `CategoryTable`
- **Empty state**: `MudAlert Severity.Info` "No ingredients found."
- **Loading state**: `MudProgressLinear Indeterminate="true"`
- **Reference**: `CategoryTable.razor` for layout, dialog confirmation, and parameter patterns (omit drag-and-drop — ingredients are not reorderable)

### Blazor API client — `MenuApiClient`

- Add `GetIngredientsForManageAsync(string? name = null, IEnumerable<Guid>? categoryIds = null)` method to `MenuApiClient.cs`
- Builds the query string conditionally (append `name=` when provided; append repeated `categoryIds=` params when provided)
- Returns `IEnumerable<GetIngredientsForManageResponse>`
- **Reference**: `GetIngredientsForDropdownAsync` in `MenuApiClient.cs`

Add the corresponding response type:
- **File**: `MyHomeRamen.Blazor/Features/Menu/Ingredients/Responses/GetIngredientsForManageResponse.cs`
- `record GetIngredientsForManageResponse(Guid Id, string Name, string Description)`

### Page change — `IngredientsManagementPage`

- **File**: `MyHomeRamen.Blazor/Features/Menu/Ingredients/IngredientsManagementPage.razor`
- At the bottom of the existing page (below the "Ingredients" section header and Add Ingredient button added by `CreateIngredient`) add:
  1. `IngredientTable` component bound to a new `_ingredients` state list and `_isIngredientsLoading` flag
  2. `OnEdit` wired to `MenuNavigation.ToEditIngredient(id)` *(navigation target is a placeholder — `ToEditIngredient` method and its route will be implemented as a follow-up feature; add the stub to `MenuNavigationService` now with a `TODO` comment)*
  3. `OnDelete` wired to a `OnIngredientDeletedAsync(Guid id)` handler that calls the delete endpoint *(delete endpoint is a separate feature; the handler can be a placeholder with a `TODO` comment for now)*
- Call `LoadIngredientsAsync()` in `OnInitializedAsync`

> **Out of scope for this task**: `EditIngredient` endpoint and page, `DeleteIngredient` endpoint. The table and navigation stubs are wired up but their backing features will be separate tasks.

### Existing components / services reused

- `IngredientForm.razor` — unchanged
- `MenuApiClient` — extended with `GetIngredientsForManageAsync`
- `MenuNavigationService` — extended with stub `ToEditIngredient(Guid id)`

---

## 5) Testing Requirements

### Unit tests

**Not in scope.** The feature contains no domain logic — the handler is a filtered read that delegates to EF Core. There are no domain rules to unit-test.

---

### Integration tests

**In scope.** The HTTP endpoint, query-parameter filtering, and authorisation rules must be tested against a real database via TestContainers.

Tests to create in `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/GetIngredientsForManageTests.cs`:

| Test | Expected result |
|---|---|
| `GetIngredientsForManage_ShouldReturnOk_ForAuthenticatedAdmin` | `200 OK`, non-null list |
| `GetIngredientsForManage_ShouldReturnUnauthorized_ForUnauthenticatedUser` | `401 Unauthorized` |
| `GetIngredientsForManage_ShouldReturnForbidden_ForNonAdminRole` (Employee, Customer) | `403 Forbidden` |
| `GetIngredientsForManage_ShouldFilterByName_WhenNameProvided` | Returns only ingredients whose name contains the filter value |
| `GetIngredientsForManage_ShouldFilterByCategories_WhenCategoryIdsProvided` | Returns only ingredients belonging to the specified categories |
| `GetIngredientsForManage_ShouldReturnEmptyList_WhenNoIngredientsMatchFilters` | `200 OK`, empty list |
| `GetIngredientsForManage_ResponseShouldNotContainCategories` | Response items have `Id`, `Name`, `Description` only |

Reference: `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/GetIngredientsForDropdownTests.cs`, `MyHomeRamen.IntegrationTests/MenuModule/Categories/CreateCategoryTests.cs`

---
