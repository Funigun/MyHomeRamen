# Feature Implementation Plan — DeleteIngredient (Frontend)

- **Date**: 2025-07-15
- **Feature**: DeleteIngredient
- **Module**: Menu
- **Type**: Feature (Blazor wiring only — no new components)

---

## 11) Create frontend feature structure

No new files or folders needed — this feature only wires up the existing `IngredientTable` delete flow and `IngredientsManagementPage` placeholder handler.

---

## 12) Create or update API communication services and API Response model

### `MenuApiClient.cs` (MODIFIED)
- Add method:
```csharp
public async Task DeleteIngredientAsync(Guid id, CancellationToken ct = default)
{
    using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/menu/ingredients/{id}", ct);
    response.EnsureSuccessStatusCode();
}
```
- Reference: `DeleteCategoryAsync` in `MenuApiClient.cs`

---

## 13) Create or update models, DTOs and mappings

No changes needed — no new models or DTOs for delete.

---

## 14) Create or update Blazor components and pages

### `IngredientsManagementPage.razor` (MODIFIED)
- Implement the `OnIngredientDeletedAsync(Guid id)` placeholder (added during `GetIngredientsForManage`):
  1. Clear `_successMessage` and `_errorMessage`
  2. Call `MenuApiClient.DeleteIngredientAsync(id)`
  3. On success: set `_successMessage = "Ingredient deleted successfully."` and call `LoadIngredientsAsync()` to refresh the list
  4. On `HttpRequestException`: set `_errorMessage = "Failed to delete ingredient. Please try again."`
  5. Remove the `TODO` comment from the handler
- Reference: `OnCategoryDeletedAsync` handler in `IngredientsManagementPage.razor`

---

## 15) Create Unit tests for Blazor components and services

**Skipped.** Blazor tests instructions are `TODO` — not yet defined.
