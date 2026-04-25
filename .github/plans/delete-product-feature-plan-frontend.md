# Plan: Delete Product — Frontend

## Metadata

**Type:** Feature  
**Layers Affected:** Blazor  
**Created:** 2025-07-14

## References

- Existing `DeleteCategoryAsync` / `OnCategoryDeletedAsync` pattern in `MenuApiClient.cs` and `ProductsManagementPage.razor`
- Existing `DeleteIngredientAsync` in `MenuApiClient.cs`
- `ProductTable.razor` — delete confirmation dialog and `OnDelete` `EventCallback<Guid>` are already wired up
- `ProductsManagementPage.razor` — `OnProductDeletedAsync` stub exists but lacks the API call

---

## Implementation Plan

### Step 1: Create frontend feature structure

No new folders needed. The feature touches existing files only.

---

### Step 2: Create or update API communication service

**File to modify:** `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Common/Services/MenuApiClient.cs`

Add a new `DeleteProductAsync` method following the exact pattern of `DeleteCategoryAsync` / `DeleteIngredientAsync`:

```csharp
public async Task DeleteProductAsync(Guid id, CancellationToken ct = default)
{
    using HttpResponseMessage response = await httpClient.DeleteAsync($"/api/menu/products/{id}", ct);
    response.EnsureSuccessStatusCode();
}
```

Place it after `UpdateProductAsync` to keep products methods grouped together.

---

### Step 3: Create or update models, DTOs and mappings

No new models or DTOs needed. The operation returns `204 No Content` — there is nothing to deserialize.

---

### Step 4: Create or update Blazor components and pages

#### 4.1 Wire up the API call in `ProductsManagementPage.razor`

**File to modify:** `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/ProductsManagementPage.razor`

The `OnProductDeletedAsync` method already has the try/catch structure and success/error message fields.  
Replace the placeholder implementation with the actual API call + list reload, mirroring `OnCategoryDeletedAsync`:

```csharp
// Before (stub — missing API call):
private async Task OnProductDeletedAsync(Guid id)
{
    _successMessage = null;
    _errorMessage = null;
    try
    {
        _successMessage = "Product deleted successfully.";
        await LoadProductsAsync();
    }
    catch (HttpRequestException)
    {
        _errorMessage = "Failed to delete product. Please try again.";
    }
}

// After (wired up):
private async Task OnProductDeletedAsync(Guid id)
{
    _successMessage = null;
    _errorMessage = null;
    try
    {
        await MenuApiClient.DeleteProductAsync(id);
        _successMessage = "Product deleted successfully.";
        await LoadProductsAsync();
    }
    catch (HttpRequestException)
    {
        _errorMessage = "Failed to delete product. Please try again.";
    }
}
```

#### 4.2 `ProductTable.razor` — no changes required

The confirmation dialog (`ShowMessageBoxAsync`) and `OnDelete.InvokeAsync(id)` callback are already fully implemented and will correctly invoke `OnProductDeletedAsync` on the parent page.
