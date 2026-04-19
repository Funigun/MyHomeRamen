# Feature Brief — RestaurantMenuPage

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Accessibility** | `Anonymous` |
| **Feature name** | `RestaurantMenuPage` |
| **Short backend description** | Expose a new public `GET /api/menu/categories/menu` endpoint that returns only categories of type `Product` (Id + Name), allowing the frontend to populate the category navigation bar without authentication. The existing `GET /api/menu/products` endpoint (`GetProductsByCategory`) already exists and is public; no backend changes are needed there. |
| **Short frontend description** | Implement the `/menu` Blazor page with a horizontal, single-select category navigation bar at the top. On load, the page fetches all Product-type categories and auto-selects the first one. Selecting a category triggers a request to `GetProductsByCategory` and renders the matching product cards below. |
| **Reference feature** | `GetCategoriesByType` (categories) · `GetProductsByCategory` (products) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes — new anonymous endpoint `GetMenuCategories` |
| `frontend` — Blazor Server / WASM | yes — `RestaurantMenuPage` implementation |

---

## 3) Feature description (Backend scope)

### New flow overview

```
[Anonymous Client]
   │
   ▼ GET /api/menu/categories/menu
[GetMenuCategoriesEndpoint]
   │
   ▼
[GetMenuCategoriesHandler]
   └── queries categories filtered by CategoryType.Product
       returns list of { Id, Name }

[Anonymous Client]
   │
   ▼ GET /api/menu/products?categoryId={id}
[GetProductsByCategoryEndpoint]  ← already exists, no changes needed
```

### New API endpoint — `GetMenuCategories`

- **Route**: `GET /api/menu/categories/menu`
- **Authentication**: none — `AllowAnonymous`
- **Request**: none (no query parameters)
- **Response**: `200 OK` with `IEnumerable<GetMenuCategoriesResponse>` containing `{ Id: Guid, Name: string }`
- **Filter**: only categories where `CategoryType == Product`
- **Reference**: `GetCategoriesByTypeEndpoint`, `GetCategoriesByTypeHandler`

### Why a dedicated endpoint instead of reusing `GetCategoriesByType`

The existing `GetCategoriesByType` endpoint requires `RestaurantManagerPolicy` authorization. The restaurant menu page must be publicly accessible (anonymous), so a new dedicated endpoint is needed that is scoped to `Product` categories only and carries no authorization requirement.

---

## 4) Feature description (Frontend scope)

### Modified page — `RestaurantMenuPage` (`/menu`)

- **Route**: `/menu`
- **Access**: anonymous
- **On load**:
  1. Call `GetMenuCategoriesAsync()` to fetch all Product categories.
  2. Auto-select the first category in the list.
  3. Call `GetProductsByCategoryAsync(categoryId)` for the auto-selected category.
- **Category navigation bar** (top of page):
  - Horizontal, scrollable bar listing all product categories by name.
  - Single selection — selected category is visually highlighted.
  - Clicking a category triggers `GetProductsByCategoryAsync(categoryId)` and replaces the current product list.
- **Product list area** (below navigation bar):
  - Renders a card per product showing: Name, Description, Price, Image, Ingredients
  - Shows a loading indicator while the request is in flight.
  - Shows an empty-state message when no products are returned.

### API client changes — `MenuApiClient`

- Add `GetMenuCategoriesAsync()` method returning `IEnumerable<GetMenuCategoriesResponse>` calling `GET /api/menu/categories/menu` with caching.
- Add `GetProductsByCategoryAsync(Guid categoryId)` method returning `IEnumerable<GetProductsByCategoryResponse>` calling `GET /api/menu/products?categoryId={categoryId}`.
- Add matching Blazor-side response records:
  - `GetMenuCategoriesResponse` — `{ Guid Id, string Name }`
  - `GetProductsByCategoryResponse` — mirrors the API response `{ Guid Id, string Name, string Description, decimal Price, string ImageUrl, IEnumerable<ProductIngredientDto> Ingredients }`
- **Reference**: existing `GetCategoriesByTypeAsync` and `GetIngredientsForDropdownAsync` methods in `MenuApiClient`

---

## 5) Testing Requirements

### Unit tests

**Not in scope.** The new `GetMenuCategoriesHandler` is a straightforward read-only query with no domain logic or validation. No unit tests are justified.

---

### Integration tests

**In scope.** The new endpoint is public and must be verified with a real database via TestContainers.

Tests to create (`GetMenuCategoriesTests`):
- `GetMenuCategories_ShouldReturn_OnlyProductCategories` — seeds categories of both `Product` and `Ingredient` types, calls `GET /api/menu/categories/menu`, asserts only `Product` categories are returned.
- `GetMenuCategories_ShouldReturn_EmptyList_WhenNoProductCategoriesExist` — seeds only `Ingredient` categories, asserts empty list is returned.
- `GetMenuCategories_ShouldReturn_OK_ForAnonymousUser` — no auth token provided, asserts `200 OK` (not `401`).

Reference: `MyHomeRamen.IntegrationTests/MenuModule/Categories/GetCategoriesByTypeTests.cs`, `MyHomeRamen.IntegrationTests/MenuModule/Products/GetProductsByCategoryTests.cs`
