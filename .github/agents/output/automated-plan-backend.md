# Feature Implementation Plan — Backend

- **Date**: 2025-07-17
- **Feature**: GetCategoriesForManage
- **Module**: Menu
- **Reference**: GetCategoriesForDropdown, GetIngredientsForDropdown

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/Menu/Features/Categories/
??? GetCategoriesForManage/
?   ??? Models/
?   ?   ??? GetCategoriesForManageRequest.cs
?   ?   ??? GetCategoriesForManageResponse.cs
?   ?   ??? Mappings.cs
?   ??? GetCategoriesForManageEndpoint.cs
?   ??? GetCategoriesForManageHandler.cs
```

**Notes:**
- No `Policies/` folder needed — no query parameters to validate, no custom authorization policy, no cache policy.
- Marker request record (no properties) is still required for the `IRequestHandler<TRequest, TResponse>` generic contract — follows `GetIngredientsForDropdownRequest` pattern.

---

## 2) Create primitive rules and contracts

**No new contracts needed.** This is a read-only query endpoint returning existing categories. No input requires validation.

---

## 3) Create models, DTOs and mappings

### `Models/GetCategoriesForManageRequest.cs`

Marker request record (no properties) following `GetIngredientsForDropdownRequest` pattern:

```csharp
namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForManage.Models;

public sealed record GetCategoriesForManageRequest : IRequest<GetCategoriesForManageResponse>;
```

### `Models/GetCategoriesForManageResponse.cs`

Response containing two `IEnumerable` lists — one for product categories, one for ingredient categories:

```csharp
namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForManage.Models;

public sealed record GetCategoriesForManageResponse(
    IEnumerable<CategoryForManageDto> ProductCategories,
    IEnumerable<CategoryForManageDto> IngredientCategories);

