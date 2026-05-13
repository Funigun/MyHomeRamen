# Menu Products – Feature Slice Structure Refactoring Plan

## Status Overview

| Feature | Status |
|---|---|
| CreateProduct | ⏳ Needs refactoring |
| UpdateProduct | ⏳ Needs refactoring |
| GetProductById | ⏳ Needs refactoring |
| GetProductsByCategory | ⏳ Needs refactoring |
| GetProductsForManage | ⏳ Needs refactoring |
| GetProductByIdForManage | ⏳ Needs refactoring |

---

## Reference Pattern

See `plans/menu-categories-slice-structure-plan.md` for the full pattern description.
Key rule: Request/Response objects live in `Common.Contracts`; the API keeps only a `{Feature}Command` / `{Feature}Query` in the feature folder (no `Models/` or `Policies/` subfolders).

---

## Feature: CreateProduct

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/Menu/Products/`)

- `Requests/CreateProductRequest.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Products.Requests;
  public sealed record CreateProductRequest(
      string Name,
      string? Description,
      decimal Price,
      Guid CategoryId,
      IEnumerable<Guid> IngredientIds,
      IEnumerable<Guid> CustomIngredientIds);
  ```

- `Responses/CreateProductResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Products.Responses;
  public sealed record CreateProductResponse(Guid Id);
  ```

### Api changes (`MyHomeRamen.Api/Menu/Features/Products/CreateProduct/`)

**New files:**
- `CreateProductCommand.cs`
  ```csharp
  // Wraps CreateProductRequest from Common.Contracts
  namespace MyHomeRamen.Api.Menu.Features.Products.CreateProduct;
  public sealed record CreateProductCommand(CreateProductRequest CreateProductRequest) : IRequest<CreateProductResponse>;
  ```

- `CreateProductValidator.cs` (moved & renamed from `Policies/CreateProductValidator.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Products.CreateProduct`
  - Validate against `CreateProductCommand`; access fields via `command.CreateProductRequest.*`
  - Replace `using ...CreateProduct.Models;` with `using MyHomeRamen.Common.Contracts.Menu.Products.Requests;`

- `Mappings.cs` (moved from `Models/Mappings.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Products.CreateProduct`
  - Extension method targets `CreateProductRequest` (from Contracts)

**Modified files:**
- `CreateProductEndpoint.cs`
  - Remove `using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models;`
  - Add `using MyHomeRamen.Common.Contracts.Menu.Products.Requests;`
  - Add `using MyHomeRamen.Common.Contracts.Menu.Products.Responses;`
  - Bind `[FromBody] CreateProductRequest request` (from Contracts)
  - Construct `CreateProductCommand command = new(request);`
  - Change handler type to `IRequestHandler<CreateProductCommand, CreateProductResponse>`
  - Return `Results.Created($"/api/menu/products/{response.Id}", response)`
- `CreateProductHandler.cs`
  - Replace `IRequestHandler<CreateProductRequest, Guid>` with `IRequestHandler<CreateProductCommand, CreateProductResponse>`
  - Access fields via `command.CreateProductRequest.*`
  - Return `new CreateProductResponse(product.Id.Value)` instead of bare `Guid`

**Deleted files:**
- `Models/CreateProductRequest.cs`
- `Models/CreateProductResponse.cs`
- `Models/Mappings.cs`
- `Policies/CreateProductValidator.cs`

---

## Feature: UpdateProduct

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/Menu/Products/`)

- `Requests/UpdateProductRequest.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Products.Requests;
  public sealed record UpdateProductRequest(
      string Name,
      string? Description,
      decimal Price,
      Guid CategoryId,
      IEnumerable<Guid> IngredientIds,
      IEnumerable<Guid> CustomIngredientIds);
  ```

- `Responses/UpdateProductResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Products.Responses;
  public sealed record UpdateProductResponse(Guid Id);
  ```

### Api changes (`MyHomeRamen.Api/Menu/Features/Products/UpdateProduct/`)

