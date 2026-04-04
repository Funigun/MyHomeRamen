# Feature Refactor Plan - Backend

- **Date**: 2025-07-21
- **Feature**: GetCategoriesByType (replaces GetCategoriesForDropdown + GetCategoriesForManage)
- **Module**: Menu
- **Type**: Refactor
- **Reference features**: GetCategoriesForDropdown, GetCategoriesForManage

---

## Summary

Replace two separate endpoints (`GetCategoriesForDropdown` and `GetCategoriesForManage`) with a single `GetCategoriesByType` endpoint that accepts a `CategoryType` parameter. Consolidate the two `DbExtensions` methods (`ForDropdown` and `ForManage`) into a single `ForCategoryType` method.

### Breaking changes
- `GET /api/menu/categories/dropdown?categoryType={int}` - REMOVED
- `GET /api/menu/categories/manage` - REMOVED
- `GET /api/menu/categories/by-type?categoryType={int}` - NEW (replaces both)
- Response shape changes from two different response types to a single `GetCategoriesByTypeResponse` list

---

## 1) Create feature folder and structure

### New folder:
```
MyHomeRamen.Api/Menu/Features/Categories/
    GetCategoriesByType/
        Models/
            GetCategoriesByTypeRequest.cs
            GetCategoriesByTypeResponse.cs
            Mappings.cs
        Policies/
            GetCategoriesByTypeValidator.cs
        GetCategoriesByTypeEndpoint.cs
        GetCategoriesByTypeHandler.cs
```

### Folders to remove:
```
MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/   (entire folder)
MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForManage/     (entire folder)
```

---

## 2) Create primitive rules and contracts

**No new contracts needed.** The `CategoryType` validation is inline (enum check) and does not warrant a shared contract validator.

---

## 3) Create models, DTOs and mappings

### `Models/GetCategoriesByTypeRequest.cs`

Request record with `CategoryType` as an `int` query parameter (same pattern as existing `GetCategoriesForDropdownRequest`):

```csharp
namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Models;

public sealed record GetCategoriesByTypeRequest(int CategoryType) 
    : IRequest<IEnumerable<GetCategoriesByTypeResponse>>;
```

### `Models/GetCategoriesByTypeResponse.cs`

Unified response containing `Id`, `Name`, and `SortOrder` (superset of both old responses):

```csharp
namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Models;

public sealed record GetCategoriesByTypeResponse(Guid Id, string Name, int SortOrder);
```

**Design decision:** The new response includes `SortOrder` (from `GetCategoriesForManage`) and keeps `Id` + `Name` (from `GetCategoriesForDropdown`). This unified shape serves both dropdown and management use cases.

### `Models/Mappings.cs`

```csharp
internal static class Mappings
{
    public static GetCategoriesByTypeResponse ToResponse(this Category category)
    {
        return new GetCategoriesByTypeResponse(category.Id.Value, category.Name, category.SortOrder);
    }
}
```

---

## 4) Create validation policy

### `Policies/GetCategoriesByTypeValidator.cs`

Reuse the same validation logic from `GetCategoriesForDropdownValidator`:

```csharp
public sealed class GetCategoriesByTypeValidator : AbstractValidator<GetCategoriesByTypeRequest>
{
    public GetCategoriesByTypeValidator()
    {
        RuleFor(x => x.CategoryType)
            .Must(BeValidCategoryType)
            .WithMessage("Please select a valid category type.");
    }

    private static bool BeValidCategoryType(int categoryType)
    {
        return Enum.IsDefined(typeof(CategoryType), (CategoryType)categoryType);
    }
}
```

---

## 5) Consolidate Persistence DB Extensions

### `MyHomeRamen.Persistance/Common/DbExtensions.cs`

**Remove** both existing methods:
- `ForDropdown(this DbSet<Category> categories, CategoryType categoryType)`
- `ForManage(this DbSet<Category> categories, CategoryType categoryType)`

**Add** single consolidated method:

