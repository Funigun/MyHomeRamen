# Feature Brief — UpdateProduct

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Accessibility** | `Manager` |
| **Feature name** | `UpdateProduct` |
| **Short backend description** | New `PUT /api/menu/products/{id}` endpoint that updates an existing product's name, description, price, category, and ingredients. Requires a new `Update()` method on the `Product` domain entity, a new `IsProductNameUniqueExcludingAsync` DB extension, and a prerequisite `GetProductById` endpoint used by the frontend to pre-populate the edit form. Validation mirrors `CreateProductValidator`: field rules (name, description, price), name uniqueness excluding the product being updated, category existence, and ingredients not empty — plus an existence check for the product id. |
| **Short frontend description** | Adjust `ProductForm.razor` to support Edit mode (call `UpdateProductAsync` instead of `CreateProductAsync`). Add a new `EditProductPage.razor` page that loads the existing product via `GetProductByIdAsync` and renders the form in Edit mode. Extend `MenuApiClient`, `ProductModel`, and `MenuNavigationService` with the new operations. |
| **Reference feature** | `CreateProduct` (Menu module) · `UpdateIngredient` (Menu module) · `EditIngredientPage` (Blazor) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes |

---

## 3) Feature description (Backend scope)

### Prerequisite: GetProductById endpoint

Before implementing `UpdateProduct`, a `GET /api/menu/products/{id}` endpoint is required so the Blazor edit page can load the current product data.

- **Endpoint**: `GET /api/menu/products/{id}`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: `Manager` role
- **Response**: `200 OK`
- **Reference**: `GetIngredientByIdEndpoint`, `GetIngredientByIdHandler`

### New flow overview

```
[Client]
   │
   ▼ PUT /api/menu/products/{id}
[UpdateProductEndpoint]
   │ validates via UpdateProductValidator
   ▼
[UpdateProductHandler]
   ├── loads Product (with Categories, BaseIngredients) from DB
   ├── loads Category by CategoryId
   ├── loads Ingredients by IngredientIds
   └── calls product.Update(name, description, price, category, ingredients)
          │
          ▼
   SaveChangesAsync → 200 OK with UpdateProductResponse
```

### New API endpoint

- **Endpoint**: `PUT /api/menu/products/{id}`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: `Manager` role
- **Request**: `UpdateProductRequest` — name, description, price, categoryId, ingredientIds (body); id from route via `UpdateProductIRequestId`
- **Response**: `200 OK`
- **Reference**: `UpdateIngredientEndpoint`, `UpdateIngredientHandler`, `UpdateIngredientRequest`

### Domain changes — Menu module

- New `Update(string name, string? description, decimal price, Category category, IEnumerable<Ingredient> ingredients)` method on `Product` entity, calling `ProductValidator.ValidateProduct(this)` before returning
- Reference: `Ingredient.Update()`, `Product.Create()`

### Persistence changes — Menu module

- New `IsProductNameUniqueExcludingAsync` DB extension on `IQueryable<Product>` in `MyHomeRamen.Persistance/Common/DbExtensions.cs`
- Follows the pattern of `IsIngredientNameUniqueExcludingAsync`

```csharp
// ✅ New extension — name uniqueness excluding the product being updated
public static async Task<bool> IsProductNameUniqueExcludingAsync(
    this IQueryable<Product> query,
    string name,
    ProductId excludeId,
    CancellationToken cancellationToken = default)
{
    return !await query.AnyAsync(p => p.Id != excludeId && p.Name.ToLower() == name.ToLower(), cancellationToken);
}
```

### Validation (UpdateProductValidator)

Mirrors `CreateProductValidator`, with two additional rules:

| Rule | Source |
|---|---|
| Name — format/length | `ProductNameValidator` (from `MyHomeRamen.Common.Contracts`) |
| Description — format/length | `ProductDescriptionValidator` (from `MyHomeRamen.Common.Contracts`) |
| Price — range/format | `ProductPriceValidator` (from `MyHomeRamen.Common.Contracts`) |
| Product exists by id | `dbContext.Products.ExistsByIdAsync((ProductId)id, ct)` via `IHttpContextAccessor` |
| Name unique excluding current | `dbContext.Products.IsProductNameUniqueExcludingAsync(name, (ProductId)id, ct)` via `IHttpContextAccessor` |
| Category exists | `dbContext.Categories.AnyAsync(c => c.Id == new CategoryId(categoryId), ct)` |
| Ingredients not empty | `NotEmpty()` |

