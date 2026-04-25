---
type: feature
module: Menu
aggregate: Product
accessibility: Manager
---

# DeleteProduct

## User Story
As an Admin/Manager, I want to delete a product so that it is removed from the menu and no longer available to customers.

## Scope
- **Backend:** yes
- **Frontend:** yes

## Backend Details

### Description
Implement a new feature to delete a product from the database. This action should be restricted to users with Manager/Admin privileges.

### API Endpoints
- **DELETE** `/api/menu/products/{id}`

### Validation Rules
- Verify entity existence: Ensure the product exists before attempting to delete it. Return a 404 Not Found or appropriate domain error if it does not.

### Domain Events
- None.

### Caching
- Cache invalidation required: Invalidate relevant product and menu caches so that the deleted item no longer appears in customer-facing and management lists.

## Frontend Details

### API Client
- **`MenuApiClient.cs`**: Add a new method `DeleteProductAsync(Guid id, CancellationToken ct = default)` that sends a DELETE request to `/api/menu/products/{id}` and ensures a success status code.

### Pages & Components
- **`ProductsManagementPage.razor`**: 
  - Wire up the actual API call inside the existing `OnProductDeletedAsync` method using `MenuApiClient.DeleteProductAsync(id)`.
  - Handle potential exceptions (like `HttpRequestException`) to display an error notification.
  - Display a success message and reload the product list (`LoadProductsAsync`) upon successful deletion.
- **`ProductTable.razor`**: 
  - Ensure the delete action invokes the `OnDelete` EventCallback after the user confirms the deletion dialog. (The dialog and callback mechanism are already structurally present, just ensure they trigger the correct parent component method smoothly).