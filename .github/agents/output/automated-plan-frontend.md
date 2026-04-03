# Feature Refactor Plan - Frontend

- **Date**: 2025-07-21
- **Feature**: GetCategoriesByType - Replace CategoriesIndexPage with ProductsManagementPage + IngredientsManagementPage
- **Module**: Menu
- **Type**: Refactor
- **Reference features**: CategoriesIndexPage, CreateCategoryForm, EmployeeLayout

---

## Summary

Replace the single `CategoriesIndexPage` with two dedicated management pages (`ProductsManagementPage` and `IngredientsManagementPage`). Update `CreateCategoryForm` to accept a `CategoryType` parameter instead of showing a dropdown. Update `MenuApiClient` to use the new `GetCategoriesByType` endpoint. Add navigation links in `EmployeeLayout`.

---

## 11) Create frontend feature structure

### New files to create:
```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/
    ProductsManagement/
        ProductsManagementPage.razor
    IngredientsManagement/
        IngredientsManagementPage.razor
```

### Files to modify:
```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/Components/CreateCategoryForm.razor
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/Components/CategoryModel.cs
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/Components/CategoryValidator.cs
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/Components/CategoryTable.razor
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuApiClient.cs
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuNavigationService.cs
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Components/Layout/Employee/EmployeeLayout.razor
```

### Files to remove:
```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/CategoriesIndex/CategoriesIndexPage.razor
```

---

## 12) Create or update API communication services and API Response model

### `MenuApiClient.cs` - Update

**Remove:**
- `GetCategoriesForDropdownAsync(int categoryType)` method
- `GetCategoriesForManageAsync()` method

**Add:**
```csharp
public async Task<IEnumerable<GetCategoriesByTypeResponse>> GetCategoriesByTypeAsync(
    int categoryType,
    CancellationToken ct = default)
{
    IEnumerable<GetCategoriesByTypeResponse>? result = await httpClient
        .GetFromJsonAsync<IEnumerable<GetCategoriesByTypeResponse>>(
            $"/api/menu/categories/by-type?categoryType={categoryType}", ct);

    return result ?? [];
}
```

**Update existing usages:** Any existing calls to `GetCategoriesForDropdownAsync` in other components/pages (e.g., `CreateProductForm`) should be updated to call `GetCategoriesByTypeAsync` instead.

**Add response record** (at bottom of file or in a shared models location):
```csharp
public sealed record GetCategoriesByTypeResponse(Guid Id, string Name, int SortOrder);
```

**Remove response records:**
- `GetCategoriesForDropdownResponse` (if defined in Blazor)
- `GetCategoriesForManageResponse` (if defined in Blazor)
- `CategoryForManageDto` (if defined in Blazor)

### `MenuNavigationService.cs` - Update

**Add routes** to `Routes.Admin`:
```csharp
public const string ProductsManagement = "/admin/menu/products-management";
public const string IngredientsManagement = "/admin/menu/ingredients-management";
```

**Add navigation methods:**
```csharp
public void ToProductsManagement() => navigation.NavigateTo(Routes.Admin.ProductsManagement);
public void ToIngredientsManagement() => navigation.NavigateTo(Routes.Admin.IngredientsManagement);
```

**Remove** (if no longer needed after refactor):
- `CategoriesIndex` route constant (will be replaced)
- `ToAdminCategoriesIndex()` navigation method

---

## 13) Create or update models, DTOs and mappings

### `CategoryModel.cs` - Update

Add `CategoryType` as a constructor parameter instead of a settable property. The form no longer allows the user to pick the category type - it is determined by which management page they are on.

```csharp
public sealed class CategoryModel
{
    public string Name { get; set; } = string.Empty;

    public CategoryType CategoryType { get; set; }

    public CreateCategoryRequest ToCreateRequest()
    {
        return new CreateCategoryRequest(Name, (int)CategoryType);
    }
}
```

**Note:** The `CategoryType` property stays as a settable property for initialization from the parent component, but the dropdown is removed from the form.

### `CategoryValidator.cs` - Update