**New files:**
- `UpdateProductCommand.cs`
  ```csharp
  // Combines route Id + body request
  namespace MyHomeRamen.Api.Menu.Features.Products.UpdateProduct;
  public sealed record UpdateProductCommand(Guid Id, UpdateProductRequest UpdateProductRequest) : IRequest<UpdateProductResponse>;
  ```

- `UpdateProductValidator.cs` (moved & renamed from `Policies/UpdateProductValidator.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Products.UpdateProduct`
  - Validate against `UpdateProductCommand`; access fields via `command.UpdateProductRequest.*`
  - Route-param `id` now comes from `command.Id` directly — no more `IHttpContextAccessor`

- `Mappings.cs` (moved from `Models/Mappings.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Products.UpdateProduct`

**Modified files:**
- `UpdateProductEndpoint.cs`
  - Remove `using ...UpdateProduct.Models;`
  - Add `using MyHomeRamen.Common.Contracts.Menu.Products.Requests;`
  - Add `using MyHomeRamen.Common.Contracts.Menu.Products.Responses;`
  - Remove `UpdateProductRequestId` — bind `Guid id` from route instead
  - Construct `UpdateProductCommand command = new(id, request);`
  - Change handler type to `IRequestHandler<UpdateProductCommand, UpdateProductResponse>`
- `UpdateProductHandler.cs`
  - Replace `IRequestHandler<UpdateProductRequest, UpdateProductResponse>` with `IRequestHandler<UpdateProductCommand, UpdateProductResponse>`
  - Access `command.Id` and `command.UpdateProductRequest.*`

**Deleted files:**
- `Models/UpdateProductRequest.cs`
- `Models/UpdateProductResponse.cs`
- `Models/UpdateProductRequestId.cs`
- `Models/Mappings.cs`
- `Policies/UpdateProductValidator.cs`

---

## Feature: GetProductById

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/Menu/Products/`)

- `Responses/GetProductByIdResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Products.Responses;
  public sealed record GetProductByIdResponse(
      Guid Id,
      string Name,
      string Description,
      List<ProductIngredientDetailDto> BaseIngredients,
      List<ProductIngredientDetailDto> CustomIngredients);
  ```

- `DTOs/ProductIngredientDetailDto.cs`
  ```csharp
  // Replaces the local IngredientDto in GetProductById/Models
  namespace MyHomeRamen.Common.Contracts.Menu.Products.DTOs;
  public sealed record ProductIngredientDetailDto(Guid Id, string Name, string Description, decimal Price);
  ```

### Api changes (`MyHomeRamen.Api/Menu/Features/Products/GetProductById/`)

**New files:**
- `GetProductByIdQuery.cs`
  ```csharp
  // Replaces GetProductByIdRequest struct
  namespace MyHomeRamen.Api.Menu.Features.Products.GetProductById;
  public record struct GetProductByIdQuery : IRequestId<GetProductByIdQuery>, IRequest<GetProductByIdResponse>;
  ```

- `GetProductByIdValidator.cs` (moved & renamed from `Policies/GetProductByIdValidator.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Products.GetProductById`
  - Validate against `GetProductByIdQuery`

- `Mappings.cs` (moved from `Models/Mappings.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Products.GetProductById`
  - Return type changes to `GetProductByIdResponse` from Contracts

**Modified files:**
- `GetProductByIdEndpoint.cs`
  - Replace `GetProductByIdRequest` → `GetProductByIdQuery`
  - Update usings
- `GetProductByIdHandler.cs`
  - Replace handler signature to use `GetProductByIdQuery` / `GetProductByIdResponse`

**Deleted files:**
- `Models/GetProductByIdRequest.cs`
- `Models/GetProductByIdResponse.cs`
- `Models/IngredientDto.cs`
- `Models/Mappings.cs`
- `Policies/GetProductByIdValidator.cs`

---

## Feature: GetProductsByCategory

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/Menu/Products/`)

- `Requests/GetProductsByCategoryRequest.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Products.Requests;
  public sealed record GetProductsByCategoryRequest(Guid CategoryId);
  ```

- `Responses/GetProductsByCategoryResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Products.Responses;
  public sealed record GetProductsByCategoryResponse(
      Guid Id,
      string Name,
      string Description,
      decimal Price,
      string ImageUrl,
      IEnumerable<ProductIngredientDto> Ingredients);
  ```

- `DTOs/ProductIngredientDto.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Products.DTOs;
  public sealed record ProductIngredientDto(Guid Id, string Name);
  ```

### Api changes (`MyHomeRamen.Api/Menu/Features/Products/GetProductsByCategory/`)

**New files:**
- `GetProductsByCategoryQuery.cs`
  ```csharp
  namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory;
  public sealed record GetProductsByCategoryQuery(Guid CategoryId) : IRequest<IEnumerable<GetProductsByCategoryResponse>>;
  ```

- `GetProductsByCategoryValidator.cs` (moved & renamed from `Policies/`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory`
  - Validate against `GetProductsByCategoryQuery`

- `Mappings.cs` (moved from `Models/Mappings.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory`

**Modified files:**
- `GetProductsByCategoryEndpoint.cs`
  - Bind `[AsParameters] GetProductsByCategoryRequest request` (from Contracts)
  - Construct `GetProductsByCategoryQuery query = new(request.CategoryId);`
  - Update handler type
- `GetProductsByCategoryHandler.cs`
  - Replace handler signature

**Deleted files:**
- `Models/GetProductsByCategoryRequest.cs`
- `Models/GetProductsByCategoryResponse.cs`
- `Models/ProductIngredientDto.cs`
- `Models/Mappings.cs`
- `Policies/GetProductsByCategoryValidator.cs`

---

## Feature: GetProductsForManage

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/Menu/Products/`)

- `Requests/GetProductsForManageRequest.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Products.Requests;
  public sealed record GetProductsForManageRequest(
      string? Name,
      IEnumerable<Guid>? CategoryIds,
      IEnumerable<Guid>? IngredientIds,
      decimal? PriceFrom,
      decimal? PriceTo,
      string? OrderBy);
  // Note: PageParameters stays as an API concern via [AsParameters] – not part of the contract record
  ```

- `Responses/GetProductsForManageResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Products.Responses;
  public sealed record GetProductsForManageResponse(int Page, int PageSize, int TotalCount, IEnumerable<ProductForManageDto> Products);
  ```

- `DTOs/ProductForManageDto.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Products.DTOs;
  public sealed record ProductForManageDto(Guid Id, string Name, string? Description, decimal Price);
  ```

### Api changes (`MyHomeRamen.Api/Menu/Features/Products/GetProductsForManage/`)

**New files:**
- `GetProductsForManageQuery.cs`
  ```csharp
  namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage;
  public sealed record GetProductsForManageQuery(GetProductsForManageRequest Request) : IRequest<GetProductsForManageResponse>
  {
      public PageParameters PageParameters { get; set; }
  }
  ```

- `GetProductsForManageValidator.cs` (moved & renamed from `Policies/`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage`
  - Validate against `GetProductsForManageQuery`; access fields via `query.Request.*`

- `Mappings.cs` (moved from `Models/Mappings.cs`)

**Modified files:**
- `GetProductsForManageEndpoint.cs`
  - Bind `[AsParameters] GetProductsForManageRequest request` (from Contracts) and `[AsParameters] PageParameters pageParameters` separately
  - Construct `GetProductsForManageQuery query = new(request) { PageParameters = pageParameters };`
  - Update handler type
- `GetProductsForManageHandler.cs`
  - Replace handler signature

**Deleted files:**
- `Models/GetProductsForManageRequest.cs`
- `Models/GetProductsForManageResponse.cs`
- `Models/ProductDto.cs`
- `Models/Mappings.cs`
- `Policies/GetProductsForManageValidator.cs`

---

## Feature: GetProductByIdForManage

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/Menu/Products/`)

- `Responses/GetProductByIdForManageResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Products.Responses;
  public sealed record GetProductByIdForManageResponse(
      Guid Id,
      string Name,
      string Description,
      decimal Price,
      Guid CategoryId,
      IEnumerable<Guid> IngredientIds,
      IEnumerable<Guid> CustomIngredientIds);
  ```

### Api changes (`MyHomeRamen.Api/Menu/Features/Products/GetProductByIdForManage/`)

**New files:**
- `GetProductByIdForManageQuery.cs`
  ```csharp
  namespace MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage;
  public record struct GetProductByIdForManageQuery : IRequestId<GetProductByIdForManageQuery>, IRequest<GetProductByIdForManageResponse>;
  ```

- `GetProductByIdForManageValidator.cs` (moved & renamed from `Policies/`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage`
  - Validate against `GetProductByIdForManageQuery`

- `Mappings.cs` (moved from `Models/Mappings.cs`)

**Modified files:**
- `GetProductByIdForManageEndpoint.cs`
  - Replace `GetProductByIdForManageRequest` → `GetProductByIdForManageQuery`
  - Update usings
- `GetProductByIdForManageHandler.cs`
  - Replace handler signature

**Deleted files:**
- `Models/GetProductByIdForManageRequest.cs`
- `Models/GetProductByIdForManageResponse.cs`
- `Models/Mappings.cs`
- `Policies/GetProductByIdForManageValidator.cs`

---

## Integration Tests adjustments (`MyHomeRamen.IntegrationTests/MenuModule/Products/`)

| Test file | Changes |
|---|---|
| `CreateProductTests.cs` | Replace `using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models;` with `using MyHomeRamen.Common.Contracts.Menu.Products.Requests;` and `Responses;` |
| `UpdateProductTests.cs` | Replace `using ...UpdateProduct.Models;` with Contracts equivalents |
| `GetProductByIdTests.cs` | Replace `using ...GetProductById.Models;` with Contracts equivalents; update `IngredientDto` → `ProductIngredientDetailDto` |
| `GetProductsByCategoryTests.cs` | Replace `using ...GetProductsByCategory.Models;` with Contracts equivalents; update `ProductIngredientDto` namespace |
| `GetProductsForManageTests.cs` | Replace `using ...GetProductsForManage.Models;` with Contracts equivalents; update `ProductDto` → `ProductForManageDto` |
| `GetProductByIdForManageTests.cs` | Replace `using ...GetProductByIdForManage.Models;` with Contracts equivalents |

---

## Blazor adjustments (`MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Products/`)

### Requests – replace with Common.Contracts

| Blazor file | Action | Common.Contracts replacement |
|---|---|---|
| `Requests/CreateProductRequest.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Products.Requests.CreateProductRequest` |
| `Requests/UpdateProductRequest.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Products.Requests.UpdateProductRequest` |

### Responses – replace with Common.Contracts

| Blazor file | Action | Common.Contracts replacement |
|---|---|---|
| `Responses/CreateProductResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Products.Responses.CreateProductResponse` |
| `Responses/UpdateProductResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Products.Responses.UpdateProductResponse` |
| `Responses/GetProductByIdResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Products.Responses.GetProductByIdResponse` |
| `Responses/GetProductsByCategory\GetProductsByCategoryResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Products.Responses.GetProductsByCategoryResponse` |
| `Responses/GetProductsForManageResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Products.Responses.GetProductsForManageResponse` |
| `Responses/GetProductByIdForManageResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Products.Responses.GetProductByIdForManageResponse` |
| `Responses/ProductIngredientDto.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Products.DTOs.ProductIngredientDto` |
| `Responses/IngredientDto.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Products.DTOs.ProductIngredientDetailDto` |
| `Responses/ProductForManageItemResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Products.DTOs.ProductForManageDto` |

### Service client (`Features/Menu/Common/Services/MenuApiClient.cs`)
- Update all `using` directives to reference `MyHomeRamen.Common.Contracts.Menu.Products.*`
- All method signatures / deserialization target types change accordingly

### Components / razor files
- Update `@using` directives that reference the old Blazor-local response/request types
