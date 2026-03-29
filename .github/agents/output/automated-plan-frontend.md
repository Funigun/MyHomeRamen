# Feature Implementation Plan — Frontend

- **Date**: 2025-07-14
- **Feature**: CreateIngredient
- **Module**: Menu
- **Reference**: CreateCategoryForm.razor, ProductForm.razor, CreateProductPage.razor

---

## 11) Create frontend feature structure

```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Ingredients/
??? Components/
?   ??? IngredientForm.razor          ? Form component with MudForm + validation
?   ??? IngredientModel.cs            ? UI model with ToCreateRequest() mapping
?   ??? IngredientValidator.cs        ? FluentValidation validator extending BaseValidator<IngredientModel>
??? CreateIngredient/
?   ??? CreateIngredientPage.razor    ? Page wrapping IngredientForm
?   ??? CreateIngredientRequest.cs    ? API DTO record
```

No new folders needed for `Common/Services/` or `Common/Models/` — existing `MenuApiClient`, `MenuNavigationService`, and `CategoryOption` are reused.

---

## 12) Create or update API communication services and API Response model

### `MenuApiClient.cs` — add `CreateIngredientAsync` method

Add to `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuApiClient.cs`:
```csharp
public async Task<Guid> CreateIngredientAsync(CreateIngredientRequest request, CancellationToken ct = default)
{
    using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/menu.ingredients", request, ct);
    response.EnsureSuccessStatusCode();

    CreateIngredientResponse? result = await response.Content.ReadFromJsonAsync<CreateIngredientResponse>(ct);
    return result?.Id ?? throw new InvalidOperationException("Failed to deserialize ingredient creation response.");
}
```

Add response record at bottom of file (following existing pattern with `CreateProductResponse`, `CreateCategoryResponse`):
```csharp
public sealed record CreateIngredientResponse(Guid Id);
```

Add `using` for `CreateIngredientRequest`:
```csharp
using MyHomeRamen.Blazor.Features.Menu.Ingredients.CreateIngredient;
```

### `MenuNavigationService.cs` — add ingredient routes

Add to `Routes.Admin` class:
```csharp
public const string IngredientsIndex = "/admin/menu/ingredients";
public const string CreateIngredient = "/admin/menu/ingredients/create";
```

Add navigation methods:
```csharp
public void ToAdminIngredientsIndex() => navigation.NavigateTo(Routes.Admin.IngredientsIndex);
public void ToCreateIngredient() => navigation.NavigateTo(Routes.Admin.CreateIngredient);
```

---

## 13) Create or update models, DTOs and mappings

### `CreateIngredient/CreateIngredientRequest.cs` (API DTO)
```csharp
namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.CreateIngredient;

public sealed record CreateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);
```

### `Components/IngredientModel.cs` (UI Model)
```csharp
namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Components;

public sealed class IngredientModel
{
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public IEnumerable<Guid> CategoryIds { get; set; } = [];

    public CreateIngredientRequest ToCreateRequest()
    {
        return new CreateIngredientRequest(Name, Description, Price, CategoryIds);
    }
}
```
- Follows `CategoryModel` and `ProductModel` pattern
- Exposes `ToCreateRequest()` for manual mapping to API DTO

### `Components/IngredientValidator.cs`
```csharp
namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Components;

public sealed class IngredientValidator : BaseValidator<IngredientModel>
{
    public IngredientValidator()
    {
        RuleFor(x => x.Name)
            .SetValidator(new IngredientNameValidator());

        RuleFor(x => x.Description)
            .SetValidator(new IngredientDescriptionValidator());

        RuleFor(x => x.Price)
            .SetValidator(new IngredientPriceValidator());

        RuleFor(x => x.CategoryIds)
            .NotEmpty()
            .WithMessage("Please select at least one category.");
    }
}
```
- Extends `BaseValidator<IngredientModel>` (provides `ValidateValue` delegate for MudForm)
- Reuses primitive validators from `MyHomeRamen.Common.Contracts.Menu.Ingredients`

---

## 14) Create or update Blazor components and pages

### `Components/IngredientForm.razor`

Form component following the `CreateCategoryForm.razor` and `ProductForm.razor` patterns:

