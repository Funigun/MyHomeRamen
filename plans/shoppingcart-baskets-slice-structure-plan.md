# Shopping Cart Baskets – Feature Slice Structure Refactoring Plan

## Status Overview

| Feature | Status |
|---|---|
| AddItemToBasket | ⏳ Needs refactoring |
| GetCurrentBasketSummary | ⏳ Needs refactoring |
| GetCurrentBasketDetails | ⏳ Needs refactoring |

---

## Reference Pattern

See `plans/menu-categories-slice-structure-plan.md` for the full pattern description.

---

## Feature: AddItemToBasket

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/ShoppingCart/Baskets/`)

- `Requests/AddItemToBasketRequest.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;
  public sealed record AddItemToBasketRequest(
      Guid ProductId,
      int Quantity,
      List<BasketIngredientDto> BaseIngredients,
      List<BasketIngredientDto> CustomIngredients,
      string? Comments);
  ```

- `DTOs/BasketIngredientDto.cs`
  ```csharp
  // Replaces IngredientRequestDto in both API and Blazor
  namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;
  public sealed record BasketIngredientDto(Guid Id, int Quantity);
  ```

- `Responses/AddItemToBasketResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
  public sealed record AddItemToBasketResponse(Guid BasketId, Guid BasketItemId);
  ```

### Api changes (`MyHomeRamen.Api/ShoppingCart/Features/Baskets/AddItemToBasket/`)

**New files:**
- `AddItemToBasketCommand.cs`
  ```csharp
  namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket;
  public sealed record AddItemToBasketCommand(AddItemToBasketRequest AddItemToBasketRequest) : IRequest<AddItemToBasketResponse>;
  ```

- `AddItemToBasketValidator.cs` (moved & renamed from `Policies/AddItemToBasketValidator.cs`)
  - Namespace: `MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket`
  - Validate against `AddItemToBasketCommand`; access fields via `command.AddItemToBasketRequest.*`
  - `IngredientRequestDto` references updated to `BasketIngredientDto` from Contracts

- `Mappings.cs` (moved from `Models/Mappings.cs`)
  - Namespace: `MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket`
  - Extension method targets `AddItemToBasketRequest` (from Contracts); `IngredientRequestDto` → `BasketIngredientDto`

**Modified files:**
- `AddItemToBasketEndpoint.cs`
  - Remove `using MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket.Models;`
  - Add `using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;`
  - Add `using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;`
  - Bind `[FromBody] AddItemToBasketRequest request` (from Contracts)
  - Construct `AddItemToBasketCommand command = new(request);`
  - Change handler type to `IRequestHandler<AddItemToBasketCommand, AddItemToBasketResponse>`
- `AddItemToBasketHandler.cs`
  - Replace `IRequestHandler<AddItemToBasketRequest, AddItemToBasketResponse>` with `IRequestHandler<AddItemToBasketCommand, AddItemToBasketResponse>`
  - Access fields via `command.AddItemToBasketRequest.*`
  - `IngredientRequestDto` → `BasketIngredientDto`

**Deleted files:**
- `Models/AddItemToBasketRequest.cs`
- `Models/AddItemToBasketResponse.cs`
- `Models/IngredientRequestDto.cs`
- `Models/Mappings.cs`
- `Policies/AddItemToBasketValidator.cs`

---

## Feature: GetCurrentBasketSummary

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/ShoppingCart/Baskets/`)

- `Responses/GetCurrentBasketSummaryResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
  public sealed record GetCurrentBasketSummaryResponse(Guid Id, IEnumerable<BasketSummaryItemDto> Items);
  ```

- `DTOs/BasketSummaryItemDto.cs`
  ```csharp
  // Replaces local BasketItemDto in GetCurrentBasketSummary/Models
  namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;
  public sealed record BasketSummaryItemDto(
      Guid Id,
      string ProductName,
      string ProductImageUrl,
      int Quantity,
      decimal Price);
  ```

### Api changes (`MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetCurrentBasketSummary/`)

**New files:**
- `GetCurrentBasketSummaryQuery.cs`
  ```csharp
  // Replaces GetCurrentBasketSummaryRequest (no input params)
  namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketSummary;
  public sealed record GetCurrentBasketSummaryQuery : IRequest<GetCurrentBasketSummaryResponse>;
  ```

- `Mappings.cs` (moved from `Models/Mappings.cs`)
  - Namespace: `MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketSummary`
  - Return type changes to use Contracts types; `BasketItemDto` → `BasketSummaryItemDto`

**Modified files:**
- `GetCurrentBasketSummaryEndpoint.cs`
  - Replace `GetCurrentBasketSummaryRequest` → `GetCurrentBasketSummaryQuery`
  - Update usings
- `GetCurrentBasketSummaryHandler.cs`
  - Replace handler signature

**Deleted files:**
- `Models/GetCurrentBasketSummaryRequest.cs`
- `Models/GetCurrentBasketSummaryResponse.cs`
- `Models/BasketItemDto.cs`
- `Models/Mappings.cs`

---

