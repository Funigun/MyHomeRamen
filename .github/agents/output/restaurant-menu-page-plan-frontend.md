# Feature Implementation Plan — RestaurantMenuPage (Frontend)

- **Date**: 2025-07-14
- **Feature**: RestaurantMenuPage — `/menu` Blazor page with category navigation and product cards

---

## 11) Create frontend feature structure

No new folder needed — `RestaurantMenuPage.razor` already exists at:
`MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/RestaurantMenuPage.razor`

New files to create:

```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/
├── RestaurantMenuPage.razor              ← MODIFY (implement full page)
├── RestaurantMenu/
│   ├── Responses/
│   │   ├── GetMenuCategoriesResponse.cs  ← CREATE
│   │   └── GetProductsByCategoryResponse.cs  ← CREATE
│   └── Components/
│       ├── CategoryNavBar.razor          ← CREATE
│       └── ProductCard.razor             ← CREATE
```

---

## 12) Create or update API communication services and API Response model

### Modify: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuApiClient.cs`

Add two new methods:

1. **`GetMenuCategoriesAsync()`**
   - Returns `IEnumerable<GetMenuCategoriesResponse>`
   - Calls `GET /api/menu/categories/menu`
   - Apply caching (reference: existing `GetCategoriesByTypeAsync` caching pattern)

2. **`GetProductsByCategoryAsync(Guid categoryId)`**
   - Returns `IEnumerable<GetProductsByCategoryResponse>`
   - Calls `GET /api/menu/products?categoryId={categoryId}`
   - Reference: existing API client methods in `MenuApiClient`

---

## 13) Create or update models, DTOs and mappings

### Create: `RestaurantMenu/Responses/GetMenuCategoriesResponse.cs`
- Record: `Guid Id`, `string Name`

### Create: `RestaurantMenu/Responses/GetProductsByCategoryResponse.cs`
- Record: `Guid Id`, `string Name`, `string Description`, `decimal Price`, `string ImageUrl`, `IEnumerable<ProductIngredientDto> Ingredients`
- Include `ProductIngredientDto` record if not already shared: `Guid Id`, `string Name`

---

## 14) Create or update Blazor components and pages

### Create: `RestaurantMenu/Components/CategoryNavBar.razor`
- **Parameters**: `IEnumerable<GetMenuCategoriesResponse> Categories`, `Guid SelectedCategoryId`, `EventCallback<Guid> OnCategorySelected`
- Renders a horizontal, scrollable bar of category buttons
- Highlights the currently selected category
- Clicking a category invokes `OnCategorySelected`

### Create: `RestaurantMenu/Components/ProductCard.razor`
- **Parameter**: `GetProductsByCategoryResponse Product`
- Displays: Name, Description, Price (formatted), Image, Ingredients list
- Styled as a card component

### Modify: `RestaurantMenuPage.razor`
- **Route**: `@page "/menu"`
- **Access**: anonymous (no `[Authorize]` attribute)
- **State**: `categories` list, `selectedCategoryId`, `products` list, `isLoading` flag
- **OnInitializedAsync**:
  1. Fetch categories via `MenuApiClient.GetMenuCategoriesAsync()`
  2. Auto-select first category
  3. Fetch products for selected category via `MenuApiClient.GetProductsByCategoryAsync(categoryId)`
- **OnCategorySelected handler**:
  1. Set `isLoading = true`
  2. Update `selectedCategoryId`
  3. Fetch products for new category
  4. Set `isLoading = false`
- **Render**:
  - `<CategoryNavBar>` at top
  - Loading indicator when `isLoading`
  - Grid/list of `<ProductCard>` components
  - Empty-state message when no products returned

---

## 15) Create Unit tests for Blazor components and services

Not in scope per brief — frontend tests not explicitly requested. Consider adding component tests for `CategoryNavBar` and `ProductCard` as a follow-up if needed.
