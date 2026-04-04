# Feature Implementation Plan — DeleteCategory (Frontend)

- **Date**: 2025-01-27
- **Feature**: DeleteCategory — wire up delete button in CategoryTable and management pages
- **Module**: Menu (Blazor)

---

## 11) Create frontend feature structure

No new feature folder needed — the delete action integrates into the existing `CategoryTable` component and parent management pages.

---

## 12) Create or update API communication services and API Response model

### File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuApiClient.cs`
- Add method:
  ```csharp
  public async Task DeleteCategoryAsync(Guid id, CancellationToken ct = default)
  ```
- Calls `DELETE /api/menu/categories/{id}`
- Calls `response.EnsureSuccessStatusCode()`
- No response body (204 No Content)

No new API response model needed.

---

## 13) Create or update models, DTOs and mappings

No new models needed — the delete operation uses only the category `Guid` already available in `GetCategoriesByTypeResponse`.

---

## 14) Create or update Blazor components and pages

### File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/Components/CategoryTable.razor`
- Add `[Parameter] public EventCallback<Guid> OnDelete { get; set; }` parameter
- Wire the existing Delete `MudIconButton` to invoke `OnDelete.InvokeAsync(category.Id)` with the category's ID from the current `context`
- Optionally: add a confirmation dialog before firing the callback to prevent accidental deletes (e.g., `MudDialog` or simple `MudMessageBox`)

### File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/ProductsManagement/ProductsManagementPage.razor`
- Pass `OnDelete="OnCategoryDeleted"` to `<CategoryTable>`
- Add handler `OnCategoryDeleted(Guid id)`:
  1. Call `MenuApiClient.DeleteCategoryAsync(id)`
  2. Call `LoadCategoriesAsync()` to reload the full list from the API (server-assigned sort orders will be correct)
  3. Show success alert: "Category deleted successfully."
  4. On `HttpRequestException`: show error alert without modifying local state
  5. **Important**: The page must **not** remove the item from local state manually — always reload from the server so that updated `SortOrder` values are reflected correctly

### File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/IngredientsManagement/IngredientsManagementPage.razor`
- Same changes as `ProductsManagementPage.razor` above

---

## 15) Create Unit tests for Blazor components and services

Skip — blazor-tests instructions are marked as TODO, no existing patterns to follow.
