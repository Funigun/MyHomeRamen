# Feature Implementation Plan — GetIngredientById (Frontend)

- **Date**: 2025-07-15
- **Feature**: GetIngredientById
- **Module**: Menu
- **Type**: Feature (Blazor edit page + API client wiring)

---

## 11) Create frontend feature structure

```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Ingredients/
├── Components/
│   ├── IngredientForm.razor          ← existing (MODIFIED — FormMode + InitialModel support)
│   ├── IngredientFormModel.cs        ← RENAMED from IngredientModel.cs
│   ├── IngredientTableModel.cs       ← NEW — read-only display model for IngredientTable
│   └── IngredientValidator.cs        ← existing (unchanged)
├── Responses/
│   ├── CreateIngredientResponse.cs   ← existing (unchanged)
│   ├── GetIngredientsForManageResponse.cs ← existing (from GetIngredientsForManage feature)
│   └── GetIngredientByIdResponse.cs  ← NEW
├── EditIngredientPage.razor          ← NEW
├── IngredientsManagementPage.razor   ← existing (unchanged)
└── CreateIngredientPage.razor        ← existing (unchanged)
```

---

## 12) Create or update API communication services and API Response model

### `Responses/GetIngredientByIdResponse.cs` (NEW)
- `public sealed record GetIngredientByIdResponse(Guid Id, string Name, string Description, decimal Price, IEnumerable<Guid> CategoryIds);`
- Namespace: `MyHomeRamen.Blazor.Features.Menu.Ingredients.Responses`

### `MenuApiClient.cs` (MODIFIED)
- Add method:
```csharp
public async Task<GetIngredientByIdResponse?> GetIngredientByIdAsync(Guid id, CancellationToken ct = default)
```
- Call `httpClient.GetFromJsonAsync<GetIngredientByIdResponse>($"/api/menu/ingredients/{id}", ct)`
- Reference: `GetIngredientsForDropdownAsync`

### `MenuNavigationService.cs` (MODIFIED)
- Implement the `ToEditIngredient(Guid id)` stub (added during `GetIngredientsForManage`):
  - Update `Routes.Admin.EditIngredient(Guid id)` to return `$"/admin/menu/ingredients/{id}/edit"`
  - Remove the `TODO` comment from both the route and the navigation method

---

## 13) Create or update models, DTOs and mappings

### `Components/IngredientFormModel.cs` (RENAMED from `IngredientModel.cs`)
- Rename file and class from `IngredientModel` to `IngredientFormModel` — this is the form-binding model used by `IngredientForm` for both Create and Edit scenarios
- Add a static factory: `public static IngredientFormModel FromResponse(GetIngredientByIdResponse response)` that maps `Name`, `Description`, `Price`, and `CategoryIds`
- Existing `ToCreateRequest()` method remains unchanged
- All existing references to `IngredientModel` in `IngredientForm.razor`, `IngredientValidator.cs`, and `CreateIngredientPage.razor` must be updated to `IngredientFormModel`

### `Components/IngredientTableModel.cs` (NEW)
- A separate read-only display model used by `IngredientTable` to decouple the table view from the API response DTO and the form model
- Properties: `Guid Id`, `string Name`, `string Description`
- Static factory: `public static IngredientTableModel FromResponse(GetIngredientsForManageResponse response)` that maps the three fields
- `IngredientTable.razor` (from `GetIngredientsForManage`) should bind its `Items` parameter to `List<IngredientTableModel>` instead of `List<GetIngredientsForManageResponse>`; update `IngredientsManagementPage` accordingly

---

## 14) Create or update Blazor components and pages

### `EditIngredientPage.razor` (NEW)
- Route: `@page "/admin/menu/ingredients/{Id:guid}/edit"`
- Authorization: `@attribute [Authorize(Roles = MenuRoleConstants.Admin)]`
- Parameter: `[Parameter] public Guid Id { get; set; }`
- Inject: `MenuApiClient`, `MenuNavigationService`
- Behavior:
  1. In `OnInitializedAsync`, call `MenuApiClient.GetIngredientByIdAsync(Id)` to load ingredient details
  2. Populate `IngredientFormModel` via `IngredientFormModel.FromResponse(response)`
  3. Render `IngredientForm Mode="FormMode.Edit"` with `InitialModel` bound to the populated `IngredientFormModel`
  4. On form submit: placeholder with `// TODO: Call EditIngredient endpoint`
  5. On success: navigate back to `IngredientsManagementPage` via `MenuNavigation.ToIngredientsManagement()`
- Loading state: `MudProgressLinear` while fetching
- Error state: `MudAlert Severity.Error` if fetch fails
- Reference: `CreateIngredientPage.razor`, `IngredientForm.razor`

### `IngredientForm.razor` (MODIFIED)
- Add `[Parameter] public FormMode Mode { get; set; } = FormMode.Create;`
- Add `[Parameter] public IngredientFormModel? InitialModel { get; set; }` — when set, copy values into the internal `_model` in `OnParametersSet`
- Conditionally change the submit button text: "Create Ingredient" vs "Save Changes" based on `Mode`
- Update all internal references from `IngredientModel` → `IngredientFormModel`

---

## 15) Create Unit tests for Blazor components and services

**Skipped.** Blazor tests instructions are `TODO` — not yet defined.
