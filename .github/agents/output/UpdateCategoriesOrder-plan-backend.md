# Feature Implementation Plan — UpdateCategoriesOrder (Backend)

- **Date**: 2025-01-27
- **Feature**: UpdateCategoriesOrder — batch update sort order for categories
- **Module**: Menu
- **Authorization**: RestaurantManagerPolicy (Admin role only)

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/Menu/Features/Categories/
└── UpdateCategoriesOrder/
    ├── Models/
    │   ├── UpdateCategoriesOrderRequest.cs
    │   └── CategoryOrderItemDto.cs
    ├── Policies/
    │   └── UpdateCategoriesOrderValidator.cs
    ├── UpdateCategoriesOrderEndpoint.cs
    └── UpdateCategoriesOrderHandler.cs
```

---

## 2) Create primitive rules and contracts

No new primitive validators needed — reuse existing `CategorySortOrderValidator` from `MyHomeRamen.Common.Contracts.Menu.Categories`.

---

## 3) Domain changes

### File: `MyHomeRamen.Domain/Menu/Categories/Category.cs`
- Add public method `UpdateSortOrder(int newSortOrder)`:
  - Sets `SortOrder = newSortOrder`
  - Calls `CategoryValidator.CheckSortOrder(this)` to validate the new value (need to make `CheckSortOrder` available or add a dedicated `ValidateSortOrder` method)
  - Consistent with the existing `Create` factory pattern where validation runs before state is committed

### File: `MyHomeRamen.Domain/Menu/Categories/CategoryValidator.cs`
- Extract `CheckSortOrder` into an `internal static` method that can be called independently from both `Validate` and the new `UpdateSortOrder` method
- Current `CheckSortOrder` is already `private static` — change visibility to `internal static` so `Category.UpdateSortOrder` can call it directly

---

## 4) Create models, DTOs and mappings

### File: `MyHomeRamen.Api/Menu/Features/Categories/UpdateCategoriesOrder/Models/CategoryOrderItemDto.cs`
```csharp
public sealed record CategoryOrderItemDto(Guid Id, int SortOrder);
```

### File: `MyHomeRamen.Api/Menu/Features/Categories/UpdateCategoriesOrder/Models/UpdateCategoriesOrderRequest.cs`
```csharp
public sealed record UpdateCategoriesOrderRequest(
    List<CategoryOrderItemDto> Items) : IRequest;
```

No `Mappings.cs` needed — no response DTO (204 No Content) and no domain entity creation from request. The handler applies `UpdateSortOrder` directly to fetched entities.

---

## 5) Create IRequestHandler implementation

### File: `MyHomeRamen.Api/Menu/Features/Categories/UpdateCategoriesOrder/UpdateCategoriesOrderHandler.cs`
- Implements `IRequestHandler<UpdateCategoriesOrderRequest>`
- Inject `IMenuDbContext`
- Steps:
  1. Extract all category IDs from `request.Items`
  2. Fetch all matching `Category` entities via `dbContext.Categories.GetByIds(ids, ct)` (existing DbExtension)
  3. For each item in request, find the matching entity and call `entity.UpdateSortOrder(item.SortOrder)`
  4. Call `dbContext.SaveChangesAsync(ct)` — single round-trip batch update

---

## 6) Create IEndpoint implementation

### File: `MyHomeRamen.Api/Menu/Features/Categories/UpdateCategoriesOrder/UpdateCategoriesOrderEndpoint.cs`
- Implements `IEndpoint`
- `GroupName = "Menu"`
- Maps `PUT "categories/order"` using `MapStandardValidatedPut<UpdateCategoriesOrderRequest>`
- `WithName("UpdateCategoriesOrderEndpoint")`
- `WithDescription("Updates the sort order of multiple categories in a single batch operation.")`
- `.RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy)`
- Handler method:
  - Accepts `[FromBody] UpdateCategoriesOrderRequest request`, `[FromServices] IRequestHandler<UpdateCategoriesOrderRequest> handler`
  - Calls `handler.Handle(request, ct)`
  - Returns `Results.NoContent()`

---

## 7) Create validation policy

### File: `MyHomeRamen.Api/Menu/Features/Categories/UpdateCategoriesOrder/Policies/UpdateCategoriesOrderValidator.cs`
- Extends `AbstractValidator<UpdateCategoriesOrderRequest>`
- Rules:
  - `RuleFor(x => x.Items)` — `.NotEmpty()` with message "Categories list must not be empty."
  - `RuleForEach(x => x.Items)` — child validator for `CategoryOrderItemDto`:
    - `RuleFor(x => x.Id)` — `.NotEmpty()` with message "Category ID must not be empty."
    - `RuleFor(x => x.SortOrder)` — `.SetValidator(new CategorySortOrderValidator())` (reuse from Common.Contracts)
  - `RuleFor(x => x.Items)` — `.Must(HaveUniqueIds)` with message "Category IDs must be unique within the request."

---

## 7) Create unit tests

### File: `MyHomeRamen.UnitTests/MenuModule/Categories/CategoryValidationTests.cs`
Add tests for the new `UpdateSortOrder` domain method:

- `UpdateSortOrder_Should_UpdateSortOrder_When_SortOrderIsValid` — creates a category, calls `UpdateSortOrder(5)`, asserts `SortOrder == 5`
- `UpdateSortOrder_Should_ThrowDomainException_When_SortOrderIsBelowMinimum` — creates a category, calls `UpdateSortOrder(CategoryConstants.MinSortOrder - 1)`, asserts `DomainException` with `CategoryErrors.SortOrderTooSmall().Message`

---

## 8) Create integration tests

### File: `MyHomeRamen.IntegrationTests/MenuModule/Categories/UpdateCategoriesOrderTests.cs`

Test cases:
- `UpdateCategoriesOrder_ShouldReturnNoContent_ForValidRequest` — seed categories, build valid request with swapped sort orders, assert 204, then GET categories and verify new order
- `UpdateCategoriesOrder_ShouldReturnBadRequest_ForEmptyList` — send empty items list, assert 400
- `UpdateCategoriesOrder_ShouldReturnBadRequest_ForDuplicateIds` — send duplicate IDs, assert 400
- `UpdateCategoriesOrder_ShouldReturnBadRequest_ForInvalidSortOrder` — send negative sort order, assert 400
- `UpdateCategoriesOrder_ShouldReturnUnauthorized_ForAnonymousUser` — no auth header, assert 401
- `UpdateCategoriesOrder_ShouldReturnForbidden_ForNonAdminUser` — `[InlineData(UserRoles.Employee)]`, `[InlineData(UserRoles.Customer)]`, assert 403

---

## 9) Create architecture tests

No new architecture tests needed — existing module boundary tests already cover the Menu module.

---

## 10) Create system tests

Skip — no distributed workflow involved, single API call with DB persistence only.
