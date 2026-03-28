Feature implementation plan — Frontend:
- **Date**: 2026-03-28 21:10
- **Feature**: GetCategoriesForDropdown — update `ProductForm.razor` to load categories dynamically from the API instead of receiving them as a static `[Parameter]`
- **Reference feature**: `ProductForm.razor` + `CreateProductPage.razor`

---

## 11) Create frontend feature structure

No new folders or files are needed. All changes are modifications to existing files:

```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/
└── Features/
    └── Menu/
        ├── Common/
        │   └── Services/
        │       └── MenuApiClient.cs                   ← add GetCategoriesForDropdownAsync + response record
        └── Products/
            ├── Components/
            │   └── ProductForm.razor                  ← remove [Parameter] Categories, load internally
            └── CreateProduct/
                └── CreateProductPage.razor            ← remove _categories field + Categories binding
```

---

## 12) Create or update API communication services and API Response model

### MenuApiClient — add GetCategoriesForDropdownAsync

Add the new method to `MenuApiClient.cs` (existing file at `Features/Menu/Common/Services/MenuApiClient.cs`):

```csharp
public async Task<IEnumerable<GetCategoriesForDropdownResponse>> GetCategoriesForDropdownAsync(
    int categoryType,
    CancellationToken ct = default)
{
    IEnumerable<GetCategoriesForDropdownResponse>? result = await httpClient
        .GetFromJsonAsync<IEnumerable<GetCategoriesForDropdownResponse>>(
            $"/api/menu/categories/dropdown?categoryType={categoryType}", ct);

    return result ?? [];
}
```

Add the response record at the bottom of `MenuApiClient.cs`, grouped with the other response records (`CreateProductResponse`, `CreateCategoryResponse`):

```csharp
public sealed record GetCategoriesForDropdownResponse(Guid Id, string Name);
```

**Route note**: The correct API base route for the `Menu` group is `/api/menu` (derived from `GroupName = "Menu"` lowercased). The pattern `categories/dropdown` gives the full URL `/api/menu/categories/dropdown`. This contrasts with the dot-notation `/api/menu.categories` currently used by `CreateCategoryAsync` — the new method uses the confirmed slash-notation route that matches what the API actually registers.

---

## 13) Create or update models, DTOs and mappings

No new Blazor-side request/response records are needed beyond the `GetCategoriesForDropdownResponse` record added in step 12. The response record properties `Id` and `Name` align with the existing `CategoryOption(Guid Id, string Name)` model used by `ProductForm.razor`. A simple projection from `GetCategoriesForDropdownResponse` to `CategoryOption` is done inline in `ProductForm.razor`.

---

## 14) Create or update Blazor components and pages

### ProductForm.razor — remove static Categories parameter, load dynamically

**Changes required:**

1. **Remove** the `[Parameter] public IEnumerable<CategoryOption> Categories { get; set; } = [];` parameter.
2. **Add** a private field `private IEnumerable<CategoryOption> _categories = [];`.
3. **Add** `OnInitializedAsync` lifecycle method that calls `MenuApiClient.GetCategoriesForDropdownAsync` with `CategoryType.Product`.
4. **Update** the `MudSelect` loop to iterate `_categories` (already the field name used in markup — no markup change needed since the existing markup already binds to `Categories` which was the parameter name; this must be updated to `_categories`).
5. **Add** error handling for the API call failure.

**Full updated `@code` block:**
```csharp
@code {
    private MudForm _form = default!;
    private readonly ProductModel _model = new();
    private readonly ProductValidator _validator = new();
    private bool _isBusy;
    private string? _errorMessage;
    private IEnumerable<CategoryOption> _categories = [];

    [Parameter] public FormMode Mode { get; set; } = FormMode.Create;
    [Parameter] public IEnumerable<IngredientOption> Ingredients { get; set; } = [];
    [Parameter] public EventCallback<Guid> OnSuccess { get; set; }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            IEnumerable<GetCategoriesForDropdownResponse> result =
                await MenuApiClient.GetCategoriesForDropdownAsync((int)CategoryType.Product);
            _categories = result.Select(c => new CategoryOption(c.Id, c.Name));
        }
        catch (HttpRequestException)
        {
            _errorMessage = "Failed to load categories. Please refresh the page.";
        }
    }

    private async Task SubmitAsync()
    {
        await _form.Validate();

        if (!_form.IsValid)
        {
            return;
        }

        _isBusy = true;
        _errorMessage = null;

        try
        {
            Guid productId = await MenuApiClient.CreateProductAsync(_model.ToCreateRequest());
            await OnSuccess.InvokeAsync(productId);
        }
        catch (HttpRequestException)
        {
            _errorMessage = "Failed to create product. Please try again.";
        }
        finally
        {
            _isBusy = false;
        }
    }
}
```

**Markup change** — update the `MudSelect` loop from `Categories` to `_categories`:
```razor
<MudSelect Label="Category"
           @bind-Value="_model.CategoryId"
           For="@(() => _model.CategoryId)"
           Variant="Variant.Outlined"
           Disabled="Mode == FormMode.View"
           Required="true">
    @foreach (CategoryOption category in _categories)
    {
        <MudSelectItem Value="@category.Id">@category.Name</MudSelectItem>
    }
</MudSelect>
```

**Using directives to add** at the top of `ProductForm.razor`:
```razor
@using MyHomeRamen.Blazor.Features.Menu.Categories
```
(needed for `CategoryType.Product` — `CategoryType` is defined in `MyHomeRamen.Blazor.Features.Menu.Categories`)

Also add the `GetCategoriesForDropdownResponse` type reference — since it is defined in `MenuApiClient.cs` (same namespace `MyHomeRamen.Blazor.Features.Menu.Common.Services`), no additional `@using` is needed if `MenuApiClient` is already imported.

### CreateProductPage.razor — remove Categories pass-through

**Changes required:**

1. **Remove** the `_categories` field: `private IEnumerable<CategoryOption> _categories = [];`
2. **Remove** the `Categories="_categories"` binding from `<ProductForm>`.
3. **Remove** the TODO comment referencing `GetCategories`.

**Updated `@code` block:**
```csharp
@code {
    private IEnumerable<IngredientOption> _ingredients = [];

    private void HandleSuccess(Guid productId)
    {
        MenuNavigation.ToProductDetail(productId);
    }
}
```

**Updated `<ProductForm>` usage:**
```razor
<ProductForm Mode="FormMode.Create"
             Ingredients="_ingredients"
             OnSuccess="HandleSuccess" />
```

**Using directive to remove** (if it was only used for `CategoryOption`):
- Keep `@using MyHomeRamen.Blazor.Features.Menu.Common.Models` only if still needed for `IngredientOption`.

---

## 15) Create or update Blazor components and pages — scope summary

| File | Type of change |
|---|---|
| `MenuApiClient.cs` | Add `GetCategoriesForDropdownAsync` method + `GetCategoriesForDropdownResponse` record |
| `ProductForm.razor` | Remove `[Parameter] Categories`, add `_categories` field + `OnInitializedAsync` load, update markup loop |
| `CreateProductPage.razor` | Remove `_categories` field + `Categories="_categories"` binding + TODO comment |

---

## 16) Create Unit tests for Blazor components and services

Blazor unit tests should be **skipped** — the Blazor test instructions file (`blazor-tests.instructions.md`) is marked as `TODO` and the test infrastructure for Blazor components is not yet established. Tests will be added when the Blazor testing framework is in place.