**Remove** the `RuleFor(x => x.CategoryType)` validation rule since `CategoryType` is now set programmatically by the parent component, not by user input.

```csharp
public sealed class CategoryValidator : BaseValidator<CategoryModel>
{
    public CategoryValidator()
    {
        RuleFor(x => x.Name)
            .SetValidator(new CategoryNameValidator());
    }
}
```

---

## 14) Create or update Blazor components and pages

### `CreateCategoryForm.razor` - Update

**Changes:**
1. Add `[Parameter] public CategoryType CategoryType { get; set; }` parameter
2. Remove `MudSelect` dropdown for `CategoryType`
3. Set `_model.CategoryType = CategoryType` in `OnParametersSet`

**Updated component structure:**
```razor
@code {
    [Parameter] public CategoryType CategoryType { get; set; }
    [Parameter] public EventCallback<Guid> OnSuccess { get; set; }

    protected override void OnParametersSet()
    {
        _model.CategoryType = CategoryType;
    }
    
    // ... rest of submit logic stays the same
}
```

The `MudSelect` for `CategoryType` is removed from the form template. The category type is determined by the parent page.

### `CategoryTable.razor` - Update

**Change:** Update `CategoryForManageDto` references to use `GetCategoriesByTypeResponse` since the old DTO is removed.

```razor
[Parameter, EditorRequired] public List<GetCategoriesByTypeResponse> Items { get; set; } = default!;
```

Update `MudDropContainer` type parameter accordingly.

### `ProductsManagementPage.razor` - NEW

```
@page "/admin/menu/products-management"
```

**Pattern:** Follow `CategoriesIndexPage.razor` pattern but scoped to `CategoryType.Product`:
- Load categories via `MenuApiClient.GetCategoriesByTypeAsync((int)CategoryType.Product)`
- Pass `CategoryType.Product` to `CreateCategoryForm`
- Display single `CategoryTable` for product categories

### `IngredientsManagementPage.razor` - NEW

```
@page "/admin/menu/ingredients-management"
```

**Pattern:** Same as `ProductsManagementPage` but scoped to `CategoryType.Ingredient`:
- Load categories via `MenuApiClient.GetCategoriesByTypeAsync((int)CategoryType.Ingredient)`
- Pass `CategoryType.Ingredient` to `CreateCategoryForm`
- Display single `CategoryTable` for ingredient categories

### `EmployeeLayout.razor` - Update

**Change:** Add child navigation links under `Menu Management` section using `MudNavGroup`:

```razor
<MudNavMenu Rounded="true" Margin="Margin.Dense" Color="Color.Primary" Class="mud-width-full pa-2" Style="width: 230px">
    <MudNavGroup Title="Menu Management" Icon="@Icons.Material.Filled.RestaurantMenu" IconColor="Color.Secondary">
        <MudNavLink Href="@MenuNavigationService.Routes.Admin.ProductsManagement">
            Products Management
        </MudNavLink>
        <MudNavLink Href="@MenuNavigationService.Routes.Admin.IngredientsManagement">
            Ingredients Management
        </MudNavLink>
    </MudNavGroup>
</MudNavMenu>
```

**Add:** `@using MyHomeRamen.Blazor.Features.Menu.Common.Services` import to access `MenuNavigationService.Routes`.

---

## 15) Update dependent components

### Check for `GetCategoriesForDropdownAsync` usage in other components

Search for any calls to `GetCategoriesForDropdownAsync` in Blazor components (e.g., `CreateProductForm`, `EditProductForm`). These must be updated to call `GetCategoriesByTypeAsync` instead.

The response shape changes:
- Old: `GetCategoriesForDropdownResponse(Guid Id, string Name)`
- New: `GetCategoriesByTypeResponse(Guid Id, string Name, int SortOrder)`

Update any model mappings that map dropdown response to local models.

---

## 16) Unit tests for Blazor components and services

**No Blazor unit tests required.** Per feature brief, only integration tests are required. The `blazor-tests.instructions.md` is currently `TODO`.
