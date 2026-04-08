# Feature Brief — GetProductsForManage

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` |
| **Module** | `Menu` |
| **Accessibility** | `Manager` |
| **Feature name** | `GetProductsForManage` |
| **Short backend description** | New `GET /api/menu/products/manage` endpoint that returns a paged, filtered, and sorted list of products for the admin management view. Supports filters by product name (contains, case-insensitive), category IDs, ingredient IDs (matched against both base and custom ingredients), and price range (from/to). Supports ordering by product name or price, defaulting to product name. Response DTO includes id, name, description, and price. Requires a new `ForManage` DB extension on `DbSet<Product>`. |
| **Short frontend description** | New `ProductTable.razor` component (mirroring `IngredientTable.razor`) that renders the paged product list with Edit and Delete actions. Update `ProductsManagementPage.razor` to load and display products using the new table, following the `IngredientsManagementPage.razor` pattern. Extend `MenuApiClient` with `GetProductsForManageAsync`. Add a `ProductTableModel` view model and a `GetProductsForManageResponse` Blazor-side response record. |
| **Reference feature** | `GetIngredientsForManage` (Menu module) · `IngredientsManagementPage` (Blazor) · `IngredientTable.razor` (Blazor) |

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
   ▼ GET /api/menu/products/manage?name=...&categoryIds=...&ingredientIds=...&priceFrom=...&priceTo=...&orderBy=...&pageNumber=...&pageSize=...
[GetProductsForManageEndpoint]
   │ validates via GetProductsForManageValidator
   ▼
[GetProductsForManageHandler]
   ├── applies ForManage DB extension on DbSet<Product>
   │     ├── filter by name (contains, case-insensitive)
   │     ├── filter by categoryIds
   │     ├── filter by ingredientIds (BaseIngredients OR CustomIngredients)
   │     └── filter by price range (priceFrom / priceTo)
   ├── counts total matching products
   ├── applies ordering (name or price; default name)
   ├── applies paging (Paged extension)
   └── projects to ProductDto (Id, Name, Description, Price)
          │
          ▼
[GetProductsForManageResponse (Page, PageSize, TotalCount, Products)]
```

### New API endpoint

- **Endpoint**: `GET /api/menu/products/manage`
- **Authentication**: required (Bearer token via Keycloak)
- **Authorization**: `Manager` role (`RestaurantManagerPolicy`)
- **Query parameters**:
  - `name` — optional, string, partial match (contains)
  - `categoryIds` — optional, list of `Guid`
  - `ingredientIds` — optional, list of `Guid`; matched against both `BaseIngredients` and `CustomIngredients`
  - `priceFrom` — optional, decimal
  - `priceTo` — optional, decimal
  - `orderBy` — optional, enum: `Name` (default), `Price`
  - `pageNumber`, `pageSize` — paging via existing `PageParameters`
- **Response**: `200 OK` — `GetProductsForManageResponse`
- **Reference**: `GetIngredientsForManageEndpoint`, `GetIngredientsForManageHandler`

### New files (API layer — `MyHomeRamen.Api/Menu/Features/Products/GetProductsForManage/`)

| File | Description |
|---|---|
| `GetProductsForManageEndpoint.cs` | Maps `GET products/manage`, wires `[AsParameters]` request + `PageParameters` |
| `GetProductsForManageHandler.cs` | Queries `IMenuDbContext.Products` via `ForManage`, counts, orders, pages, projects |
| `Models/GetProductsForManageRequest.cs` | `Name?`, `CategoryIds?`, `IngredientIds?`, `PriceFrom?`, `PriceTo?`, `OrderBy?`, `PageParameters` |
| `Models/GetProductsForManageResponse.cs` | `Page`, `PageSize`, `TotalCount`, `IEnumerable<ProductDto>` |
| `Models/ProductDto.cs` | `Id`, `Name`, `Description`, `Price` |
| `Models/Mappings.cs` | Projection expression `Product → ProductDto` |
| `Policies/GetProductsForManageValidator.cs` | Name max length, price range sanity (priceFrom ≤ priceTo), positive values |

### Persistence changes — `MyHomeRamen.Persistance/Common/DbExtensions.cs`

