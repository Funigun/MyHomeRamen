# Feature plan — UpdateProduct (Frontend)

- **Date**: 2025-07-15
- **Feature**: UpdateProduct — Blazor Edit page, ProductForm Edit mode support, API client, request/response types
- **Reference**: `EditIngredientPage.razor`, `IngredientForm.razor`, `IngredientModel.cs`

---

## 11) Create frontend feature structure

New files to create:
```
MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/
├── EditProductPage.razor                          ← NEW
├── Requests/
│   └── UpdateProductRequest.cs                    ← NEW
├── Responses/
│   └── UpdateProductResponse.cs                   ← NEW
├── Components/
│   ├── ProductModel.cs                            ← MODIFY (add FromResponse, ToEditRequest)
│   └── ProductForm.razor                          ← MODIFY (add Edit mode support)
```

---

## 14) Create or update API communication services and API Response model

### New file: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Requests/UpdateProductRequest.cs`
- `public sealed record UpdateProductRequest(string Name, string? Description, decimal Price, Guid CategoryId, IEnumerable<Guid> IngredientIds)`
- Reference: `UpdateIngredientRequest` (Blazor)

### New file: `MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/Responses/UpdateProductResponse.cs`
- `public sealed record UpdateProductResponse(Guid Id)`
- Reference: `UpdateIngredientResponse` (Blazor)

### Update: `MenuApiClient.cs`
- Add method: `UpdateProductAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default)`
  - `PUT /api/menu/products/{id}`
  - Pattern: `httpClient.PutAsJsonAsync(...)` → `response.Content.ReadFromJsonAsync<UpdateProductResponse>(ct)`
  - Returns `UpdateProductResponse`
  - Reference: `UpdateIngredientAsync` in `MenuApiClient.cs`

---

## 14) Create or update models, DTOs and mappings

### Update: `ProductModel.cs`
- Add `ToEditRequest()` method:
  ```csharp
  public UpdateProductRequest ToEditRequest()
  {
      return new UpdateProductRequest(
          Name,
          string.IsNullOrWhiteSpace(Description) ? null : Description,
          Price,
          CategoryId,
          IngredientIds);
  }
  ```
- Add `FromResponse(GetProductByIdResponse response)` static factory:
  ```csharp
  public static ProductModel FromResponse(GetProductByIdResponse response)
  {
      return new ProductModel
      {
          Name = response.Name,
          Description = response.Description,
          Price = response.Price,
          CategoryId = response.CategoryId,
          IngredientIds = response.IngredientIds,
      };
  }
  ```
- Reference: `IngredientModel.cs` (`FromResponse`, `ToEditRequest`)

---

## 15) Create or update Blazor components and pages

### Update: `ProductForm.razor`
- Add parameters:
  - `[Parameter] public Guid ProductId { get; set; }` — needed in Edit mode to pass ID to API
  - `[Parameter] public ProductModel? Model { get; set; }` — inject pre-loaded model from `EditProductPage`
- Update `@code` block:
  - Change `_model` field from `readonly` to mutable: `private ProductModel _model = new();`
  - In `OnInitializedAsync`: if `Model is not null`, assign `_model = Model`
  - In `SubmitAsync`: branch on `Mode`:
    - `FormMode.Create` → existing `CreateProductAsync` logic
    - `FormMode.Edit` → call `MenuApiClient.UpdateProductAsync(ProductId, _model.ToEditRequest())`, then `OnSuccess.InvokeAsync(result.Id)`
  - Update error message: `Mode == FormMode.Edit ? "Failed to update product. Please try again." : "Failed to create product. Please try again."`
- Reference: `IngredientForm.razor`

### New page: `EditProductPage.razor`
- Route: `@page "/admin/menu/products/{Id:guid}/edit"`
- Authorization: `@attribute [Authorize(Roles = MenuRoleConstants.Admin)]`
- Injects: `MenuApiClient`, `MenuNavigationService`
- `@code` block:
  - `[Parameter] public Guid Id { get; set; }`
  - `private ProductModel _model = new();`
  - `private bool _isLoading = true;`
  - In `OnInitializedAsync`: call `MenuApiClient.GetProductByIdAsync(Id)`, populate `_model = ProductModel.FromResponse(response)`, set `_isLoading = false`
  - `HandleSuccess(Guid id)` → `MenuNavigation.ToProductsManagement()`
- Template:
  - `<PageTitle>Edit Product</PageTitle>`
  - `<MudPaper>` wrapper with loading indicator, then `<ProductForm Model="_model" Mode="FormMode.Edit" ProductId="Id" OnSuccess="HandleSuccess" />`
- Reference: `EditIngredientPage.razor`

---

## 16) Create Unit tests for Blazor components and services

Skip — Blazor test instructions are `TODO`.
