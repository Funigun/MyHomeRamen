# Feature Brief — GetProductsByCategory

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Accessibility** | `Anonymous` |
| **Feature name** | `GetProductsByCategory` |
| **Short backend description** | New `GET /api/menu/products?categoryId={guid}` endpoint that returns all products for a given category. Validates that `categoryId` references an existing `Category` with `CategoryType.Product`. Each product is returned with its base ingredients projected to a flat `ProductIngredientDto`. No authentication required — accessible by anonymous users (e.g. customers browsing the menu). |
| **Short frontend description** | Not in scope. |
| **Reference feature** | `GetIngredientsForDropdown` (anonymous filtered list pattern) · `GetIngredientsForManage` (validated GET with query parameter and `IValidationPolicy`) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | no |

---

## 3) Feature description (Backend scope)

### New API endpoint

- **Route**: `GET /api/menu/products?categoryId={guid}`
- **Authentication**: none — endpoint calls `.AllowAnonymous()` to override group-level `RequireAuthorization()`
- **Authorization**: `Anonymous`
- **Request**: `GetProductsByCategoryRequest` with a single `Guid CategoryId` query parameter
- **Response**: `200 OK` with `IEnumerable<GetProductsByCategoryResponse>`
- **Reference**: `GetIngredientsForDropdownEndpoint`, `GetIngredientsForManageEndpoint`

### Response shape

```
GetProductsByCategoryResponse
├── Guid          Id
├── string        Name
├── string        Description
├── decimal       Price
├── string        ImageUrl
└── IEnumerable<ProductIngredientDto> Ingredients
        ├── Guid   Id
        └── string Name
```

`Ingredients` is mapped from `Product.BaseIngredients` — these are the standard product ingredients shown to the customer.

### Validation (via `GetProductsByCategoryValidator`)

1. `CategoryId` must not be an empty `Guid`
2. The referenced `Category` must exist in the database — DB existence check via a `DbExtension` method on `IQueryable<Category>`
3. The referenced `Category` must have `CategoryType == CategoryType.Product` — ensures ingredient-only categories are not queried

- **Reference**: `GetIngredientsForManageValidator`, `CreateCategoryValidator`

### Persistence changes — Menu module

- New query-shape DB extension: `ForCategory(CategoryId categoryId)` on `DbSet<Product>` in `MyHomeRamen.Persistance.Common.DbExtensions`
  - Uses `AsNoTracking()`
  - Filters products whose `Categories` collection contains the given `categoryId`
  - Handler owns the final projection to `GetProductsByCategoryResponse` via `Mappings`
- New existence check DB extension (if not already present): `CategoryExistsAsync(Guid categoryId)` or similar on `IQueryable<Category>` — consumed by the validator
- **Reference**: `ForDropdown()` and `ForManage()` extensions for `Ingredients`, `IsCategoryNameUniqueAsync()` for existence pattern

### No domain changes

The feature is a pure read operation over existing `Product` and `Category` aggregates. No new domain entities, events, or validators are required.

---

## 4) Feature description (Frontend scope)

Not in scope for this feature.

---

## 5) Testing Requirements

### Unit tests

**Not in scope.** No new domain logic is introduced — the feature is a read projection with input validation. Existing `ProductValidationTests` and `CategoryValidationTests` cover the relevant domain rules.

---

### Integration tests

**In scope.** The HTTP endpoint, handler, persistence query, and validation rules must be tested with a real database via TestContainers.

Tests to create (`MyHomeRamen.IntegrationTests/MenuModule/Products/GetProductsByCategoryTests.cs`):

- `GetProductsByCategory_ShouldReturnOkWithProducts_ForValidProductCategory` — happy path: seeded products in a `CategoryType.Product` category, verify `200 OK` and correct response shape including `Ingredients`
- `GetProductsByCategory_ShouldReturnEmptyList_ForValidCategoryWithNoProducts` — valid `CategoryType.Product` category that has no products assigned, verify `200 OK` with empty list
- `GetProductsByCategory_ShouldReturnBadRequest_ForEmptyCategoryId` — `categoryId` is `Guid.Empty`, verify `400 Bad Request`
- `GetProductsByCategory_ShouldReturnBadRequest_ForNonExistentCategoryId` — `categoryId` is a valid Guid but does not exist in the DB, verify `400 Bad Request`
- `GetProductsByCategory_ShouldReturnBadRequest_ForIngredientCategory` — `categoryId` references a category with `CategoryType.Ingredient`, verify `400 Bad Request`

Reference: `GetIngredientsForDropdownTests`, `GetIngredientsForManageTests`, `CreateProductTests`

---

### Architecture tests

**In scope.** Verify the anonymous endpoint does not inadvertently require authentication and that the persistence extension respects layer boundaries.

Tests to create or extend (`MyHomeRamen.ArchitectureTests/ModuleTests/Menu/`):

- Extend `ApiBoundariesTests` — verify `GetProductsByCategoryEndpoint` is not decorated with a role-based policy (i.e. the endpoint is correctly anonymous)
- Extend `PersistanceBoundariesTests` — verify the new `ForCategory` DB extension does not return API layer types (no DTOs)

Reference: `MyHomeRamen.ArchitectureTests/ModuleTests/Menu/ApiBoundariesTests.cs`, `MyHomeRamen.ArchitectureTests/ModuleTests/Menu/PersistanceBoundariesTests.cs`

---

### System tests

**Not in scope.** The feature has no cross-service flows, background workers, or message broker interactions. Integration tests with TestContainers provide sufficient coverage.

---

## 6) Additional Notes

- **Anonymous override**: `ProductsGroup` currently calls `RequireAuthorization()` at group level. The new endpoint must call `.AllowAnonymous()` explicitly to override this, consistent with how other anonymous endpoints are handled in the codebase.
- **CategoryType validation**: Enforcing `CategoryType.Product` in the validator prevents misuse of ingredient-only categories and acts as a guard against future category type additions.
- **Ingredients source**: `Ingredients` in the response is sourced from `Product.BaseIngredients` only — these represent the fixed product composition visible to customers. Custom ingredients (add-ons/customisations) are intentionally excluded from this public-facing endpoint.
- **No pagination**: For now the endpoint returns all matching products without pagination. A follow-up may add pagination via `PageParameters` once the expected product-per-category volume is known.

---