```csharp
internal static IQueryable<Category> ForCategoryType(
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
- Both `ForDropdown` and `ForManage` had identical query logic (same filtering and ordering).
- The new `ForCategoryType` method replaces both with clearer intent.
- Returns `IQueryable<Category>` - handler owns the final projection.

---

## 6) Create `GetCategoriesByTypeHandler` (IRequestHandler)

### `GetCategoriesByTypeHandler.cs`

```csharp
public sealed class GetCategoriesByTypeHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetCategoriesByTypeRequest, IEnumerable<GetCategoriesByTypeResponse>>
{
    public async Task<IEnumerable<GetCategoriesByTypeResponse>> Handle(
        GetCategoriesByTypeRequest request,
        CancellationToken cancellationToken)
    {
        CategoryType categoryType = (CategoryType)request.CategoryType;

        return await dbContext.Categories
            .ForCategoryType(categoryType)
            .Select(c => c.ToResponse())
            .ToListAsync(cancellationToken);
    }
}
```

**Design decisions:**
- Same pattern as `GetCategoriesForDropdownHandler` but using the consolidated `ForCategoryType` extension.
- Single query per call - the frontend will call this endpoint once per category type it needs.
- Cast `int` -> `CategoryType` is safe because the validator ensures the value is valid.

---

## 7) Create `GetCategoriesByTypeEndpoint` (IEndpoint)

### `GetCategoriesByTypeEndpoint.cs`

```csharp
public sealed class GetCategoriesByTypeEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedGet<GetCategoriesByTypeRequest, IEnumerable<GetCategoriesByTypeResponse>>(
                "categories/by-type", HandleAsync)
            .WithName("GetCategoriesByTypeEndpoint")
            .WithDescription("Returns a filtered and ordered list of categories for the specified category type.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetCategoriesByTypeRequest request,
        [FromServices] IRequestHandler<GetCategoriesByTypeRequest, IEnumerable<GetCategoriesByTypeResponse>> handler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetCategoriesByTypeResponse> response = await handler.Handle(request, cancellationToken);
        return Results.Ok(response);
    }
}
```

**Design decisions:**
- Route: `GET /api/menu/categories/by-type?categoryType={int}`
- Uses `MapStandardValidatedGet` (has request parameters that need validation) - same pattern as old `GetCategoriesForDropdown`.
- Authorization: `RestaurantManagerPolicy` (Admin role only) - carried over from both old endpoints.
- `[AsParameters]` to bind query string parameter.

---

## 8) Cleanup - Remove old features

### Files to delete:
```
MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/GetCategoriesForDropdownEndpoint.cs
MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/GetCategoriesForDropdownHandler.cs
MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/Models/GetCategoriesForDropdownRequest.cs
MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/Models/GetCategoriesForDropdownResponse.cs
MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/Models/Mappings.cs
MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/Policies/GetCategoriesForDropdownValidator.cs
MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForManage/GetCategoriesForManageEndpoint.cs
MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForManage/GetCategoriesForManageHandler.cs
MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForManage/Models/GetCategoriesForManageRequest.cs
MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForManage/Models/GetCategoriesForManageResponse.cs
MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForManage/Models/Mappings.cs
```

---

## 9) Unit tests

**No unit tests required.** Per feature brief: unit tests = no.

---

## 10) Integration tests

### File: `MyHomeRamen.IntegrationTests/MenuModule/GetCategoriesByTypeTests.cs` (NEW)

Reference: `GetCategoriesForDropdownTests.cs`, `GetCategoriesForManageTests.cs`

#### Test cases (from feature brief)

| # | Test method | Type | Expected | Description |
|---|---|---|---|---|
| 1 | `GetCategoriesByType_ShouldReturnOkWithList_ForIngredientType` | Fact | 200 OK | Authenticated manager calls with `CategoryType.Ingredient`, asserts non-empty list returned. |
| 2 | `GetCategoriesByType_ShouldReturnOk_ForAuthenticatedManager` | Fact | 200 OK | Authenticated manager calls with `CategoryType.Product`, asserts 200 status. |
| 3 | `GetCategoriesByType_ShouldReturnUnauthorized_ForUnauthenticatedUser` | Fact | 401 | No auth header, asserts 401. |
| 4 | `GetCategoriesByType_ShouldReturnForbidden_ForNonManagerRoles` | Theory | 403 | `[InlineData(UserRoles.Employee)]`, `[InlineData(UserRoles.Customer)]` |

#### Endpoint URL
```
/api/menu/categories/by-type?categoryType={int}
```

#### Pattern
Follow `GetCategoriesForDropdownTests.cs` pattern:
- Use `HttpClientExtensions.CreateGetMessage()` with `AddAuthorizationHeader()`
- Use `apiFactory.HttpClient.SendAsync()` 
- Deserialize with `ReadFromJsonAsync<IEnumerable<GetCategoriesByTypeResponse>>()`

### Files to delete:
```
MyHomeRamen.IntegrationTests/MenuModule/GetCategoriesForDropdownTests.cs
MyHomeRamen.IntegrationTests/MenuModule/GetCategoriesForManageTests.cs
```

---

## 11) Architecture tests

**No architecture tests required.** Per feature brief: architecture tests = no.

---

## 12) System tests

**No system tests required.** Per feature brief: system tests = no.
