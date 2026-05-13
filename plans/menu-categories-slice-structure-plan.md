# Menu Categories – Feature Slice Structure Refactoring Plan

## Status Overview

| Feature | Status |
|---|---|
| CreateCategory | ✅ Already refactored |
| GetCategoriesByType | ✅ Already refactored |
| DeleteCategory | ⏳ Needs refactoring |
| GetMenuCategories | ⏳ Needs refactoring |
| UpdateCategoriesOrder | ⏳ Needs refactoring |

---

## Reference Pattern (already implemented in CreateCategory / GetCategoriesByType)

### Common.Contracts (`MyHomeRamen.Common.Contracts/Menu/Categories/`)
- `Requests/{Feature}Request.cs` – the HTTP contract object used by clients (Blazor, tests)
- `Responses/{Feature}Response.cs` – the HTTP contract object returned to clients
- `Validators/` – shared primitive validators reused across features

### Api feature folder (`MyHomeRamen.Api/Menu/Features/Categories/{Feature}/`)
- `{Feature}Command.cs` / `{Feature}Query.cs` – API-internal request object wrapping or mirroring the contract
- `{Feature}Handler.cs` – business logic handler
- `{Feature}Endpoint.cs` – maps route, binds body/route params, constructs Command/Query
- `{Feature}Validator.cs` – FluentValidation validator targeting the Command/Query (moved from `Policies/`)
- `Mappings.cs` – internal mapping helpers
- **No `Models/` subfolder**
- **No `Policies/` subfolder**

---

## Feature: DeleteCategory

### Common.Contracts (`MyHomeRamen.Common.Contracts/Menu/Categories/`)
No request/response contract needed — the DELETE is route-param-only (`/api/menu/categories/{id}`) and returns `204 No Content`.

### Api changes (`MyHomeRamen.Api/Menu/Features/Categories/DeleteCategory/`)

**New files:**
- `DeleteCategoryCommand.cs`
  ```csharp
  // Replaces DeleteCategoryRequest as the IRequest handler target
  // Implements IRequestId<DeleteCategoryCommand> (API-internal interface) and IRequest<IResult>
  namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory;

  public record struct DeleteCategoryCommand : IRequestId<DeleteCategoryCommand>, IRequest<IResult>
  {
      public Guid Id { get; set; }
  }
  ```

- `DeleteCategoryValidator.cs` (moved & renamed from `Policies/DeleteCategoryValidator.cs`)
  ```csharp
  // Namespace changes from:
  //   MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory.Policies
  // to:
  //   MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory
  // Type reference changes from DeleteCategoryRequest → DeleteCategoryCommand
  ```

**Modified files:**
- `DeleteCategoryEndpoint.cs`
  - Remove `using MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory.Models;`
  - Replace all `DeleteCategoryRequest` references with `DeleteCategoryCommand`
  - Namespace stays the same
- `DeleteCategoryHandler.cs`
  - Remove `using MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory.Models;`
  - Replace `IRequestHandler<DeleteCategoryRequest, IResult>` with `IRequestHandler<DeleteCategoryCommand, IResult>`
  - Replace `DeleteCategoryRequest` parameter type with `DeleteCategoryCommand`

**Deleted files:**
- `Models/DeleteCategoryRequest.cs`
- `Policies/DeleteCategoryValidator.cs`

---

## Feature: GetMenuCategories

### Common.Contracts (`MyHomeRamen.Common.Contracts/Menu/Categories/`)

**New files:**
- `Responses/GetMenuCategoriesResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
  public sealed record GetMenuCategoriesResponse(Guid Id, string Name);
  ```

### Api changes (`MyHomeRamen.Api/Menu/Features/Categories/GetMenuCategories/`)

**New files:**
- `GetMenuCategoriesQuery.cs`
  ```csharp
  // Replaces GetMenuCategoriesRequest
  // No validator needed (no input params)
  namespace MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories;

  public sealed record GetMenuCategoriesQuery : IRequest<IEnumerable<GetMenuCategoriesResponse>>;
  ```

- `Mappings.cs` (moved from `Models/Mappings.cs`)
  ```csharp
  // Namespace changes from:
  //   MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories.Models
  // to:
  //   MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories
  // Return type changes from local GetMenuCategoriesResponse → Common.Contracts version
  using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
  ```

**Modified files:**
- `GetMenuCategoriesEndpoint.cs`
  - Remove `using MyHomeRamen.Api.Menu.Features.Categories.GetMenuCategories.Models;`
  - Add `using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;`
  - Replace `GetMenuCategoriesRequest` with `GetMenuCategoriesQuery`
  - Replace `IEnumerable<GetMenuCategoriesResponse>` (local) with the Contracts version
