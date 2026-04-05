# Feature Implementation Plan — UpdateIngredient (Frontend)

- **Date**: 2025-07-15
- **Feature**: UpdateIngredient
- **Module**: Menu
- **Type**: Feature (Blazor wiring — implements EditIngredientPage TODO + form branching)

---

## 11) Create frontend feature structure

No new folders are needed. All changes are in the existing `Ingredients` feature folder.

```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Ingredients/
├── Components/
│   ├── IngredientForm.razor          ← MODIFIED — add IngredientId param, SubmitAsync branching, button label
│   └── IngredientFormModel.cs        ← MODIFIED — add ToEditRequest()
├── Requests/
│   └── UpdateIngredientRequest.cs    ← NEW
└── EditIngredientPage.razor          ← MODIFIED — replace TODO placeholder with UpdateIngredientAsync call
```

Also modified:
- `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuApiClient.cs`

---

## 12) Create or update API communication services and API Response model

### `Requests/UpdateIngredientRequest.cs` (NEW)

```csharp
namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Requests;

public sealed record UpdateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);
```

- Mirrors the backend `UpdateIngredientRequest` body fields (no `Id` — the id goes in the URL)
- Namespace: `MyHomeRamen.Blazor.Features.Menu.Ingredients.Requests`
- Reference: `CreateIngredientRequest.cs` in the same folder

### `MenuApiClient.cs` (MODIFIED)

Add method:

```csharp
public async Task<UpdateIngredientResponse> UpdateIngredientAsync(
    Guid id,
    UpdateIngredientRequest request,
    CancellationToken ct = default)
{
    using HttpResponseMessage response = await httpClient.PutAsJsonAsync(
        $"/api/menu/ingredients/{id}", request, ct);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadFromJsonAsync<UpdateIngredientResponse>(ct)
        ?? throw new InvalidOperationException("Empty response from UpdateIngredient endpoint.");
}
```

- Sends `PUT /api/menu/ingredients/{id}` with JSON body
- Returns `UpdateIngredientResponse` (containing the updated ingredient `Id`)
- Reference: `CreateIngredientAsync` (returns a typed response) and `DeleteIngredientAsync` (EnsureSuccessStatusCode pattern)

Also add the response type used by the client:

### `Responses/UpdateIngredientResponse.cs` (NEW)

```csharp
namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Responses;

public sealed record UpdateIngredientResponse(Guid Id);
```

- Matches the backend `UpdateIngredientResponse`
- Reference: `CreateIngredientResponse.cs` in the same folder

---

## 13) Create or update models, DTOs and mappings

### `Components/IngredientFormModel.cs` (MODIFIED)

Add the `ToEditRequest()` mapping method alongside the existing `ToCreateRequest()`:

```csharp
public UpdateIngredientRequest ToEditRequest()
    => new(Name, Description, Price, CategoryIds);
```

- Returns an `UpdateIngredientRequest` (body-only record) from the current form state
- Reference: existing `ToCreateRequest()` method in the same file

---

## 14) Create or update Blazor components and pages

### `Components/IngredientForm.razor` (MODIFIED)

Three changes:

**1. Add `IngredientId` parameter** (used when `Mode == FormMode.Edit`):
```razor
[Parameter] public Guid IngredientId { get; set; }
```

**2. Branch `SubmitAsync` on `Mode`**:

Replace the existing single-path `SubmitAsync` (which always calls `CreateIngredientAsync`) with a branched call:

```csharp
private async Task SubmitAsync()
{
    await _form.Validate();
    if (!_form.IsValid) return;

    _isBusy = true;
    _errorMessage = null;
    try
    {
        if (Mode == FormMode.Edit)
        {
            await MenuApiClient.UpdateIngredientAsync(IngredientId, _model.ToEditRequest());
        }
        else
        {
            await MenuApiClient.CreateIngredientAsync(_model.ToCreateRequest());
        }
        await OnSuccess.InvokeAsync();
    }
    catch (HttpRequestException)
    {
        _errorMessage = Mode == FormMode.Edit
            ? "Failed to update ingredient. Please try again."
            : "Failed to create ingredient. Please try again.";
    }
    finally
    {
        _isBusy = false;
    }
}
```

**3. Update submit button label** based on `Mode`:

```razor
<MudButton ButtonType="ButtonType.Submit" Variant="Variant.Filled" Color="Color.Primary" Disabled="_isBusy">
    @(Mode == FormMode.Edit ? "Save Changes" : "Create Ingredient")
</MudButton>
```

- Reference: `CreateCategoryForm.razor` or `CategoryTable.razor` for `_isBusy` / `_errorMessage` pattern
- Reference: existing `IngredientForm.razor` `@code` block structure

### `EditIngredientPage.razor` (MODIFIED)

Replace the `// TODO: Call EditIngredient endpoint` placeholder (introduced by `GetIngredientById`) with:

```csharp
private async Task OnIngredientUpdatedAsync()
{
    MenuNavigationService.ToIngredientsManagement();
}
```

And update the `IngredientForm` component call to pass `IngredientId`:

```razor
<IngredientForm Mode="FormMode.Edit"
                IngredientId="Id"
                InitialModel="_model"
                OnSuccess="OnIngredientUpdatedAsync" />
```

- `IngredientId="Id"` passes the page route parameter `Id` to the form so `SubmitAsync` uses the correct ingredient id
- `OnIngredientUpdatedAsync` navigates back to `IngredientsManagementPage` on success
- Reference: `CreateIngredientPage.razor` for the success navigation pattern

---

## 15) Create Unit tests for Blazor components and services

**Skipped.** Blazor tests instructions are `TODO` — not yet defined.
