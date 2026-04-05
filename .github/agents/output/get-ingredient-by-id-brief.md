# Feature Brief — GetIngredientById

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Accessibility** | `Manager` (Admin role) |
| **Feature name** | `GetIngredientById` |
| **Short backend description** | A `GET /api/menu/ingredients/{id}` endpoint that returns the full details of a single ingredient by its ID, including its associated category IDs. Intended to pre-fill the edit form in the admin management view. |
| **Short frontend description** | A new `EditIngredientPage` Blazor page that loads ingredient details via `GetIngredientById` and renders the existing `IngredientForm` component in edit mode. Navigated to from the `IngredientTable` Edit button (wired via the `MenuNavigationService.ToEditIngredient(id)` stub added in `GetIngredientsForManage`). The page submits changes via a separate `EditIngredient` feature (future task). |
| **Reference feature** | `CreateIngredient` (Menu module) · `GetIngredientsForManage` (Menu module) · `GetCategoriesByType` (Menu module) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes |

---

## 3) Feature description (Backend scope)

### New API endpoint

- **Endpoint**: `GET /api/menu/ingredients/{id}`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: `RestaurantManagerPolicy` (Admin role)
- **Route parameter**: `id` — `Guid` — the ingredient's unique identifier
- **Response**: `200 OK` with `GetIngredientByIdResponse`
- **Reference**: `GetIngredientsForDropdownEndpoint`, `GetCategoriesByTypeEndpoint`

### Response model

```
GetIngredientByIdResponse(Guid Id, string Name, string Description, decimal Price, IEnumerable<Guid> CategoryIds)
```

Category IDs are included so the edit form can pre-select the correct categories in the multi-select.

### New files

| File | Purpose |
|---|---|
| `Menu/Features/Ingredients/GetIngredientById/GetIngredientByIdEndpoint.cs` | Maps `GET ingredients/{id}`, binds route param via `[AsParameters]` |
| `Menu/Features/Ingredients/GetIngredientById/GetIngredientByIdHandler.cs` | Loads ingredient from `IMenuDbContext.Ingredients` including categories |
| `Menu/Features/Ingredients/GetIngredientById/Models/GetIngredientByIdRequest.cs` | Request record with `Guid Id` |
| `Menu/Features/Ingredients/GetIngredientById/Models/GetIngredientByIdResponse.cs` | Response record `(Guid Id, string Name, string Description, decimal Price, IEnumerable<Guid> CategoryIds)` |
| `Menu/Features/Ingredients/GetIngredientById/Models/Mappings.cs` | `ToResponse()` extension on `Ingredient` |
| `Menu/Features/Ingredients/GetIngredientById/Policies/GetIngredientByIdValidator.cs` | FluentValidation: ingredient with the given ID must exist |

### Persistence

- The handler loads the ingredient with its `Categories` navigation property included (using `Include`) to populate `CategoryIds` in the response
- No new `DbExtensions` method required — reuse the generic `GetBySelectorAsync` / `ExistsByIdAsync` extensions already present in `DbExtensions.cs`

---

## 4) Feature description (Frontend scope)

### New page — `EditIngredientPage`

- **File**: `MyHomeRamen.Blazor/Features/Menu/Ingredients/EditIngredientPage.razor`
- **Route**: `/menu/ingredients/{Id:guid}/edit`
- **Access**: Admin role only (same as management pages)
- **Behaviour**:
  1. On `OnInitializedAsync`, call `MenuApiClient.GetIngredientByIdAsync(Id)` to load ingredient details
  2. Populate an `IngredientModel` (or equivalent local model) with the returned values
  3. Render the existing `IngredientForm` component in edit mode, pre-filled with loaded data
  4. On form submit, call the `EditIngredient` endpoint (**out of scope** — handler is a placeholder with a `TODO` comment for now)
  5. On success, navigate back to `IngredientsManagementPage` via `MenuNavigationService`
- **Reference**: `IngredientsManagementPage.razor`, `IngredientForm.razor`, and the equivalent category edit page if one exists

### Page change — `MenuNavigationService`

- Implement the `ToEditIngredient(Guid id)` stub added during `GetIngredientsForManage` with the actual route `/menu/ingredients/{id}/edit`
- Remove the `TODO` comment from the stub

### Blazor API client — `MenuApiClient`

- Add `GetIngredientByIdAsync(Guid id)` method to `MenuApiClient.cs`
- Returns `GetIngredientByIdResponse?`
- **Reference**: `GetIngredientsForDropdownAsync` in `MenuApiClient.cs`

Add the corresponding response type:
- **File**: `MyHomeRamen.Blazor/Features/Menu/Ingredients/Responses/GetIngredientByIdResponse.cs`
- `record GetIngredientByIdResponse(Guid Id, string Name, string Description, decimal Price, IEnumerable<Guid> CategoryIds)`

### Validation note

The `EditIngredientPage` form validation (client-side) should follow the same field rules as `CreateIngredientValidator`:
- `Name` — `IngredientNameValidator`
- `Description` — `IngredientDescriptionValidator`
- `Price` — `IngredientPriceValidator`
- At least one category selected

The actual server-side submission validation belongs to the future `EditIngredient` feature.

---

## 5) Testing Requirements

### Unit tests

**Not in scope.** The feature contains no domain logic — the handler is a straightforward single-entity read by primary key.

---

### Integration tests

**In scope.** The HTTP endpoint, response shape, and authorisation rules must be tested against a real database via TestContainers.

Tests to create in `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/GetIngredientByIdTests.cs`:

| Test | Expected result |
|---|---|
| `GetIngredientById_ShouldReturnOk_ForAuthenticatedAdmin` | `200 OK`, correct ingredient data including `CategoryIds` |
| `GetIngredientById_ShouldReturnUnauthorized_ForUnauthenticatedUser` | `401 Unauthorized` |
| `GetIngredientById_ShouldReturnForbidden_ForNonAdminRole` (Employee, Customer) | `403 Forbidden` |
| `GetIngredientById_ShouldReturnBadRequest_ForNonExistentId` | `400 Bad Request` (validator fires) |
| `GetIngredientById_ResponseShouldContainCategoryIds` | Response includes correct `CategoryIds` matching the ingredient's categories |

Reference: `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/GetIngredientsForDropdownTests.cs`, `MyHomeRamen.IntegrationTests/MenuModule/Categories/DeleteCategoryTests.cs`
