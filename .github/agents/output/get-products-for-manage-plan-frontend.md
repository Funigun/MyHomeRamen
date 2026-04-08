# Feature plan — GetProductsForManage (Frontend)

- **Date**: 2025-07-15
- **Feature**: GetProductsForManage — Blazor ProductTable component, ProductsManagementPage updates, API client, response types
- **Reference**: `IngredientTable.razor`, `IngredientsManagementPage.razor`, `IngredientTableModel.cs`

---

## 11) Create frontend feature structure

New files to create:
```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/
├── Components/
│   ├── ProductTable.razor                               ← NEW
│   └── ProductTableModel.cs                             ← NEW
├── Responses/
│   ├── GetProductsForManageResponse.cs                  ← NEW
│   └── ProductForManageItemResponse.cs                  ← NEW
├── ProductsManagementPage.razor                         ← MODIFY
```

---

## 14) Create or update API communication services and API Response model

### New file: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Responses/GetProductsForManageResponse.cs`
- `public sealed record GetProductsForManageResponse(int Page, int PageSize, int TotalCount, IEnumerable<ProductForManageItemResponse> Products)`
- Reference: `GetIngredientsForManageResponse` (Blazor)

### New file: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Responses/ProductForManageItemResponse.cs`
- `public sealed record ProductForManageItemResponse(Guid Id, string Name, string Description, decimal Price)`
- Reference: `IngredientForManageItemResponse`

### Update: `MenuApiClient.cs`
- Add method:
  ```csharp
  public async Task<GetProductsForManageResponse?> GetProductsForManageAsync(
      string? name = null,
      IEnumerable<Guid>? categoryIds = null,
      IEnumerable<Guid>? ingredientIds = null,
      decimal? priceFrom = null,
      decimal? priceTo = null,
      string? orderBy = null,
      int pageNumber = 1,
      int pageSize = 10,
      CancellationToken ct = default)
  ```
  - Builds query string with `List<string> queryParts` pattern
  - URL: `/api/menu/products/manage?...`
  - Returns `GetProductsForManageResponse?`
- Reference: `GetIngredientsForManageAsync` in `MenuApiClient.cs`

---

## 14) Create or update models, DTOs and mappings

### New file: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Components/ProductTableModel.cs`
- `public sealed class ProductTableModel`
- Properties: `Guid Id { get; init; }`, `string Name { get; init; }`, `string Description { get; init; }`, `decimal Price { get; init; }`
- Static factory: `public static ProductTableModel FromResponse(ProductForManageItemResponse response)`
  ```csharp
  return new ProductTableModel
  {
      Id = response.Id,
      Name = response.Name,
      Description = response.Description,
      Price = response.Price,
  };
  ```
- Reference: `IngredientTableModel.cs`

---

## 15) Create or update Blazor components and pages

### New component: `ProductTable.razor`
- Path: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Components/ProductTable.razor`
- Parameters:
  - `[Parameter, EditorRequired] public List<ProductTableModel> Items { get; set; } = default!;`
  - `[Parameter] public bool IsLoading { get; set; }`
  - `[Parameter] public PageState Paging { get; set; } = PageState.Default();`
  - `[Parameter] public EventCallback<int> OnPageChanged { get; set; }`
  - `[Parameter] public EventCallback<Guid> OnEdit { get; set; }`
  - `[Parameter] public EventCallback<Guid> OnDelete { get; set; }`
- Columns: Name, Description, Price, Actions (Edit, Delete)
- Price column: display formatted as currency (e.g. `$@item.Price.ToString("F2")`)
- Delete confirmation: `DialogService.ShowMessageBoxAsync` pattern (same as `IngredientTable.razor`)
- Pagination: `MudPagination` bound to `Paging.TotalPages` / `Paging.CurrentPage`, rendered when `Paging.TotalPages > 1`
- Loading state: `MudProgressLinear` when `IsLoading`
- Empty state: `MudAlert` with "No products found." when `Items.Count == 0`
- Reference: `IngredientTable.razor`

### Update: `ProductsManagementPage.razor`
- Add `@using` statements for product components and responses:
  - `MyHomeRamen.Blazor.Features.Menu.Products.Components`
  - `MyHomeRamen.Blazor.Features.Menu.Products.Responses`
  - `MyHomeRamen.Blazor.Common.Models`
- Add inject: `@inject MenuNavigationService MenuNavigation`
- Add state fields:
  - `private List<ProductTableModel> _products = [];`
  - `private bool _isProductsLoading = true;`
  - `private PageState _productsPaging = PageState.Default();`
- Update `OnInitializedAsync` to load both categories and products in parallel:
  ```csharp
  await Task.WhenAll(LoadCategoriesAsync(), LoadProductsAsync());
  ```
- Add private methods:
  - `LoadProductsAsync()` — calls `MenuApiClient.GetProductsForManageAsync(pageNumber, pageSize)`, maps to `ProductTableModel.FromResponse`, updates `_productsPaging`
  - `OnProductPageChangedAsync(int page)` — updates `_productsPaging` with new page, calls `LoadProductsAsync`
  - `OnEditProduct(Guid id)` — calls `MenuNavigation.ToEditProduct(id)`
  - `OnProductDeletedAsync(Guid id)` — placeholder (delete not yet implemented; show success/error messages)
- Add markup below categories section:
  ```razor
  <MudDivider Class="my-6" />

  <MudStack Row="true">
      <MudText Typo="Typo.h5" Class="mb-4" Style="margin: 0px !important; align-content: center">Products</MudText>
      <MudIconButton Icon="@Icons.Material.Filled.Add" OnClick="OnAddProduct" />
  </MudStack>

  <ProductTable Items="_products"
                IsLoading="_isProductsLoading"
                Paging="_productsPaging"
                OnPageChanged="OnProductPageChangedAsync"
                OnEdit="OnEditProduct"
                OnDelete="OnProductDeletedAsync" />
  ```
- Add `OnAddProduct()` method → `MenuNavigation.ToCreateProduct()`
- Reference: `IngredientsManagementPage.razor`

---

## 16) Create Unit tests for Blazor components and services

Skip — Blazor test instructions are `TODO`.