- Injects `MenuApiClient`
- Uses `MudForm` with `_validator.ValidateValue`
- Fields:
  - `MudTextField` for Name (required)
  - `MudTextField` for Description (required, multiline with `Lines="4"`)
  - `MudNumericField` for Price (required, with `$` adornment, Min=0.0m, Max=50.0m)
  - `MudSelect<Guid>` for Categories (MultiSelection=true, required) — loads `CategoryType.Ingredient` categories from `MenuApiClient.GetCategoriesForDropdownAsync((int)CategoryType.Ingredient)`
- Error alert (`MudAlert`) for API failure
- Submit button with busy state (`MudProgressCircular`)
- `[Parameter] public EventCallback<Guid> OnSuccess` — invoked after successful creation
- `OnInitializedAsync` loads ingredient categories from API

Follows the submission pattern:
```csharp
private async Task SubmitAsync()
{
    await _form.Validate();
    if (!_form.IsValid) return;

    _isBusy = true;
    _errorMessage = null;
    try
    {
        Guid id = await MenuApiClient.CreateIngredientAsync(_model.ToCreateRequest());
        await OnSuccess.InvokeAsync(id);
    }
    catch (HttpRequestException)
    {
        _errorMessage = "Failed to create ingredient. Please try again.";
    }
    finally
    {
        _isBusy = false;
    }
}
```

### `CreateIngredient/CreateIngredientPage.razor`

Page component following the `CreateProductPage.razor` pattern:

```razor
@page "/admin/menu/ingredients/create"
@attribute [Authorize(Roles = MenuRoleConstants.Admin)]

<PageTitle>Create Ingredient</PageTitle>

<MudPaper Elevation="3" Class="pa-6">
    <MudText Typo="Typo.h4" Class="mb-6">Create New Ingredient</MudText>
    <IngredientForm OnSuccess="HandleSuccess" />
</MudPaper>

@code {
    @inject MenuNavigationService MenuNavigation

    private void HandleSuccess(Guid ingredientId)
    {
        MenuNavigation.ToAdminIngredientsIndex();
    }
}
```
- Route: `/admin/menu/ingredients/create`
- Authorization: `MenuRoleConstants.Admin`
- On success navigates to ingredients index page

---

## 15) Create Unit tests for Blazor components and services

**No frontend unit tests required** — `blazor-tests.instructions.md` is `TODO` status and no test infrastructure is in place for Blazor yet.

---

## Summary of files to create/modify

### New files:
| File | Description |
|---|---|
| `MyHomeRamen.Api/Menu/Features/Ingredients/CreateIngredient/Models/CreateIngredientRequest.cs` | API request record |
| `MyHomeRamen.Api/Menu/Features/Ingredients/CreateIngredient/Models/CreateIngredientResponse.cs` | API response record |
| `MyHomeRamen.Api/Menu/Features/Ingredients/CreateIngredient/Models/Mappings.cs` | Request ? Domain mapping |
| `MyHomeRamen.Api/Menu/Features/Ingredients/CreateIngredient/Policies/CreateIngredientValidator.cs` | FluentValidation validator |
| `MyHomeRamen.Api/Menu/Features/Ingredients/CreateIngredient/CreateIngredientEndpoint.cs` | Minimal API endpoint |
| `MyHomeRamen.Api/Menu/Features/Ingredients/CreateIngredient/CreateIngredientHandler.cs` | Request handler |
| `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Ingredients/Components/IngredientForm.razor` | Blazor form component |
| `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Ingredients/Components/IngredientModel.cs` | UI model |
| `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Ingredients/Components/IngredientValidator.cs` | Blazor FluentValidation validator |
| `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Ingredients/CreateIngredient/CreateIngredientPage.razor` | Create page |
| `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Ingredients/CreateIngredient/CreateIngredientRequest.cs` | Blazor API DTO |
| `MyHomeRamen.IntegrationTests/MenuModule/CreateIngredientTests.cs` | Integration tests |

### Modified files:
| File | Change |
|---|---|
| `MyHomeRamen.Persistance/Common/DbExtensions.cs` | Add `IsIngredientNameUniqueAsync` extension method |
| `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuApiClient.cs` | Add `CreateIngredientAsync` method + `CreateIngredientResponse` record |
| `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuNavigationService.cs` | Add ingredient routes and navigation methods |
| `MyHomeRamen.IntegrationTests/MenuModule/Common/Data/DataGenerator.cs` | Add `InvalidCreateIngredientRequests()` theory data |
| `MyHomeRamen.IntegrationTests/MenuModule/Common/Data/Mappings.cs` | Add `ToCreateIngredientRequest()` mapping extension |
