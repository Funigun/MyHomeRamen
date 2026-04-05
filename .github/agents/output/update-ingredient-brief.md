# Feature Brief — UpdateIngredient

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Accessibility** | `Manager` (Admin role) |
| **Feature name** | `UpdateIngredient` |
| **Short backend description** | A `PUT /api/menu/ingredients/{id}` endpoint that updates the name, description, price, and category assignments of an existing ingredient. Applies the same field validation rules as `CreateIngredient` (name length, description length, price range, at least one category), plus an existence check and a name-uniqueness check that excludes the ingredient's own current name. Returns `200 OK` with the updated ingredient's ID. The `Ingredient` domain entity must gain an `Update` method to encapsulate mutation and validation. |
| **Short frontend description** | The `EditIngredientPage` (introduced by `GetIngredientById`) already renders `IngredientForm` in `FormMode.Edit` with a `TODO` placeholder for submit. This feature implements that placeholder: the form calls `MenuApiClient.UpdateIngredientAsync` when `Mode == FormMode.Edit` instead of `CreateIngredientAsync`. On success, navigate back to `IngredientsManagementPage`. No new pages or components are needed. |
| **Reference feature** | `CreateIngredient` (Menu module) · `DeleteIngredient` (Menu module) · `GetIngredientById` (Menu module) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes |

---

## 3) Feature description (Backend scope)

### New API endpoint

- **Endpoint**: `PUT /api/menu/ingredients/{id}`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: `RestaurantManagerPolicy` (Admin role)
- **Route parameter**: `id` — `Guid` — the ingredient's unique identifier
- **Request body**: `name`, `description`, `price`, `categoryIds`
- **Response**: `200 OK` with `UpdateIngredientResponse(Guid Id)`
- **Reference**: `CreateIngredientEndpoint`, `DeleteIngredientEndpoint`

### Validation rules

All field rules mirror `CreateIngredientValidator` exactly:

| Rule | Source |
|---|---|
| `name` — via `IngredientNameValidator` | same as `CreateIngredient` |
| `description` — via `IngredientDescriptionValidator` | same as `CreateIngredient` |
| `price` — via `IngredientPriceValidator` | same as `CreateIngredient` |
| `categoryIds` — `NotEmpty` | same as `CreateIngredient` |
| `id` — ingredient with the given ID must exist | same as `DeleteIngredient` |
| `name` — must be unique, **excluding the ingredient being updated** | new: `IsIngredientNameUniqueAsync(name, excludeId, ct)` |

### Domain changes — `Ingredient` entity

- Add `public void Update(string name, string description, decimal price, Collection<Category> categories)` method to `Ingredient`
- The method updates all mutable fields and calls `IngredientValidator.Validate(this)` before returning
- Reference: pattern consistent with other aggregate update methods in the codebase

### New files

| File | Purpose |
|---|---|
| `Menu/Features/Ingredients/UpdateIngredient/UpdateIngredientEndpoint.cs` | Maps `PUT ingredients/{id}`, binds route param + body |
| `Menu/Features/Ingredients/UpdateIngredient/UpdateIngredientHandler.cs` | Loads ingredient, calls `Update`, saves |
| `Menu/Features/Ingredients/UpdateIngredient/Models/UpdateIngredientRequest.cs` | Request record with `Guid Id` (route) + body fields; implements `IRequestId` |
| `Menu/Features/Ingredients/UpdateIngredient/Models/UpdateIngredientResponse.cs` | Response record `(Guid Id)` |
| `Menu/Features/Ingredients/UpdateIngredient/Models/Mappings.cs` | `ToResponse()` extension on `Ingredient` |
| `Menu/Features/Ingredients/UpdateIngredient/Policies/UpdateIngredientValidator.cs` | Reuses primitive validators + existence + name-uniqueness-excluding-self |

### Persistence

- Add `IsIngredientNameUniqueExcludingAsync(string name, IngredientId excludeId, CancellationToken)` extension on `IQueryable<Ingredient>` in `DbExtensions.cs`
  - Returns `true` when no other ingredient (Id ≠ excludeId) has the same name (case-insensitive)
- Reference: existing `IsIngredientNameUniqueAsync` in `DbExtensions.cs`

---

## 4) Feature description (Frontend scope)

