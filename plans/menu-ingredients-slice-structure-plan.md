# Menu Ingredients – Feature Slice Structure Refactoring Plan

## Status Overview

| Feature | Status |
|---|---|
| CreateIngredient | ⏳ Needs refactoring |
| UpdateIngredient | ⏳ Needs refactoring |
| DeleteIngredient | ⏳ Needs refactoring |
| GetIngredientById | ⏳ Needs refactoring |
| GetIngredientsForManage | ⏳ Needs refactoring |
| GetIngredientsForDropdown | ⏳ Needs refactoring |

---

## Reference Pattern

See `plans/menu-categories-slice-structure-plan.md` for the full pattern description.

---

## Feature: CreateIngredient

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/Menu/Ingredients/`)

- `Requests/CreateIngredientRequest.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
  public sealed record CreateIngredientRequest(
      string Name,
      string Description,
      decimal Price,
      IEnumerable<Guid> CategoryIds);
  ```

- `Responses/CreateIngredientResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
  public sealed record CreateIngredientResponse(Guid Id);
  ```

### Api changes (`MyHomeRamen.Api/Menu/Features/Ingredients/CreateIngredient/`)

**New files:**
- `CreateIngredientCommand.cs`
  ```csharp
  namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient;
  public sealed record CreateIngredientCommand(CreateIngredientRequest CreateIngredientRequest) : IRequest<CreateIngredientResponse>;
  ```

- `CreateIngredientValidator.cs` (moved & renamed from `Policies/CreateIngredientValidator.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient`
  - Validate against `CreateIngredientCommand`; access fields via `command.CreateIngredientRequest.*`
  - Replace `using ...CreateIngredient.Models;` with `using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;`

- `Mappings.cs` (moved from `Models/Mappings.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient`
  - Extension method targets `CreateIngredientRequest` (from Contracts)

**Modified files:**
- `CreateIngredientEndpoint.cs`
  - Remove `using MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;`
  - Add `using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;`
  - Add `using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;`
  - Bind `[FromBody] CreateIngredientRequest request` (from Contracts)
  - Construct `CreateIngredientCommand command = new(request);`
  - Change handler type to `IRequestHandler<CreateIngredientCommand, CreateIngredientResponse>`
  - Return `Results.Created($"/api/menu/ingredients/{response.Id}", response)`
- `CreateIngredientHandler.cs`
  - Replace `IRequestHandler<CreateIngredientRequest, Guid>` with `IRequestHandler<CreateIngredientCommand, CreateIngredientResponse>`
  - Access fields via `command.CreateIngredientRequest.*`
  - Return `new CreateIngredientResponse(ingredient.Id.Value)` instead of bare `Guid`

**Deleted files:**
- `Models/CreateIngredientRequest.cs`
- `Models/CreateIngredientResponse.cs`
- `Models/Mappings.cs`
- `Policies/CreateIngredientValidator.cs`

---

## Feature: UpdateIngredient

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/Menu/Ingredients/`)

- `Requests/UpdateIngredientRequest.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
  public sealed record UpdateIngredientRequest(
      string Name,
      string Description,
      decimal Price,
      IEnumerable<Guid> CategoryIds);
  ```

- `Responses/UpdateIngredientResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
  public sealed record UpdateIngredientResponse(Guid Id);
  ```

### Api changes (`MyHomeRamen.Api/Menu/Features/Ingredients/UpdateIngredient/`)

**New files:**
- `UpdateIngredientCommand.cs`
  ```csharp
  // Combines route Id + body request
  namespace MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient;
  public sealed record UpdateIngredientCommand(Guid Id, UpdateIngredientRequest UpdateIngredientRequest) : IRequest<UpdateIngredientResponse>;
  ```