- New `ForManage` extension on `DbSet<Product>` accepting name, categoryIds, ingredientIds, priceFrom, priceTo, and orderBy
- Ingredient filter must check both `BaseIngredients` and `CustomIngredients` navigation properties
- Reference: existing `ForManage` extension on `DbSet<Ingredient>`

---

## 4) Feature description (Frontend scope)

### New component — `ProductTable.razor`

- **Path**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Components/ProductTable.razor`
- **Pattern**: mirrors `IngredientTable.razor`
- **Columns**: Name, Description, Price, Actions (Edit, Delete)
- **Parameters**: `Items` (`List<ProductTableModel>`), `IsLoading`, `Paging` (`PageState`), `OnPageChanged`, `OnEdit`, `OnDelete`
- **Delete confirmation**: same `DialogService.ShowMessageBoxAsync` pattern as `IngredientTable.razor`

### New view model — `ProductTableModel.cs`

- **Path**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Components/ProductTableModel.cs`
- Properties: `Id` (`Guid`), `Name`, `Description`, `Price` (`decimal`)
- Static factory: `FromResponse(ProductForManageItemResponse response)`
- **Reference**: `IngredientTableModel.cs`

### New Blazor-side response records

- **Path**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Responses/`
- `GetProductsForManageResponse.cs` — `Page`, `PageSize`, `TotalCount`, `IEnumerable<ProductForManageItemResponse>`
- `ProductForManageItemResponse.cs` — `Id`, `Name`, `Description`, `Price`
- **Reference**: `GetIngredientsForManageResponse.cs`, `IngredientForManageItemResponse.cs`

### Updated page — `ProductsManagementPage.razor`

- **Path**: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/ProductsManagementPage.razor`
- Add product list state: `_products` (`List<ProductTableModel>`), `_isProductsLoading`, `_productsPaging` (`PageState`)
- Load products on `OnInitializedAsync` in parallel with categories (pattern: `Task.WhenAll`)
- New private methods: `LoadProductsAsync`, `OnProductPageChangedAsync`, `OnEditProduct`, `OnProductDeletedAsync`
- `OnEditProduct` navigates via `MenuNavigationService.ToEditProduct(id)` (already exists)
- Wire `ProductTable` component below the categories section, following the `IngredientsManagementPage.razor` structure
- **Reference**: `IngredientsManagementPage.razor`

### Updated service — `MenuApiClient.cs`

- New method: `GetProductsForManageAsync` accepting optional `name`, `categoryIds`, `ingredientIds`, `priceFrom`, `priceTo`, `orderBy`, `pageNumber`, `pageSize`
- Builds query string using the same `List<string> queryParts` pattern as `GetIngredientsForManageAsync`
- Returns `GetProductsForManageResponse?`
- **Reference**: `GetIngredientsForManageAsync` in `MenuApiClient.cs`

---

## 5) Testing Requirements

### Unit tests

**In scope.** The validator carries range logic (priceFrom ≤ priceTo, positive price bounds) that benefits from isolated verification.

Tests to create:
- `GetProductsForManageValidatorTests` — valid request passes; negative price fails; priceFrom > priceTo fails; name exceeding max length fails
- **Reference**: `MyHomeRamen.UnitTests/MenuModule/Ingredients/GetIngredientsForManageValidatorTests.cs` (if it exists) or any existing `*ValidatorTests.cs` in the `MenuModule` unit-test folder

### Integration tests

**In scope.** The HTTP endpoint, handler, DB filters, authorization, and paging must be exercised with a real DB via TestContainers.

Tests to create:
- `GetProductsForManage_ShouldReturnOk_ForAuthenticatedAdmin` — returns `200 OK` with non-empty product list
- `GetProductsForManage_ShouldReturnUnauthorized_ForUnauthenticatedUser`
- `GetProductsForManage_ShouldReturnForbidden_ForNonAdminRole` (`[Theory]` with Employee, Customer roles)
- `GetProductsForManage_ShouldReturnFilteredResults_ByName`
- `GetProductsForManage_ShouldReturnFilteredResults_ByCategoryId`
- `GetProductsForManage_ShouldReturnFilteredResults_ByIngredientId` (verifies match in both base and custom ingredients)
- `GetProductsForManage_ShouldReturnFilteredResults_ByPriceRange`
- `GetProductsForManage_ShouldReturnPagedResults`
- **Reference**: `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/GetIngredientsForManageTests.cs`

---
