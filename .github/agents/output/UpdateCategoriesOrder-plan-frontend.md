# Feature Implementation Plan — UpdateCategoriesOrder (Frontend)

- **Date**: 2025-01-27
- **Feature**: UpdateCategoriesOrder — save reordered categories from drag-and-drop UI
- **Module**: Menu (Blazor)

---

## 11) Create frontend feature structure

No new feature folder needed — the update-order action integrates into the existing `CategoryTable` component and parent management pages.

New file only:
```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/UpdateCategoriesOrder/
└── UpdateCategoriesOrderRequest.cs
```

---

## 12) Create or update API communication services and API Response model

### File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuApiClient.cs`
- Add method:
  ```csharp
  public async Task UpdateCategoriesOrderAsync(UpdateCategoriesOrderRequest request, CancellationToken ct = default)
  ```
- Calls `PUT /api/menu/categories/order` with JSON body
- Calls `response.EnsureSuccessStatusCode()`
- No response body (204 No Content)

No new API response model needed.

---

## 13) Create or update models, DTOs and mappings

### File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/UpdateCategoriesOrder/UpdateCategoriesOrderRequest.cs`
```csharp
public sealed record CategoryOrderItem(Guid Id, int SortOrder);
public sealed record UpdateCategoriesOrderRequest(List<CategoryOrderItem> Items);
```

---

## 14) Create or update Blazor components and pages

### File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/Components/CategoryTable.razor`
- Add `[Parameter] public EventCallback<List<GetCategoriesByTypeResponse>> OnOrderChanged { get; set; }` parameter
- In `OnItemDropped`, after reordering the local list, invoke `OnOrderChanged.InvokeAsync(Items)` if the callback is bound

### File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/ProductsManagement/ProductsManagementPage.razor`
- Pass `OnOrderChanged="OnCategoriesOrderChanged"` to `<CategoryTable>`
- Add handler `OnCategoriesOrderChanged(List<GetCategoriesByTypeResponse> items)`:
  1. Build `UpdateCategoriesOrderRequest` from the reordered list with new `SortOrder` values (1-based index)
  2. Call `MenuApiClient.UpdateCategoriesOrderAsync(request)`
  3. Show success alert on completion
  4. Show error alert on `HttpRequestException`

### File: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/IngredientsManagement/IngredientsManagementPage.razor`
- Same changes as `ProductsManagementPage.razor` above

---

## 15) Create Unit tests for Blazor components and services

Skip — blazor-tests instructions are marked as TODO, no existing patterns to follow.