public sealed record CategoryForManageDto(Guid Id, string Name, int SortOrder);
```

**Design decisions:**
- `SortOrder` is included because the frontend needs it for reordering functionality.
- A shared `CategoryForManageDto` is used for both lists since product and ingredient categories have the same shape.
- Response wraps both lists in a single object rather than requiring two API calls.

### `Models/Mappings.cs`

```csharp
internal static class Mappings
{
    public static CategoryForManageDto ToManageDto(this Category category)
    {
        return new CategoryForManageDto(category.Id.Value, category.Name, category.SortOrder);
    }
}
```

---

## 4) Create Persistence DB Extension

### `DbExtensions.cs` — add `ForManage` extension on `DbSet<Category>`

Add to `MyHomeRamen.Persistance/Common/DbExtensions.cs`:

```csharp
internal static IQueryable<Category> ForManage(
    this DbSet<Category> categories,
    CategoryType categoryType)
{
    return categories
        .AsNoTracking()
        .Where(c => c.CategoryType == categoryType)
        .OrderBy(c => c.SortOrder);
}
```

**Notes:**
- Follows the existing `ForDropdown` pattern exactly.
- Returns `IQueryable<Category>` — handler owns the final projection to DTO (per persistence layer boundary rules).
- Ordered by `SortOrder` to support frontend reordering display.
- Could potentially reuse `ForDropdown` since the query shape is identical, but a separate extension provides clearer intent and allows the manage query to diverge in the future (e.g., including soft-deleted items, additional filters).

---

## 5) Create `GetCategoriesForManageHandler` (IRequestHandler)

### `GetCategoriesForManageHandler.cs`

```csharp
public sealed class GetCategoriesForManageHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetCategoriesForManageRequest, GetCategoriesForManageResponse>
{
    public async Task<GetCategoriesForManageResponse> Handle(
        GetCategoriesForManageRequest request,
        CancellationToken cancellationToken)
    {
        IEnumerable<CategoryForManageDto> productCategories = await dbContext.Categories
            .ForManage(CategoryType.Product)
            .Select(c => c.ToManageDto())
            .ToListAsync(cancellationToken);

        IEnumerable<CategoryForManageDto> ingredientCategories = await dbContext.Categories
            .ForManage(CategoryType.Ingredient)
            .Select(c => c.ToManageDto())
            .ToListAsync(cancellationToken);

        return new GetCategoriesForManageResponse(productCategories, ingredientCategories);
    }
}
```

**Design decisions:**
- Two separate queries (one per `CategoryType`) to populate both lists — keeps queries simple and translatable.
- Uses `AsNoTracking()` via the `ForManage` extension (read-only query, CQRS query rules).
- No `SaveChangesAsync()` — pure query with no side effects.
- Handler owns the projection via `Select(c => c.ToManageDto())`.

---

## 6) Create `GetCategoriesForManageEndpoint` (IEndpoint)

### `GetCategoriesForManageEndpoint.cs`

```csharp
public sealed class GetCategoriesForManageEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetCategoriesForManageResponse>("categories/manage", HandleAsync)
            .WithName("GetCategoriesForManageEndpoint")
            .WithDescription("Returns all categories grouped by type for admin management.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IRequestHandler<GetCategoriesForManageRequest, GetCategoriesForManageResponse> handler,
        CancellationToken cancellationToken)
    {
        GetCategoriesForManageResponse response = await handler.Handle(
            new GetCategoriesForManageRequest(), cancellationToken);
        return Results.Ok(response);
    }
}
```

**Design decisions:**
- Route: `GET /api/menu/categories/manage` (GroupName "Menu" + segment "categories/manage").
- Uses `MapStandardGet<TResponse>` (no validation filter needed) — same pattern as `GetIngredientsForDropdownEndpoint`.
- Authorization: `RestaurantManagerPolicy` (Admin role only) — per user requirement: "available for admin".
- No `[AsParameters]` needed since there are no request parameters.
- Instantiates marker `GetCategoriesForManageRequest` inline — same pattern as `GetIngredientsForDropdownEndpoint`.

---

## 7) Unit tests

**No unit tests required.** This is a pure read-only query with no domain logic, no validation, and no contracts to test.

---

## 8) Integration tests

### File: `MyHomeRamen.IntegrationTests/MenuModule/GetCategoriesForManageTests.cs`

Reference: `GetCategoriesForDropdownTests.cs`

#### Test cases

| # | Test method | Type | Expected | Description |
|---|---|---|---|---|
| 1 | `GetCategoriesForManage_ShouldReturnOk_ForAuthenticatedAdmin` | Fact | 200 OK | Admin user calls endpoint, asserts status code. |
| 2 | `GetCategoriesForManage_ShouldReturnBothNonEmptyLists_ForSeededData` | Fact | 200 OK | Deserialize response, verify `ProductCategories` and `IngredientCategories` are both non-null and non-empty (seeded data exists for both types). |
| 3 | `GetCategoriesForManage_ShouldReturnProductCategoriesOrderedBySortOrder` | Fact | 200 OK | Compare returned product categories order against DB query ordered by `SortOrder`. |
| 4 | `GetCategoriesForManage_ShouldReturnIngredientCategoriesOrderedBySortOrder` | Fact | 200 OK | Compare returned ingredient categories order against DB query ordered by `SortOrder`. |
| 5 | `GetCategoriesForManage_ShouldReturnUnauthorized_ForNotAuthenticatedUser` | Fact | 401 | No auth header. |
| 6 | `GetCategoriesForManage_ShouldReturnForbidden_ForNonManagerRole` | Theory | 403 | `[InlineData(UserRoles.Employee)]`, `[InlineData(UserRoles.Customer)]`. |

All tests use:
- Route: `GET /api/menu/categories/manage`
- `HttpClientExtensions.CreateGetMessage(...)` pattern
- `AddAuthorizationHeader(UserRoles.Admin)` for valid auth
- `WebApiFactory` injected via primary constructor

#### Response deserialization

Tests should deserialize using the API response types directly:
- `GetCategoriesForManageResponse` (contains `ProductCategories` and `IngredientCategories`)
- `CategoryForManageDto` (contains `Id`, `Name`, `SortOrder`)

---

## 9) Architecture tests

**No architecture tests required.**

---

## 10) System tests

**No system tests required.**