- `UpdateIngredientValidator.cs` (moved & renamed from `Policies/UpdateIngredientValidator.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient`
  - Validate against `UpdateIngredientCommand`; access fields via `command.UpdateIngredientRequest.*`
  - Route-param `id` now comes from `command.Id` directly — eliminates `IHttpContextAccessor` dependency

- `Mappings.cs` (moved from `Models/Mappings.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient`

**Modified files:**
- `UpdateIngredientEndpoint.cs`
  - Remove `using ...UpdateIngredient.Models;`
  - Add `using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;`
  - Add `using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;`
  - Remove `UpdateIngredientIRequestId` — bind `Guid id` from route directly
  - Construct `UpdateIngredientCommand command = new(id, request);`
  - Change handler type to `IRequestHandler<UpdateIngredientCommand, UpdateIngredientResponse>`
- `UpdateIngredientHandler.cs`
  - Replace handler signature to use `UpdateIngredientCommand` / `UpdateIngredientResponse`
  - Access `command.Id` and `command.UpdateIngredientRequest.*`

**Deleted files:**
- `Models/UpdateIngredientRequest.cs`
- `Models/UpdateIngredientResponse.cs`
- `Models/UpdateIngredientIRequestId.cs`
- `Models/Mappings.cs`
- `Policies/UpdateIngredientValidator.cs`

---

## Feature: DeleteIngredient

### Common.Contracts
No request/response contract needed — the DELETE is route-param-only (`/api/menu/ingredients/{id}`) and returns `204 No Content`.

### Api changes (`MyHomeRamen.Api/Menu/Features/Ingredients/DeleteIngredient/`)

**New files:**
- `DeleteIngredientCommand.cs`
  ```csharp
  namespace MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient;
  public record struct DeleteIngredientCommand : IRequestId<DeleteIngredientCommand>, IRequest<IResult>
  {
      public Guid Id { get; set; }
  }
  ```

- `DeleteIngredientValidator.cs` (moved & renamed from `Policies/DeleteIngredientValidator.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient`
  - Validate against `DeleteIngredientCommand` instead of `DeleteIngredientRequest`

**Modified files:**
- `DeleteIngredientEndpoint.cs`
  - Remove `using ...DeleteIngredient.Models;`
  - Replace `DeleteIngredientRequest` with `DeleteIngredientCommand`
- `DeleteIngredientHandler.cs`
  - Replace `IRequestHandler<DeleteIngredientRequest, IResult>` with `IRequestHandler<DeleteIngredientCommand, IResult>`

**Deleted files:**
- `Models/DeleteIngredientRequest.cs`
- `Policies/DeleteIngredientValidator.cs`

---

## Feature: GetIngredientById

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/Menu/Ingredients/`)

- `Responses/GetIngredientByIdResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
  public sealed record GetIngredientByIdResponse(
      Guid Id,
      string Name,
      string Description,
      decimal Price,
      IEnumerable<Guid> CategoryIds);
  ```

### Api changes (`MyHomeRamen.Api/Menu/Features/Ingredients/GetIngredientById/`)

**New files:**
- `GetIngredientByIdQuery.cs`
  ```csharp
  namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById;
  public record struct GetIngredientByIdQuery : IRequestId<GetIngredientByIdQuery>, IRequest<GetIngredientByIdResponse>;
  ```

- `GetIngredientByIdValidator.cs` (moved & renamed from `Policies/`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById`
  - Validate against `GetIngredientByIdQuery`

- `Mappings.cs` (moved from `Models/Mappings.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById`
  - Return type changes to `GetIngredientByIdResponse` from Contracts

**Modified files:**
- `GetIngredientByIdEndpoint.cs`
  - Replace `GetIngredientByIdRequest` → `GetIngredientByIdQuery`
  - Update usings
- `GetIngredientByIdHandler.cs`
  - Replace handler signature

**Deleted files:**
- `Models/GetIngredientByIdRequest.cs`
- `Models/GetIngredientByIdResponse.cs`
- `Models/Mappings.cs`
- `Policies/GetIngredientByIdValidator.cs`

---

## Feature: GetIngredientsForManage

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/Menu/Ingredients/`)

- `Requests/GetIngredientsForManageRequest.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
  public sealed record GetIngredientsForManageRequest(string? Name, IEnumerable<Guid>? CategoryIds);
  // Note: PageParameters stays as an API concern via [AsParameters] – not part of the contract record
  ```

- `Responses/GetIngredientsForManageResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
  public sealed record GetIngredientsForManageResponse(int Page, int PageSize, int TotalCount, IEnumerable<IngredientForManageDto> Ingredients);
  ```

- `DTOs/IngredientForManageDto.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.DTOs;
  public sealed record IngredientForManageDto(Guid Id, string Name, string Description);
  ```

### Api changes (`MyHomeRamen.Api/Menu/Features/Ingredients/GetIngredientsForManage/`)

**New files:**
- `GetIngredientsForManageQuery.cs`
  ```csharp
  namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage;
  public sealed record GetIngredientsForManageQuery(GetIngredientsForManageRequest Request) : IRequest<GetIngredientsForManageResponse>
  {
      public PageParameters PageParameters { get; set; }
  }
  ```

- `GetIngredientsForManageValidator.cs` (moved & renamed from `Policies/`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage`
  - Validate against `GetIngredientsForManageQuery`; access fields via `query.Request.*`

- `Mappings.cs` (moved from `Models/Mappings.cs`)

**Modified files:**
- `GetIngredientsForManageEndpoint.cs`
  - Bind `[AsParameters] GetIngredientsForManageRequest request` (from Contracts) and `[AsParameters] PageParameters pageParameters` separately
  - Construct `GetIngredientsForManageQuery query = new(request) { PageParameters = pageParameters };`
  - Update handler type
- `GetIngredientsForManageHandler.cs`
  - Replace handler signature

**Deleted files:**
- `Models/GetIngredientsForManageRequest.cs`
- `Models/GetIngredientsForManageResponse.cs`
- `Models/IngredientDto.cs`
- `Models/Mappings.cs`
- `Policies/GetIngredientsForManageValidator.cs`

---

## Feature: GetIngredientsForDropdown

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/Menu/Ingredients/`)

- `Responses/GetIngredientsForDropdownResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
  public sealed record GetIngredientsForDropdownResponse(Guid Id, string Name);
  ```

### Api changes (`MyHomeRamen.Api/Menu/Features/Ingredients/GetIngredientsForDropdown/`)

**New files:**
- `GetIngredientsForDropdownQuery.cs`
  ```csharp
  // Replaces GetIngredientsForDropdownRequest (no input params, no validator)
  namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForDropdown;
  public sealed record GetIngredientsForDropdownQuery : IRequest<IEnumerable<GetIngredientsForDropdownResponse>>;
  ```

- `Mappings.cs` (moved from `Models/Mappings.cs`)
  - Namespace: `MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForDropdown`
  - Return type changes to `GetIngredientsForDropdownResponse` from Contracts

**Modified files:**
- `GetIngredientsForDropdownEndpoint.cs`
  - Replace `GetIngredientsForDropdownRequest` → `GetIngredientsForDropdownQuery`
  - Update usings
- `GetIngredientsForDropdownHandler.cs`
  - Replace handler signature

**Deleted files:**
- `Models/GetIngredientsForDropdownRequest.cs`
- `Models/GetIngredientsForDropdownResponse.cs`
- `Models/Mappings.cs`

---

## Integration Tests adjustments (`MyHomeRamen.IntegrationTests/MenuModule/Ingredients/`)

| Test file | Changes |
|---|---|
| `GetIngredientByIdTests.cs` | Replace `using ...GetIngredientById.Models;` with `using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;` |
| `GetIngredientsForDropdownTests.cs` | Replace `using ...GetIngredientsForDropdown.Models;` with Contracts equivalent |
| `GetIngredientsForManageTests.cs` | Replace `using ...GetIngredientsForManage.Models;` with Contracts equivalents; update `IngredientDto` → `IngredientForManageDto` |
| `DeleteIngredientTests.cs` | No request object – no change needed |
| `UpdateIngredientTests.cs` | Replace `using ...UpdateIngredient.Models;` with Contracts equivalents |

---

## Blazor adjustments (`MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/Menu/Ingredients/`)

### Requests – replace with Common.Contracts

| Blazor file | Action | Common.Contracts replacement |
|---|---|---|
| `Requests/CreateIngredientRequest.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests.CreateIngredientRequest` |
| `Requests/UpdateIngredientRequest.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests.UpdateIngredientRequest` |

### Responses – replace with Common.Contracts

| Blazor file | Action | Common.Contracts replacement |
|---|---|---|
| `Responses/CreateIngredientResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses.CreateIngredientResponse` |
| `Responses/UpdateIngredientResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses.UpdateIngredientResponse` |
| `Responses/GetIngredientByIdResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses.GetIngredientByIdResponse` |
| `Responses/GetIngredientsForManageResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses.GetIngredientsForManageResponse` |
| `Responses/IngredientForManageItemResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.Menu.Ingredients.DTOs.IngredientForManageDto` |

### Misplaced Blazor file

| Blazor file | Action | Common.Contracts replacement |
|---|---|---|
| `Features/Menu/Categories/Responses/GetIngredientsForDropdownResponse.cs` | **Delete** (wrong folder) | `MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses.GetIngredientsForDropdownResponse` |

### Service client (`Features/Menu/Common/Services/MenuApiClient.cs`)
- Update all `using` directives to reference `MyHomeRamen.Common.Contracts.Menu.Ingredients.*`
- All method signatures / deserialization target types change accordingly

### Components / razor files
- Update `@using` directives that reference the old Blazor-local response/request types
