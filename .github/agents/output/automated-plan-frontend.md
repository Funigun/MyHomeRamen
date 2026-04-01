# Feature Implementation Plan — Frontend

- **Date**: 2025-07-17
- **Feature**: GetCategoriesForManage
- **Module**: Menu
- **Reference**: CategoriesIndexPage.razor, GetCategoriesForDropdown Blazor integration

---

## 11) Create frontend feature structure

No new feature folders needed — this feature updates the existing `CategoriesIndex` page and the shared `MenuApiClient`.

Existing structure remains:
```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/
??? Categories/
?   ??? CategoriesIndex/
?   ?   ??? CategoriesIndexPage.razor          ? UPDATE: replace placeholder alerts with category tables
?   ??? Components/
?       ??? (existing: CreateCategoryForm.razor, CategoryModel.cs, CategoryValidator.cs)
??? Common/
?   ??? Services/
?   ?   ??? MenuApiClient.cs                   ? UPDATE: add GetCategoriesForManageAsync method + response records
```

---

## 12) Create or update API communication services and API Response model

### `MenuApiClient.cs` — add `GetCategoriesForManageAsync` method

Add to `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuApiClient.cs`:

```csharp
public async Task<GetCategoriesForManageResponse> GetCategoriesForManageAsync(CancellationToken ct = default)
{
    GetCategoriesForManageResponse? result = await httpClient
        .GetFromJsonAsync<GetCategoriesForManageResponse>(
            "/api/menu/categories/manage", ct);

    return result ?? new GetCategoriesForManageResponse([], []);
}
```

Add response records at bottom of file (following existing pattern with other response records):

```csharp
public sealed record GetCategoriesForManageResponse(
    IEnumerable<CategoryForManageDto> ProductCategories,
    IEnumerable<CategoryForManageDto> IngredientCategories);

public sealed record CategoryForManageDto(Guid Id, string Name, int SortOrder);
```

---

## 13) Create or update models, DTOs and mappings

**No new UI models or DTOs needed** beyond the API response records added to `MenuApiClient.cs` in step 12. The page directly consumes the API response for display.

---

## 14) Create or update Blazor components and pages

### `CategoriesIndex/CategoriesIndexPage.razor` — UPDATE

Replace the placeholder `<MudAlert>` sections with data-driven category tables. The page should:

#### Data loading
- Inject `MenuApiClient`
- In `OnInitializedAsync`, call `MenuApiClient.GetCategoriesForManageAsync()` to load both category lists
- Store results in `_productCategories` and `_ingredientCategories` fields (type `List<CategoryForManageDto>`)
- Show a loading indicator while data is being fetched

#### Product Categories table
Replace the "Category list coming soon." alert under **Product Categories** with:
- `<MudTable>` bound to `_productCategories`
- Columns:
  | Column | Header | Source |
  |---|---|---|
  | LP | `#` | Row index (1-based) |
  | Name | `Name` | `category.Name` |
  | Actions | `Actions` | Edit + Delete icon buttons |
- Edit button: `<MudIconButton Icon="@Icons.Material.Filled.Edit">` — placeholder (no action yet)
- Delete button: `<MudIconButton Icon="@Icons.Material.Filled.Delete" Color="Color.Error">` — placeholder (no action yet)
- Table should support drag-and-drop reordering (use `MudDropContainer` or `AllowReorder` if available in MudBlazor, otherwise note as TODO)

#### Ingredient Categories table
Same structure as Product Categories table, bound to `_ingredientCategories`.

#### Reordering support
- Both tables should allow reordering via drag-and-drop or up/down buttons
- Implementation options (choose based on MudBlazor capabilities):
  - **Option A**: Use `<MudDropContainer>` with `<MudDropZone>` and `<MudDynamicDropItem>` for drag-and-drop
  - **Option B**: Add Up/Down `<MudIconButton>` in each row to move items manually
- Reordering is **visual only** for now — no API call to persist order changes (that would be a separate feature: `UpdateCategorySortOrder`)

#### Error handling
- Wrap API call in try-catch for `HttpRequestException`
- Display `<MudAlert Severity="Severity.Error">` if the API call fails

#### Success message
- Keep existing `OnCategoryCreated` callback — after category creation, refresh the category lists by calling `GetCategoriesForManageAsync` again

