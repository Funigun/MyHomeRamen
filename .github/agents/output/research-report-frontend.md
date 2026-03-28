# Research Report — Frontend

- **Date**: 2025-07-17
- **Task**: GetCategoriesForDropdown
- **Module**: Menu
- **Reference feature**: ProductForm.razor + CreateProductPage.razor

---

## 1) Reference Implementation Map

### ProductForm + CreateProductPage — File Inventory

| Layer | File | Purpose |
|---|---|---|
| Page Component | `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/CreateProduct/CreateProductPage.razor` | Hosts `ProductForm`, passes `_categories` and `_ingredients` as parameters |
| Form Component | `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Components/ProductForm.razor` | Form with `[Parameter] IEnumerable<CategoryOption> Categories` — currently a static param |
| API Service | `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuApiClient.cs` | `HttpClient`-based service with `CreateProductAsync`, `CreateCategoryAsync` methods |
| CategoryOption Model | `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Models/CategoryOption.cs` | `sealed record CategoryOption(Guid Id, string Name)` |
| Blazor CategoryType | `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/CategoryType.cs` | Mirrors Domain `CategoryType` enum: `Product = 1`, `Ingredient = 2` |
| CreateCategory Request | `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/CreateCategory/CreateCategoryRequest.cs` | `sealed record CreateCategoryRequest(string Name, int CategoryType)` |

### Key Code Patterns

#### Current ProductForm Category Parameter
```razor
// ProductForm.razor, lines 101-102
[Parameter] public IEnumerable<CategoryOption> Categories { get; set; } = [];

// In CreateProductPage.razor, line 24-25:
// TODO: Load from MenuApiClient once GetCategories / GetIngredients endpoints exist
private IEnumerable<CategoryOption> _categories = [];
```

#### MenuApiClient HTTP Call Pattern
```csharp
// Extracted from MenuApiClient.cs, lines 17-24
public async Task<Guid> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken ct = default)
{
    using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/menu.categories", request, ct);
    response.EnsureSuccessStatusCode();

    CreateCategoryResponse? result = await response.Content.ReadFromJsonAsync<CreateCategoryResponse>(ct);
    return result?.Id ?? throw new InvalidOperationException("Failed to deserialize category creation response.");
}
```

> **Note**: The new `GetCategoriesForDropdownAsync` method is a GET with a query parameter. Use `httpClient.GetFromJsonAsync<IEnumerable<GetCategoriesForDropdownResponse>>($"/api/menu/categories/dropdown?categoryType={categoryType}", ct)`.

#### Form Submit Pattern (for reference)
```csharp
// ProductForm.razor, lines 117-125
try
{
    Guid productId = await MenuApiClient.CreateProductAsync(_model.ToCreateRequest());
    await OnSuccess.InvokeAsync(productId);
}
catch (HttpRequestException)
{
    _errorMessage = "Failed to create product. Please try again.";
}
```

---

## 2) Conventions Discovered

| Convention | Example | Location |
|---|---|---|
| `MenuApiClient` is a single class for all Menu API calls | `CreateProductAsync`, `CreateCategoryAsync` | `MenuApiClient.cs` |
| Response records defined at end of `MenuApiClient.cs` | `public sealed record CreateCategoryResponse(Guid Id)` | `MenuApiClient.cs` |
| Blazor has its own request/response records, not shared with API | `CreateCategoryRequest` in Blazor project | `CreateCategoryRequest.cs` |
| Blazor has its own `CategoryType` enum mirroring Domain | `CategoryType.Product = 1, Ingredient = 2` | `CategoryType.cs` |
| `CategoryOption` is the shared Blazor model for dropdown items | `sealed record CategoryOption(Guid Id, string Name)` | `CategoryOption.cs` |
| Form components use `[Parameter]` for data; pages own lifecycle | `IEnumerable<CategoryOption> Categories` param | `ProductForm.razor` |
| Pages load data in `OnInitializedAsync`, not in form components | `CreateProductPage` has the TODO showing this intent | `CreateProductPage.razor` |
| MudBlazor `MudSelect` used for dropdowns | `<MudSelect @bind-Value="_model.CategoryId">` | `ProductForm.razor` |
| `HttpRequestException` caught on API failure, sets `_errorMessage` | `catch (HttpRequestException)` | `ProductForm.razor` |

---

## 3) Common Utilities Available

| Utility | Purpose | Namespace |
|---|---|---|
| `MenuApiClient` | Central HTTP client for all Menu API calls | `MyHomeRamen.Blazor.Features.Menu.Common.Services` |
| `CategoryOption` | Shared record for category dropdowns `(Guid Id, string Name)` | `MyHomeRamen.Blazor.Features.Menu.Common.Models` |
| `CategoryType` (Blazor) | Enum `Product = 1, Ingredient = 2` | `MyHomeRamen.Blazor.Features.Menu.Categories` |
| `MenuNavigationService` | Navigation helpers for menu routes | `MyHomeRamen.Blazor.Features.Menu.Common.Services` |

---

## 4) Architecture Boundaries

### Existing tests for Menu Frontend
- None identified in `MyHomeRamen.BlazorTests` for Menu module (project exists but no Menu tests found).

### New boundaries needed
- None identified.

---

## 5) Planned Changes for Frontend

| Component | Change |
|---|---|
| `MenuApiClient.cs` | Add `GetCategoriesForDropdownAsync(int categoryType, CancellationToken ct)` method returning `IEnumerable<GetCategoriesForDropdownResponse>`. Add `GetCategoriesForDropdownResponse` record at bottom of file. |
| `ProductForm.razor` | Remove `[Parameter] IEnumerable<CategoryOption> Categories` — replace with `private IEnumerable<CategoryOption> _categories = []` and load via `OnInitializedAsync` calling `MenuApiClient.GetCategoriesForDropdownAsync((int)CategoryType.Product)`. Inject `MenuApiClient` (already injected). |
| `CreateProductPage.razor` | Remove `Categories="_categories"` parameter pass-through and `_categories` field (now owned by `ProductForm`). Remove the TODO comment. |

---

## 6) Potential Pitfalls

- **`ProductForm` is used in multiple places**: Verify `ProductForm.razor` is only instantiated in `CreateProductPage.razor` (and possibly an edit page) — the `Categories` parameter removal affects all usages. Check for an edit product page.
- **`GetCategoriesForDropdownResponse` record location**: Add it at the bottom of `MenuApiClient.cs` consistent with `CreateCategoryResponse` and `CreateProductResponse` already there.
- **API route format**: Existing calls use `/api/menu.products` (dot notation). The new endpoint route is `/api/menu/categories/dropdown` (slash notation per feature brief). Verify which route format the API actually uses by checking how `CategoriesGroup` registers the route prefix.
- **Error handling in `OnInitializedAsync`**: If the API call fails during initialization, `_categories` stays `[]` — the form will show an empty dropdown silently. Consider setting `_errorMessage` on failure.
- **`CategoryType` int cast**: Pass `(int)CategoryType.Product` to the API method, consistent with how `CreateCategoryRequest` passes `int CategoryType`.
