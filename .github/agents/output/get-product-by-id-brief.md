# Feature Brief — GetProductById

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Accessibility** | `Manager` |
| **Feature name** | `GetProductById` |
| **Short backend description** | New `GET /api/menu/products/{id}` endpoint that returns the full details of a single product by its ID for the management UI. Requires existence validation on the product id. Response includes name, description, price, categoryId, and ingredientIds. |
| **Short frontend description** | Add `GetProductByIdAsync` to `MenuApiClient` and a matching `GetProductByIdResponse`. Add `ProductModel.FromResponse()` static factory method. Adjust `ProductForm.razor` to accept an external `Model` parameter and a `ProductId` parameter (following the `IngredientForm` pattern). Add a new `EditProductPage.razor` that loads the product via the new endpoint, populates the form, and renders it in `FormMode.Edit`. |
| **Reference feature** | `GetIngredientById` (Menu module · API) · `EditIngredientPage` (Blazor) · `IngredientForm.razor` (Blazor) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes |

---

## 3) Feature description (Backend scope)

### New flow overview

```
[Client]
   │
   ▼ GET /api/menu/products/{id}
[GetProductByIdEndpoint]
   │ validates via GetProductByIdValidator (product exists)
   ▼
[GetProductByIdHandler]
   └── loads Product (with Category, BaseIngredients) from DB (not tracked)
          │
          ▼
   200 OK with GetProductByIdResponse
```

### New API endpoint

- **Endpoint**: `GET /api/menu/products/{id}`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: `Manager` role
- **Request**: `GetProductByIdRequest` — id from route
- **Response**: `200 OK` — `GetProductByIdResponse` with `Id`, `Name`, `Description`, `Price`, `CategoryId`, `IngredientIds`
- **Reference**: `GetIngredientByIdEndpoint`, `GetIngredientByIdHandler`, `GetIngredientByIdValidator`

### Validation (GetProductByIdValidator)

| Rule | Detail |
|---|---|
| Product exists by id | `dbContext.Products.ExistsByIdAsync((ProductId)id, ct)` |

- Reference: `GetIngredientByIdValidator`

---

## 4) Feature description (Frontend scope)

### New page: EditProductPage

- **File**: `Features/Menu/Products/EditProductPage.razor`
- **Route**: `/admin/menu/products/{Id:guid}/edit`
- **Access**: `Manager` role (same as `CreateProductPage`)
- **Behaviour**: on init, calls `MenuApiClient.GetProductByIdAsync(Id)` to load the current product data; populates `ProductModel` via `ProductModel.FromResponse()`; shows a loading indicator while fetching; then renders `<ProductForm>` in `FormMode.Edit` mode with the pre-populated model and the product id
- **Success behaviour**: navigates back to products management using `MenuNavigationService.ToProductsManagement()`
- **Reference**: `EditIngredientPage.razor`

### Modified component: ProductForm.razor

- Add `[Parameter] public ProductModel Model { get; set; }` — allows `EditProductPage` to inject a pre-populated model (follow `IngredientForm` pattern; initialise `_model` from this parameter in `OnParametersSet` or `OnInitializedAsync`)
- Add `[Parameter] public Guid ProductId { get; set; }` — the product id used when calling the update API in Edit mode (needed for a future `UpdateProduct` feature)
- The `SubmitAsync` method is **not changed** in this feature — Edit-mode submission is out of scope here and will be addressed in the `UpdateProduct` feature
- Reference: `IngredientForm.razor`

### Modified model: ProductModel.cs

- Add `static FromResponse(GetProductByIdResponse response)` factory method to construct a `ProductModel` from the API response
- Reference: `IngredientModel.FromResponse()`

### Modified service: MenuApiClient.cs

- Add `GetProductByIdAsync(Guid id)` — `GET /api/menu/products/{id}` → `GetProductByIdResponse?`
- Reference: `GetIngredientByIdAsync`

### New Blazor response type

| Type | Location |
|---|---|
| `GetProductByIdResponse` | `Features/Menu/Products/Responses/` |

---

## 5) Testing Requirements

### Unit tests

**Not in scope.** This feature contains no domain logic — the handler only loads and maps data; there is no behaviour to isolate.

### Integration tests

**In scope.** The HTTP endpoint, validation, authorisation rules, and persistence mapping must be verified with a real DB via TestContainers.

Tests to create in `MyHomeRamen.IntegrationTests/MenuModule/Products/GetProductByIdTests.cs`:

| Test case | Description |
|---|---|
| `GetProductById_ShouldReturnOk_ForAuthenticatedAdmin` | Happy path: verify `200 OK`, response body fields (id, name, price, categoryId, ingredientIds) |
| `GetProductById_ShouldReturnNotFound_WhenProductDoesNotExist` | Non-existent id → `404 Not Found` |
| `GetProductById_ShouldReturnUnauthorized_ForUnauthenticatedUser` | No auth header → `401 Unauthorized` |
| `GetProductById_ShouldReturnForbidden_ForNonAdminRole` | `Employee` and `Customer` roles → `403 Forbidden` |

- Reference: `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/GetIngredientByIdTests.cs`
