# Feature Implementation Plan — GetIngredientsForManage (Frontend)

- **Date**: 2025-07-15
- **Feature**: GetIngredientsForManage
- **Module**: Menu
- **Type**: Feature (Blazor component + API client wiring)

---

## 11) Create frontend feature structure

No new feature folder needed — the component lives inside the existing `Ingredients` feature folder:

```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Ingredients/
├── Components/
│   ├── IngredientForm.razor          ← existing (unchanged)
│   ├── IngredientModel.cs            ← existing (unchanged)
│   ├── IngredientValidator.cs        ← existing (unchanged)
│   └── IngredientTable.razor         ← NEW
├── Responses/
│   ├── CreateIngredientResponse.cs   ← existing (unchanged)
│   └── GetIngredientsForManageResponse.cs ← NEW
├── IngredientsManagementPage.razor   ← MODIFIED
└── CreateIngredientPage.razor        ← existing (unchanged)
```

---

## 12) Create or update API communication services and API Response model

### `Responses/GetIngredientsForManageResponse.cs` (NEW)
- `public sealed record GetIngredientsForManageResponse(Guid Id, string Name, string Description);`
- Namespace: `MyHomeRamen.Blazor.Features.Menu.Ingredients.Responses`
- Reference: `GetIngredientsForDropdownResponse.cs`

### `MenuApiClient.cs` (MODIFIED)
- Add method:
```csharp
public async Task<IEnumerable<GetIngredientsForManageResponse>> GetIngredientsForManageAsync(
    string? name = null, IEnumerable<Guid>? categoryIds = null, CancellationToken ct = default)
```
- Build query string conditionally:
  - Append `?name={name}` when `name` is not null/empty
  - Append repeated `&categoryIds={id}` for each category ID when provided
- Call `httpClient.GetFromJsonAsync<IEnumerable<GetIngredientsForManageResponse>>(url, ct)`
- Return result or empty enumerable
- Reference: `GetIngredientsForDropdownAsync`, `GetCategoriesByTypeAsync`

### `MenuNavigationService.cs` (MODIFIED)
- Add route stub in `Routes.Admin`:
  - `public static string EditIngredient(Guid id) => $"/admin/menu/ingredients/{id}/edit";` with `// TODO: Implement EditIngredient page`
- Add navigation method:
  - `public void ToEditIngredient(Guid id) => navigation.NavigateTo(Routes.Admin.EditIngredient(id));` with `// TODO: Implement EditIngredient page`

---

## 13) Create or update models, DTOs and mappings

No new UI models needed — the `IngredientTable` component works directly with the `GetIngredientsForManageResponse` read DTO (display-only, no form binding).

---

## 14) Create or update Blazor components and pages

### `Components/IngredientTable.razor` (NEW)
- Parameters:
  - `[Parameter, EditorRequired] public List<GetIngredientsForManageResponse> Items { get; set; }`
  - `[Parameter] public bool IsLoading { get; set; }`
  - `[Parameter] public EventCallback<Guid> OnEdit { get; set; }`
  - `[Parameter] public EventCallback<Guid> OnDelete { get; set; }`
- Layout:
  - Header row: `Name`, `Description`, `Actions`
  - Data rows using `MudStack Row="true"` (no drag-and-drop — ingredients are not reorderable)
  - Actions column: Edit `MudIconButton` (`Icons.Material.Filled.Edit`) + Delete `MudIconButton` (`Icons.Material.Filled.Delete`, `Color.Error`)
- Loading state: `MudProgressLinear Indeterminate="true"`
- Empty state: `MudAlert Severity.Info` "No ingredients found."
- Delete confirmation: `MudDialog` via `IDialogService.ShowMessageBoxAsync` — same pattern as `CategoryTable.razor`
- Reference: `CategoryTable.razor` (omit `MudDropContainer`, `OnOrderChanged`, and drag indicator)

### `IngredientsManagementPage.razor` (MODIFIED)
- Add state fields:
  - `private List<GetIngredientsForManageResponse> _ingredients = [];`
  - `private bool _isIngredientsLoading = true;`
- Add using: `@using MyHomeRamen.Blazor.Features.Menu.Ingredients.Responses`
- Add using: `@using MyHomeRamen.Blazor.Features.Menu.Ingredients.Components`
- Inject: `@inject MenuNavigationService MenuNavigation`
- At the bottom of the existing `<MudPaper>`, after the `CategoryTable`, add:
  1. `<MudDivider Class="my-6" />`
  2. `<MudText Typo="Typo.h5" Class="mb-4">Ingredients</MudText>`
  3. `<IngredientTable Items="_ingredients" IsLoading="_isIngredientsLoading" OnEdit="OnEditIngredient" OnDelete="OnIngredientDeletedAsync" />`
- Add methods:
  - `LoadIngredientsAsync()` — calls `MenuApiClient.GetIngredientsForManageAsync()`, populates `_ingredients`
  - `OnEditIngredient(Guid id)` — calls `MenuNavigation.ToEditIngredient(id)`
  - `OnIngredientDeletedAsync(Guid id)` — placeholder with `// TODO: Call delete endpoint and reload list`
- Modify `OnInitializedAsync` to also call `LoadIngredientsAsync()`

---

## 15) Create Unit tests for Blazor components and services

**Skipped.** Blazor tests instructions are `TODO` — not yet defined.
