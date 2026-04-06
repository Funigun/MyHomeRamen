- **Date**: 2025-07-15
- **Feature**: GetIngredientById
- **Critical**: 1
- **Warnings**: 3
- **Information**: 1

---

# Review Report — GetIngredientById (Frontend)

## Critical

---

### [1] [Frontend] — `EditIngredientPage.razor` not implemented

**Severity**: Critical

**Description**:
The frontend scope is fully unimplemented. The brief requires:
- A new `EditIngredientPage.razor` page at route `/admin/menu/ingredients/{Id:guid}/edit`
- `GetIngredientByIdAsync(Guid id)` method in `MenuApiClient`
- `GetIngredientByIdResponse.cs` response model in `MyHomeRamen.Blazor/Features/Menu/Ingredients/Responses/`

None of these were created. Without the page, the Edit button in `IngredientTable` (already wired to `OnEdit` → `MenuNavigation.ToEditIngredient(id)`) navigates to a route that returns a 404 blank page.

**Solution proposal**:

1. Create `MyHomeRamen.Blazor/Features/Menu/Ingredients/Responses/GetIngredientByIdResponse.cs` matching the API response shape (see backend report issue [3] for final shape):
```csharp
namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Responses;

public sealed record GetIngredientByIdResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);
```

2. Add `GetIngredientByIdAsync` to `MenuApiClient`:
```csharp
public async Task<GetIngredientByIdResponse?> GetIngredientByIdAsync(Guid id, CancellationToken ct = default)
{
    return await httpClient.GetFromJsonAsync<GetIngredientByIdResponse>(
        $"/api/menu/ingredients/{id}", ct);
}
```

3. Create `EditIngredientPage.razor`:
```razor
@page "/admin/menu/ingredients/{Id:guid}/edit"

@using Microsoft.AspNetCore.Authorization
@using MyHomeRamen.Blazor.Features.Menu.Common.Constants
@using MyHomeRamen.Blazor.Features.Menu.Common.Services
@using MyHomeRamen.Blazor.Features.Menu.Ingredients.Components

@attribute [Authorize(Roles = MenuRoleConstants.Admin)]

@inject MenuApiClient MenuApiClient
@inject MenuNavigationService MenuNavigation

<PageTitle>Edit Ingredient</PageTitle>

<MudPaper Elevation="3" Class="pa-6">
    <MudText Typo="Typo.h4" Class="mb-6">Edit Ingredient</MudText>

    @if (_isLoading)
    {
        <MudProgressLinear Indeterminate="true" />
    }
    else
    {
        <IngredientForm Model="_model" Mode="FormMode.Edit" OnSuccess="HandleSuccess" />
    }
</MudPaper>

@code {
    [Parameter] public Guid Id { get; set; }

    private IngredientModel _model = new();
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        var response = await MenuApiClient.GetIngredientByIdAsync(Id);
        if (response is not null)
        {
            _model = IngredientModel.FromResponse(response);
        }
        _isLoading = false;
    }

    private void HandleSuccess(Guid id)
    {
        MenuNavigation.ToIngredientsManagement();
    }
}
```

- **Implementation status**: ✅ Fixed in iteration 1 — `EditIngredientPage.razor` created at `/admin/menu/ingredients/{Id:guid}/edit`. `GetIngredientByIdResponse.cs` and `GetIngredientByIdAsync` added. `IngredientForm` updated to accept `Model` and `Mode` parameters.

---

## Warnings

---

### [2] [`MenuApiClient.cs`] — `GetIngredientByIdAsync` method not added

**Severity**: Warning

**Description**:
The brief requires adding `GetIngredientByIdAsync(Guid id)` to `MenuApiClient`. Without it, the `EditIngredientPage` (issue [1]) cannot call the API endpoint and there is no typed client method for this endpoint in the Blazor project. The pattern is well-established in `MenuApiClient` (e.g., `GetIngredientsForDropdownAsync`).

**Solution proposal**:

Add to `MenuApiClient.cs`:
```csharp
public async Task<GetIngredientByIdResponse?> GetIngredientByIdAsync(Guid id, CancellationToken ct = default)
{
    return await httpClient.GetFromJsonAsync<GetIngredientByIdResponse>(
        $"/api/menu/ingredients/{id}", ct);
}
```