- Reference: `UpdateIngredientValidator`, `CreateProductValidator`

---

## 4) Feature description (Frontend scope)

### New page: EditProductPage

- **Route**: `/admin/menu/products/{Id:guid}/edit`
- **Access**: `Manager` role (same as `CreateProductPage`)
- **Behaviour**: on init, calls `MenuApiClient.GetProductByIdAsync(Id)` to load the current product, populates `ProductModel` via a new static `ProductModel.FromResponse()` method, then renders `<ProductForm>` in `FormMode.Edit` mode
- **Success behaviour**: navigates back to product management/detail using `MenuNavigationService`
- **Reference**: `EditIngredientPage.razor`

### Modified component: ProductForm.razor

- The `@code` block's `SubmitAsync` method currently only handles Create. It must be adjusted to:
  - When `Mode == FormMode.Edit`: call `MenuApiClient.UpdateProductAsync(ProductId, _model.ToUpdateRequest())` and invoke `OnSuccess`
  - When `Mode == FormMode.Create`: existing behaviour unchanged
- Add a `[Parameter] public Guid ProductId { get; set; }` parameter (needed when in Edit mode to pass the id to the API call)
- Add a `[Parameter] public ProductModel Model { get; set; }` parameter so `EditProductPage` can inject the pre-loaded model (follow `IngredientForm` pattern)
- Reference: `IngredientForm.razor`, `ProductForm.razor`

### Modified model: ProductModel.cs

- Add `ToUpdateRequest()` method returning a new `UpdateProductRequest`
- Add `static FromResponse(GetProductByIdResponse response)` factory method to populate the model from the API response
- Reference: `IngredientModel` (`FromResponse`, `ToUpdateRequest`)

### Modified service: MenuApiClient.cs

- Add `GetProductByIdForManageAsync(Guid id)` — `GET /api/menu/products/{id}` → `GetProductByIdForManageResponse`
- Add `UpdateProductAsync(Guid id, UpdateProductRequest request)` — `PUT /api/menu/products/{id}` → `UpdateProductResponse`
- Reference: `GetIngredientByIdAsync`, `UpdateIngredientAsync`

### Modified service: MenuNavigationService.cs

- Add `ToProductEdit(Guid id)` navigation method
- Reference: existing `ToIngredientsManagement()` and similar navigation helpers

### New Blazor model/response types

| Type | Location |
|---|---|
| `UpdateProductRequest` | `Features/Menu/Products/Requests/` |
| `UpdateProductResponse` | `Features/Menu/Products/Responses/` |
| `GetProductByIdResponse` | `Features/Menu/Products/Responses/` |

---

## 5) Testing Requirements

### Unit tests

**In scope.** The new `Product.Update()` domain method carries validation logic that must be tested in isolation.

Tests to create or extend in `MyHomeRamen.UnitTests/MenuModule/Products/`:
- `ProductUpdateTests` — new test class mirroring `IngredientUpdateTests`:
  - `Update_Should_UpdateProperties_When_InputIsValid` — verify name, description, price, category, and ingredients are all updated
  - `Update_Should_Throw_When_NameIsEmpty`
  - `Update_Should_Throw_When_PriceIsOutOfRange`
  - `Update_Should_Throw_When_CategoryIsNull`

Reference: `MyHomeRamen.UnitTests/MenuModule/Ingredients/IngredientUpdateTests.cs`

---

### Integration tests

**In scope.** The HTTP endpoint, handler, authorisation rules, and persistence must be tested with a real DB via TestContainers.

Tests to create in `MyHomeRamen.IntegrationTests/MenuModule/Products/UpdateProductTests.cs`:
- `UpdateProduct_ShouldReturnOk_ForValidRequest` — happy path: verify `200 OK`, response body, and persisted changes
- `UpdateProduct_ShouldReturnNotFound_WhenProductDoesNotExist`
- `UpdateProduct_ShouldReturnNotAuthorized_ForNotAuthenticatedUser`
- `UpdateProduct_ShouldReturnForbidden_ForNonAdminUser` (Employee, Customer roles)
- `UpdateProduct_ShouldReturnBadRequest_WhenNameAlreadyExistsOnAnotherProduct`
- `UpdateProduct_ShouldReturnBadRequest_ForInvalidRequest` (missing name, invalid price, missing category, empty ingredients)

Reference: `MyHomeRamen.IntegrationTests/MenuModule/Products/CreateProductTests.cs`, `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/UpdateIngredientTests.cs`