## Feature: GetCurrentBasketDetails

### Common.Contracts – new files (`MyHomeRamen.Common.Contracts/ShoppingCart/Baskets/`)

- `Responses/GetCurrentBasketDetailsResponse.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses;
  public sealed record GetCurrentBasketDetailsResponse(Guid Id, IEnumerable<BasketDetailsItemDto> Items);
  ```

- `DTOs/BasketDetailsItemDto.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;
  public sealed record BasketDetailsItemDto(
      Guid Id,
      int Quantity,
      decimal Price,
      string? Comment,
      BasketDetailsItemProductDto Product);
  ```

- `DTOs/BasketDetailsItemProductDto.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;
  public sealed record BasketDetailsItemProductDto(
      Guid Id,
      string Name,
      string Description,
      string ImageUrl,
      IEnumerable<BasketDetailsIngredientDto> BaseIngredients,
      IEnumerable<BasketDetailsIngredientDto> CustomIngredients);
  ```

- `DTOs/BasketDetailsIngredientDto.cs`
  ```csharp
  namespace MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs;
  public sealed record BasketDetailsIngredientDto(Guid Id, string Name);
  ```

### Api changes (`MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetCurrentBasketDetails/`)

**New files:**
- `GetCurrentBasketDetailsQuery.cs`
  ```csharp
  // Replaces GetCurrentBasketDetailsRequest (no input params)
  namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails;
  public sealed record GetCurrentBasketDetailsQuery : IRequest<GetCurrentBasketDetailsResponse?>;
  ```

- `Mappings.cs` (moved from `Models/Mappings.cs`)
  - Namespace: `MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetCurrentBasketDetails`
  - All local DTO types replaced with Contracts equivalents:
    - `BasketItemDto` → `BasketDetailsItemDto`
    - `BasketItemProductDto` → `BasketDetailsItemProductDto`
    - `BasketItemIngredientDto` → `BasketDetailsIngredientDto`

**Modified files:**
- `GetCurrentBasketDetailsEndpoint.cs`
  - Replace `GetCurrentBasketDetailsRequest` → `GetCurrentBasketDetailsQuery`
  - Update usings
- `GetCurrentBasketDetailsHandler.cs`
  - Replace handler signature; update all DTO type references

**Deleted files:**
- `Models/GetCurrentBasketDetailsRequest.cs`
- `Models/GetCurrentBasketDetailsResponse.cs`
- `Models/BasketItemDto.cs`
- `Models/BasketItemProductDto.cs`
- `Models/BasketItemIngredientDto.cs`
- `Models/Mappings.cs`

---

## Integration Tests adjustments (`MyHomeRamen.IntegrationTests/ShoppingCartModule/Baskets/`)

| Test file | Changes |
|---|---|
| `AddItemToBasketTests.cs` | Replace `using MyHomeRamen.Api.ShoppingCart.Features.Baskets.AddItemToBasket.Models;` with `using MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests;` and `Responses;`; update `IngredientRequestDto` → `BasketIngredientDto` |
| `GetCurrentBasketSummaryTests.cs` | Replace `using ...GetCurrentBasketSummary.Models;` with Contracts equivalents; update `BasketItemDto` → `BasketSummaryItemDto` |
| `GetCurrentBasketDetailsTests.cs` | Replace `using ...GetCurrentBasketDetails.Models;` with Contracts equivalents; update all DTO type names |

---

## Blazor adjustments (`MyHomeRamen.Blazor/MyHomeRamen.Blazor/Features/ShoppingCart/Baskets/`)

### Requests – replace with Common.Contracts

| Blazor file | Action | Common.Contracts replacement |
|---|---|---|
| `Requests/AddItemToBasketRequest.cs` | **Delete** | `MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Requests.AddItemToBasketRequest` |
| `Requests/IngredientRequestDto.cs` | **Delete** | `MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs.BasketIngredientDto` |

### Responses – replace with Common.Contracts

| Blazor file | Action | Common.Contracts replacement |
|---|---|---|
| `Responses/AddItemToBasketResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses.AddItemToBasketResponse` |
| `Responses/GetCurrentBasketSummaryResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.Responses.GetCurrentBasketSummaryResponse` |
| `Responses/BasketItemResponse.cs` | **Delete** | `MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.DTOs.BasketSummaryItemDto` |

> **Note:** `GetCurrentBasketDetails` does not appear to have a dedicated Blazor response file.
> If the `ShoppingCartApiClient` deserializes directly into API-local types, those references must be updated to the Contracts DTOs.

### Service client (`Features/ShoppingCart/Common/Services/ShoppingCartApiClient.cs`)
- Update all `using` directives to reference `MyHomeRamen.Common.Contracts.ShoppingCart.Baskets.*`
- All method signatures / deserialization target types change accordingly

### Components / razor files
- Update `@using` directives that reference the old Blazor-local response/request types
- Any model or validator classes (`ProductCustomizationModel`, `ProductCustomizationValidator`, `IngredientCustomizationModel`) that reference `IngredientRequestDto` must be updated to `BasketIngredientDto`