- `GetMenuCategoriesHandler.cs`
  - Same using/type adjustments

**Deleted files:**
- `Models/GetMenuCategoriesRequest.cs`
- `Models/GetMenuCategoriesResponse.cs`
- `Models/Mappings.cs`

---

## Feature: UpdateCategoriesOrder

### Common.Contracts (`MyHomeRamen.Common.Contracts/Menu/Categories/`)

**New files:**
- `Requests/UpdateCategoriesOrderRequest.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Categories.Requests;
  public sealed record UpdateCategoriesOrderRequest(IEnumerable<CategoryOrderItemDto> Items);
  ```
- `DTOs/CategoryOrderItemDto.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Categories.DTOs;
  public sealed record CategoryOrderItemDto(Guid Id, int SortOrder);
  ```

### Api changes (`MyHomeRamen.Api/Menu/Features/Categories/UpdateCategoriesOrder/`)

**New files:**
- `UpdateCategoriesOrderCommand.cs`
  ```csharp
  // Wraps the contract Request, implements IRequest (no response body)
  namespace MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder;

  public sealed record UpdateCategoriesOrderCommand(UpdateCategoriesOrderRequest UpdateCategoriesOrderRequest) : IRequest;
  ```

- `UpdateCategoriesOrderValidator.cs` (moved & renamed from `Policies/UpdateCategoriesOrderValidator.cs`)
  ```csharp
  // Namespace changes from:
  //   MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder.Policies
  // to:
  //   MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder
  // Validate against UpdateCategoriesOrderCommand, accessing fields via command.UpdateCategoriesOrderRequest
  // CategoryOrderItemDto referenced from Common.Contracts
  ```

- `Mappings.cs` (moved from `Models/Mappings.cs` if any)

**Modified files:**
- `UpdateCategoriesOrderEndpoint.cs`
  - Remove `using MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder.Models;`
  - Add `using MyHomeRamen.Common.Contracts.Menu.Categories.Requests;`
  - Bind `[FromBody] UpdateCategoriesOrderRequest request` (from Contracts)
  - Construct `UpdateCategoriesOrderCommand command = new(request);`
  - Change handler type to `IRequestHandler<UpdateCategoriesOrderCommand>`
- `UpdateCategoriesOrderHandler.cs`
  - Replace `IRequestHandler<UpdateCategoriesOrderRequest>` with `IRequestHandler<UpdateCategoriesOrderCommand>`
  - Access data via `command.UpdateCategoriesOrderRequest.Items`

**Deleted files:**
- `Models/UpdateCategoriesOrderRequest.cs`
- `Models/CategoryOrderItemDto.cs`
- `Policies/UpdateCategoriesOrderValidator.cs`

---

## Integration Tests adjustments (`MyHomeRamen.IntegrationTests/MenuModule/Categories/`)

| Test file | Changes |
|---|---|
| `GetCategoriesByTypeTests.cs` | Remove stale `using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Models;` (if any models are no longer local) |
| `GetMenuCategoriesTests.cs` | Replace `using ...GetMenuCategories.Models` with `using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;` |
| `DeleteCategoryTests.cs` | No request object – no change needed |
| `UpdateCategoriesOrderTests.cs` | Replace `using ...UpdateCategoriesOrder.Models` with `using MyHomeRamen.Common.Contracts.Menu.Categories.Requests;` and update `CategoryOrderItemDto` namespace |

---

## Blazor adjustments (`MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Categories/`)

| Blazor file | Action |
|---|---|
| `Requests/UpdateCategoriesOrderRequest.cs` | **Delete** – replace usages with `MyHomeRamen.Common.Contracts.Menu.Categories.Requests.UpdateCategoriesOrderRequest` |
| `Requests/CategoryOrderItem.cs` | **Delete** – replace usages with `MyHomeRamen.Common.Contracts.Menu.Categories.DTOs.CategoryOrderItemDto` |
| `Responses/GetMenuCategoriesResponse.cs` | **Delete** – replace usages with `MyHomeRamen.Common.Contracts.Menu.Categories.Responses.GetMenuCategoriesResponse` |
| `MenuApiClient.cs` | Update usings and type references to use Common.Contracts types |
| Components/razor files that bind these types | Update `@using` directives accordingly |

> **Note:** `GetMenuCategories` Blazor response fields are identical (`Guid Id, string Name`) — a direct replacement with the Contracts type is safe.