#### Component structure (pseudo-code)

```razor
@page "/admin/menu/categories"
@attribute [Authorize(Roles = MenuRoleConstants.Admin)]

@using MyHomeRamen.Blazor.Features.Menu.Categories.Components
@using MyHomeRamen.Blazor.Features.Menu.Common.Constants
@using MyHomeRamen.Blazor.Features.Menu.Common.Services

@inject MenuApiClient MenuApiClient

<PageTitle>Category Management</PageTitle>

<MudPaper Elevation="3" Class="pa-6">
    <MudText Typo="Typo.h4" Class="mb-6">Category Management</MudText>

    <CreateCategoryForm OnSuccess="OnCategoryCreated" />

    @if (_successMessage is not null)
    {
        <MudAlert Severity="Severity.Success" Class="mt-4" Dense="true">@_successMessage</MudAlert>
    }

    @if (_errorMessage is not null)
    {
        <MudAlert Severity="Severity.Error" Class="mt-4" Dense="true">@_errorMessage</MudAlert>
    }

    <MudDivider Class="my-6" />

    <MudText Typo="Typo.h5">Product Categories</MudText>
    @if (_isLoading)
    {
        <MudProgressLinear Indeterminate="true" Class="mt-2" />
    }
    else if (_productCategories.Count == 0)
    {
        <MudAlert Severity="Severity.Info" Class="mt-2">No product categories found.</MudAlert>
    }
    else
    {
        <MudTable Items="_productCategories" Dense="true" Hover="true" Class="mt-2">
            <HeaderContent>
                <MudTh>#</MudTh>
                <MudTh>Name</MudTh>
                <MudTh>Actions</MudTh>
            </HeaderContent>
            <RowTemplate>
                <MudTd>@((_productCategories.IndexOf(context) + 1))</MudTd>
                <MudTd>@context.Name</MudTd>
                <MudTd>
                    <MudIconButton Icon="@Icons.Material.Filled.Edit" Size="Size.Small" />
                    <MudIconButton Icon="@Icons.Material.Filled.Delete" Size="Size.Small" Color="Color.Error" />
                </MudTd>
            </RowTemplate>
        </MudTable>
    }

    <MudDivider Class="my-6" />

    <MudText Typo="Typo.h5">Ingredient Categories</MudText>
    @if (_isLoading)
    {
        <MudProgressLinear Indeterminate="true" Class="mt-2" />
    }
    else if (_ingredientCategories.Count == 0)
    {
        <MudAlert Severity="Severity.Info" Class="mt-2">No ingredient categories found.</MudAlert>
    }
    else
    {
        <MudTable Items="_ingredientCategories" Dense="true" Hover="true" Class="mt-2">
            <!-- Same structure as Product Categories table -->
        </MudTable>
    }
</MudPaper>

@code {
    private List<CategoryForManageDto> _productCategories = [];
    private List<CategoryForManageDto> _ingredientCategories = [];
    private bool _isLoading = true;
    private string? _successMessage;
    private string? _errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadCategoriesAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        _isLoading = true;
        _errorMessage = null;
        try
        {
            var response = await MenuApiClient.GetCategoriesForManageAsync();
            _productCategories = response.ProductCategories.ToList();
            _ingredientCategories = response.IngredientCategories.ToList();
        }
        catch (HttpRequestException)
        {
            _errorMessage = "Failed to load categories. Please try again.";
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task OnCategoryCreated(Guid id)
    {
        _successMessage = "Category created successfully.";
        await LoadCategoriesAsync();
    }
}
```

#### Reordering implementation notes

For reordering, add Up/Down icon buttons in the Actions column:
- `<MudIconButton Icon="@Icons.Material.Filled.ArrowUpward">` — swap with previous item
- `<MudIconButton Icon="@Icons.Material.Filled.ArrowDownward">` — swap with next item
- Disable Up on first item, Down on last item
- Reordering updates the local list only (visual) — persisting order requires a separate `UpdateCategorySortOrder` API endpoint (future feature)

---

## 15) Unit tests for Blazor components and services

**No Blazor unit tests required.** The Blazor test instructions are marked as `TODO` and no testing framework is established for Blazor components yet.