### Page change — `EditIngredientPage.razor`

- **File**: `MyHomeRamen.Blazor/Features/Menu/Ingredients/EditIngredientPage.razor`
- Replace the `// TODO: Call EditIngredient endpoint` placeholder with:
  1. Map `IngredientFormModel` to `UpdateIngredientRequest` via `_model.ToEditRequest()`
  2. Call `MenuApiClient.UpdateIngredientAsync(Id, request)`
  3. On success, navigate back via `MenuNavigation.ToIngredientsManagement()`
  4. On `HttpRequestException`, display an error message

### Model change — `IngredientFormModel` (renamed from `IngredientModel` in `GetIngredientById`)

- Add `ToEditRequest()` method: `new UpdateIngredientRequest(Name, Description, Price, CategoryIds)`
- Reference: existing `ToCreateRequest()` in `IngredientModel.cs`

### Blazor API client — `MenuApiClient`

- Add `UpdateIngredientAsync(Guid id, UpdateIngredientRequest request)` method
- Sends `PUT /api/menu/ingredients/{id}` with JSON body
- Returns `UpdateIngredientResponse` (or simply `Task` — up to implementer, consistent with existing delete pattern)
- **Reference**: `CreateIngredientAsync`, `DeleteIngredientAsync` in `MenuApiClient.cs`

Add the corresponding request type:
- **File**: `MyHomeRamen.Blazor/Features/Menu/Ingredients/Requests/UpdateIngredientRequest.cs`
- `record UpdateIngredientRequest(string Name, string Description, decimal Price, IEnumerable<Guid> CategoryIds)`

### Component change — `IngredientForm.razor`

- In `SubmitAsync`, branch on `Mode`:
  - `FormMode.Create` → call `MenuApiClient.CreateIngredientAsync(_model.ToCreateRequest())` (existing behaviour)
  - `FormMode.Edit` → call `MenuApiClient.UpdateIngredientAsync(IngredientId, _model.ToEditRequest())`
- Add `[Parameter] public Guid IngredientId { get; set; }` for use in edit mode (passed from `EditIngredientPage`)
- Update submit button label: `"Create Ingredient"` vs `"Save Changes"` based on `Mode`
- Reference: existing `IngredientForm.razor` submit pattern

---

## 5) Testing Requirements

### Unit tests

**In scope.** The `Ingredient.Update()` domain method is new domain logic with validation behaviour that must be tested in isolation.

Tests to create in `MyHomeRamen.UnitTests/MenuModule/Ingredients/`:

| Test class | Coverage |
|---|---|
| `IngredientUpdateTests` | Valid update succeeds; name too short/long throws `DomainException`; description too long throws; price below/above range throws; empty categories throws |

- Reference: `MyHomeRamen.UnitTests/MenuModule/Ingredients/` (existing ingredient validation tests)

---

### Integration tests

**In scope.** The HTTP endpoint, update persistence, validation rules, and authorisation must be tested against a real database via TestContainers.

Tests to create in `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/UpdateIngredientTests.cs`:

| Test | Expected result |
|---|---|
| `UpdateIngredient_ShouldReturnOk_ForValidRequest` | `200 OK`, ingredient fields updated in DB |
| `UpdateIngredient_ShouldReturnUnauthorized_ForUnauthenticatedUser` | `401 Unauthorized` |
| `UpdateIngredient_ShouldReturnForbidden_ForNonAdminRole` (Employee, Customer) | `403 Forbidden` |
| `UpdateIngredient_ShouldReturnBadRequest_ForNonExistentId` | `400 Bad Request` |
| `UpdateIngredient_ShouldReturnBadRequest_ForInvalidRequest` (`[MemberData]` covering name/description/price/categoryIds violations) | `400 Bad Request` |
| `UpdateIngredient_ShouldReturnBadRequest_WhenNameAlreadyTakenByDifferentIngredient` | `400 Bad Request` |
| `UpdateIngredient_ShouldReturnOk_WhenNameIsUnchanged` | `200 OK` (name-uniqueness check correctly excludes self) |

- Reference: `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/GetIngredientsForDropdownTests.cs`, `MyHomeRamen.IntegrationTests/MenuModule/Categories/DeleteCategoryTests.cs`