Reference: the existing `GetIngredientsForDropdownAsync` method at line 51 in `MenuApiClient.cs`.

- **Implementation status**: ✅ Fixed in iteration 1 — `GetIngredientByIdAsync` added to `MenuApiClient.cs`.

---

### [3] [`Features/Menu/Ingredients/Responses/`] — `GetIngredientByIdResponse` Blazor type missing

**Severity**: Warning

**Description**:
The brief requires `MyHomeRamen.Blazor/Features/Menu/Ingredients/Responses/GetIngredientByIdResponse.cs`. Without this type:
- `MenuApiClient.GetIngredientByIdAsync` cannot be implemented with a typed return.
- The architecture test `BlazorResponse_ShouldMatch_ApiResponseShape` in `ApiToBlazorContractSyncTests` will only validate the shape once both exist — currently there is a silent gap in coverage for this response type.

Note: the shape of this record must exactly match the API `GetIngredientByIdResponse`. If the API response is corrected per backend report issue [3] (using `IEnumerable<Guid> CategoryIds`), this Blazor response must mirror it.

**Solution proposal**:

Create `MyHomeRamen.Blazor/Features/Menu/Ingredients/Responses/GetIngredientByIdResponse.cs`:
```csharp
namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Responses;

public sealed record GetIngredientByIdResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);
```

- **Implementation status**: ✅ Fixed in iteration 1 — `GetIngredientByIdResponse.cs` created in `MyHomeRamen.Blazor/Features/Menu/Ingredients/Responses/` mirroring the API response shape.

**Severity**: Warning

**Description**:
Per Blazor instructions, `{Entity}FormModel` must expose:
- `static FromResponse(GetXxxByIdResponse)` — factory to pre-fill the edit form from API data
- `ToEditRequest()` — maps model to the edit payload (even as a placeholder until `EditIngredient` feature is implemented)

Currently `IngredientModel` only has `ToCreateRequest()`. The `EditIngredientPage` (issue [1]) needs `IngredientModel.FromResponse(response)` to pre-populate the form with loaded ingredient data. Without this, the page cannot pre-fill the form fields.

The instructions state explicitly:
> `static ProductFormModel FromResponse(GetProductByIdResponse r) => new() { Name = r.Name, Price = r.Price, CategoryIds = r.CategoryIds };`

**Solution proposal**:

Add to `IngredientModel.cs`:
```csharp
public static IngredientModel FromResponse(GetIngredientByIdResponse response)
{
    return new IngredientModel
    {
        Name = response.Name,
        Description = response.Description,
        Price = response.Price,
        CategoryIds = response.CategoryIds,
    };
}

// TODO: Map to EditIngredientRequest once EditIngredient feature is implemented
public CreateIngredientRequest ToEditRequest()
{
    return new CreateIngredientRequest(Name, Description, Price, CategoryIds);
}
```

- **Implementation status**: ✅ Fixed in iteration 1 — `FromResponse(GetIngredientByIdResponse)` factory and `ToEditRequest()` placeholder added to `IngredientModel.cs`.

---

## Information

---

### [5] [`MenuNavigationService.cs` : 22] — `TODO` comment not removed from `EditIngredient` route

- **Implementation status**: ✅ Fixed in iteration 1 — TODO comment removed from `MenuNavigationService.cs`.

**Severity**: Information

**Description**:
The brief says: *"Implement the `ToEditIngredient(Guid id)` stub added during `GetIngredientsForManage` with the actual route — Remove the `TODO` comment from the stub."*

The `ToEditIngredient` method is correctly implemented and the route is set to `/admin/menu/ingredients/{id}/edit`. However, the comment `// TODO: Implement EditIngredient page` remains on line 22. The navigation service is already done; the comment misleads future readers into thinking the route is still pending.

**Solution proposal**:

Remove the comment from `MenuNavigationService.Routes.Admin`:

```csharp
// Before:
// TODO: Implement EditIngredient page
public static string EditIngredient(Guid id) => $"/admin/menu/ingredients/{id}/edit";

// After:
public static string EditIngredient(Guid id) => $"/admin/menu/ingredients/{id}/edit";
```
