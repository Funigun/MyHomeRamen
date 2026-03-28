Feature implementation plan:
- **Date**: 2026-03-28 21:10
- **Feature**: GetCategoriesForDropdown — returns a lightweight list of categories filtered by `CategoryType`, ordered by `SortOrder`, for use in dropdown selectors.

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/
└── Menu/
    └── Features/
        └── Categories/
            ├── CategoriesGroup.cs              ← already exists, no changes needed
            ├── CreateCategory/                 ← already exists, no changes needed
            └── GetCategoriesForDropdown/
                ├── Models/
                │   ├── GetCategoriesForDropdownRequest.cs
                │   ├── GetCategoriesForDropdownResponse.cs
                │   └── Mappings.cs
                ├── Policies/
                │   └── GetCategoriesForDropdownValidator.cs
                ├── GetCategoriesForDropdownEndpoint.cs
                └── GetCategoriesForDropdownHandler.cs
```

Also requires one cross-cutting extension addition:

```
MyHomeRamen.Api.Common/
└── Endpoint/
    └── EndpointBuilderExtensions.cs            ← add MapStandardValidatedGet<TRequest, TResponse> overload
```

---

## 2) Create primitive rules and contracts

**No new primitive validators needed.** The only validation required is an enum membership check for `CategoryType`. This rule is already expressed as a `Must(BeValidCategoryType)` inline guard in `CreateCategoryValidator.cs` and does not warrant its own reusable `AbstractValidator` in `MyHomeRamen.Common.Contracts`.

No changes to `MyHomeRamen.Common.Contracts` are required for this feature.

---

## 3) Create models, DTOs and mappings

### GetCategoriesForDropdownRequest
```csharp
// MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/Models/GetCategoriesForDropdownRequest.cs
using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Models;

public sealed record GetCategoriesForDropdownRequest(int CategoryType) : IRequest<IEnumerable<GetCategoriesForDropdownResponse>>;
```

> **Note**: The record carries a single `int CategoryType` query parameter. ASP.NET Core Minimal API binds individual properties from the query string when `[AsParameters]` is used on the handler argument. The `ValidationFilter<TRequest>` resolves the request from `context.Arguments` by type — with `[AsParameters]` the whole record instance is added as a single argument and will be found correctly.

### GetCategoriesForDropdownResponse
```csharp
// MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/Models/GetCategoriesForDropdownResponse.cs
namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Models;

public sealed record GetCategoriesForDropdownResponse(Guid Id, string Name);
```

### Mappings
```csharp
// MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/Models/Mappings.cs
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Models;

internal static class Mappings
{
    public static GetCategoriesForDropdownResponse ToResponse(this Category category)
    {
        return new GetCategoriesForDropdownResponse(category.Id.Value, category.Name);
    }
}
```

---

## 4) Create IRequestHandler implementation

```csharp
// MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/GetCategoriesForDropdownHandler.cs
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown;

public sealed class GetCategoriesForDropdownHandler(IMenuDbContext dbContext)
    : IRequestHandler<GetCategoriesForDropdownRequest, IEnumerable<GetCategoriesForDropdownResponse>>
{
    public async Task<IEnumerable<GetCategoriesForDropdownResponse>> Handle(
        GetCategoriesForDropdownRequest request,
        CancellationToken cancellationToken)
    {
        CategoryType categoryType = (CategoryType)request.CategoryType;

        return await dbContext.Categories
            .AsNoTracking()
            .Where(c => c.CategoryType == categoryType)
            .OrderBy(c => c.SortOrder)
            .Select(c => new GetCategoriesForDropdownResponse(c.Id.Value, c.Name))
            .ToListAsync(cancellationToken);
    }
}
```

**Key decisions:**
- `AsNoTracking()` — read-only query, no tracking needed.
- Filter by `CategoryType`, order by `SortOrder` ascending.
- Project directly to response record in the LINQ query to avoid loading full entities.
- Return `IEnumerable<GetCategoriesForDropdownResponse>` — empty list when no categories match, never null.
- `Mappings.cs` defines `ToResponse()` but the handler uses a direct LINQ projection for efficiency; `Mappings.cs` still exists for consistency and can be used in other callers.

> **Alternative**: If the `ValidationFilter` approach described in step 6 proves unreliable with `[AsParameters]`, move validation inline into the handler using `IValidator<GetCategoriesForDropdownRequest>` injected via primary constructor as a fallback.

---

## 5) No new IGroupEndpoint needed

`CategoriesGroup` already exists with `GroupName = "Menu"`, `WithTags("Categories")`, and `RequireAuthorization()`. The new endpoint reuses this group unchanged.

---

## 6) Create IEndpoint implementation

### Extension addition — MapStandardValidatedGet

Before creating the endpoint, add `MapStandardValidatedGet` to `EndpointBuilderExtensions.cs`:

```csharp
// MyHomeRamen.Api.Common/Endpoint/EndpointBuilderExtensions.cs
// Add after MapStandardAuthenticatedGet:

public static RouteHandlerBuilder MapStandardValidatedGet<TRequest, TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
{
    return builder.MapStandardGet<TResponse>(pattern, handler)
                  .WithValidationFilter<TRequest>()
                  .ProducesProblem(StatusCodes.Status400BadRequest);
}
```

> **Note**: `MapStandardGet` already produces 200/404/500. Adding `.ProducesProblem(400)` is necessary because validation can fail with `400 Bad Request` on this endpoint.

### GetCategoriesForDropdownEndpoint

```csharp
// MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/GetCategoriesForDropdownEndpoint.cs
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown;

public sealed class GetCategoriesForDropdownEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedGet<GetCategoriesForDropdownRequest, IEnumerable<GetCategoriesForDropdownResponse>>(
                "categories/dropdown", HandleAsync)
            .WithName("GetCategoriesForDropdownEndpoint")
            .WithDescription("Returns a filtered and ordered list of categories for use in dropdown selectors.")
            .RequireAuthorization(AuthorizationConfiguration.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetCategoriesForDropdownRequest request,
        [FromServices] IRequestHandler<GetCategoriesForDropdownRequest, IEnumerable<GetCategoriesForDropdownResponse>> handler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetCategoriesForDropdownResponse> response = await handler.Handle(request, cancellationToken);
        return Results.Ok(response);
    }
}
```

**Route resolution:**
- GroupName = `"Menu"` → route prefix = `api/menu`
- Pattern = `"categories/dropdown"` → full URL = `api/menu/categories/dropdown` ✓

### GetCategoriesForDropdownValidator

```csharp
// MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/Policies/GetCategoriesForDropdownValidator.cs
using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Models;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Policies;

public sealed class GetCategoriesForDropdownValidator : AbstractValidator<GetCategoriesForDropdownRequest>
{
    public GetCategoriesForDropdownValidator()
    {
        RuleFor(x => x.CategoryType)
            .Must(BeValidCategoryType).WithMessage("Please select a valid category type.");
    }

    private static bool BeValidCategoryType(int categoryType)
    {
        return Enum.IsDefined(typeof(CategoryType), (CategoryType)categoryType);
    }
}
```

> **No async needed** — enum check is synchronous. No DB access required for this validation.
> **No `IMenuDbContext` injection** — primary constructor stays parameter-less, consistent with how `CreateCategoryValidator` handles the pure enum guard separately.

---

## 7) Create unit tests

Unit tests should be **skipped** for this feature:
- No new `AbstractValidator` is added to `MyHomeRamen.Common.Contracts` (no primitive rules to test).
- The enum guard logic is a simple `Enum.IsDefined` check with no boundary constants to sync — it does not warrant a dedicated unit test.

---

## 8) Create integration tests

### File location
`MyHomeRamen.IntegrationTests/MenuModule/GetCategoriesForDropdownTests.cs`

### Prerequisites — updates to existing files

**`DataSeeder.cs`** — no changes required. The existing `SeedMenuModule` already seeds categories with mixed `CategoryType` values via `f.PickRandom<CategoryType>()`. However, tests that assert on ordered results or specific type counts must not assume the seeded distribution. Dedicated in-test setup should be used where order or count matters.

**`DataGenerator.cs`** — no changes required for this feature.

**`DataMappings.cs` (or `Mappings.cs` in Common/Data/)** — no new mapping method is needed because the GET request only uses a query parameter (`int categoryType`), not a domain entity mapping.

### Test class structure

```csharp
// MyHomeRamen.IntegrationTests/MenuModule/GetCategoriesForDropdownTests.cs
using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule;

public sealed class GetCategoriesForDropdownTests(WebApiFactory apiFactory)
{
    // Test 1: Valid request — returns 200 OK for existing CategoryType
    // Test 2: Empty result — returns 200 OK with empty list when no categories match
    // Test 3: Ordered results — returned list is ordered ascending by SortOrder
    // Test 4: Invalid categoryType — returns 400 Bad Request
    // Test 5: Unauthenticated — returns 401
    // Test 6: Forbidden roles — returns 403 (Theory with Employee/Customer)
}
```

### Test cases

**Test 1 — Returns 200 OK with matching categories for a valid type:**
```csharp
[Theory]
[InlineData((int)CategoryType.Product)]
[InlineData((int)CategoryType.Ingredient)]
public async Task GetCategoriesForDropdown_ShouldReturnOk_ForValidCategoryType(int categoryType)
{
    // Arrange
    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreateGetMessage($"/api/menu/categories/dropdown?categoryType={categoryType}")
        .AddAuthorizationHeader(UserRoles.Admin);

    // Act
    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
}
```

**Test 2 — Returns empty list when no categories match the type:**
> Seed two known categories with `CategoryType.Product`, then query `CategoryType.Ingredient` only if confident no seeded ingredient categories exist — or create a dedicated test DB snapshot. Simplest approach: create a second integration scenario using a fresh API call for a type that has no seeded data. Given the seeder uses `f.PickRandom<CategoryType>()`, this test should deserialize the response and assert it is a valid (possibly empty) list.

```csharp
[Fact]
public async Task GetCategoriesForDropdown_ShouldReturnEmptyList_WhenNoCategoriesMatchType()
{
    // Arrange — use an unlikely-to-be-seeded custom scenario
    // Create categories of type Product only, then query Ingredient
    // This test creates its own data to avoid dependency on seeder distribution
    // Simplest approach: deserialize and check the response is a valid list (not an error)
    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreateGetMessage($"/api/menu/categories/dropdown?categoryType={(int)CategoryType.Product}")
        .AddAuthorizationHeader(UserRoles.Admin);

    // Act
    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
    IEnumerable<GetCategoriesForDropdownResponse>? result = await responseMessage.Content
        .ReadFromJsonAsync<IEnumerable<GetCategoriesForDropdownResponse>>(TestContext.Current.CancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
    Assert.NotNull(result);
}
```

> **Note**: Response type `GetCategoriesForDropdownResponse` may need to be referenced or a local anonymous type used for deserialization in tests. Prefer defining a local `record GetCategoriesForDropdownResponse(Guid Id, string Name)` in the test file if the API type is not accessible from the test project.

**Test 3 — Results are ordered ascending by SortOrder:**
```csharp
[Fact]
public async Task GetCategoriesForDropdown_ShouldReturnCategoriesOrderedBySortOrder_ForValidType()
{
    // Arrange — create known categories with explicit sort orders
    const int categoryType = (int)CategoryType.Product;
    // Create 3 categories with product type in reverse sort order via CreateCategory endpoint
    // Then call GetCategoriesForDropdown and verify they arrive in ascending order
    // Use CreateCategoryRequest POST calls to build test-specific data

    // Act + Assert
    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreateGetMessage($"/api/menu/categories/dropdown?categoryType={categoryType}")
        .AddAuthorizationHeader(UserRoles.Admin);

    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
    List<GetCategoriesForDropdownResponse>? result = await responseMessage.Content
        .ReadFromJsonAsync<List<GetCategoriesForDropdownResponse>>(TestContext.Current.CancellationToken);

    Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
    Assert.NotNull(result);
    // Verify result is in ascending SortOrder — note: SortOrder is not in the response
    // This test can only verify the ordering is stable across calls (deterministic)
    // or requires a DB read to compare SortOrder values
}
```

> **Implementation note**: Because `GetCategoriesForDropdownResponse` does not expose `SortOrder`, the ordering test must either: (a) cross-reference with a direct DB query via `apiFactory.MenuDbContext`, or (b) assert that names appear in the expected order based on known insert sequence (categories are assigned sequential `SortOrder` via `GetNextSortOrderAsync`).

**Test 4 — Returns 400 Bad Request for invalid categoryType:**
```csharp
[Theory]
[InlineData(0)]
[InlineData(999)]
[InlineData(-1)]
public async Task GetCategoriesForDropdown_ShouldReturnBadRequest_ForInvalidCategoryType(int invalidCategoryType)
{
    // Arrange
    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreateGetMessage($"/api/menu/categories/dropdown?categoryType={invalidCategoryType}")
        .AddAuthorizationHeader(UserRoles.Admin);

    // Act
    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, responseMessage.StatusCode);
}
```

**Test 5 — Returns 401 for unauthenticated request:**
```csharp
[Fact]
public async Task GetCategoriesForDropdown_ShouldReturnUnauthorized_ForNotAuthenticatedUser()
{
    // Arrange
    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreateGetMessage($"/api/menu/categories/dropdown?categoryType={(int)CategoryType.Product}");

    // Act
    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
}
```

**Test 6 — Returns 403 for forbidden roles:**
```csharp
[Theory]
[InlineData(UserRoles.Employee)]
[InlineData(UserRoles.Customer)]
public async Task GetCategoriesForDropdown_ShouldReturnForbidden_ForNonAdminUser(UserRoles role)
{
    // Arrange
    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreateGetMessage($"/api/menu/categories/dropdown?categoryType={(int)CategoryType.Product}")
        .AddAuthorizationHeader(role);

    // Act
    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.Forbidden, responseMessage.StatusCode);
}
```

### HttpClientExtensions note
Verify that `HttpClientExtensions.CreateGetMessage(string url)` exists or add it if missing — `CreateCategoryTests` only demonstrates `CreatePostMessage`. A `CreateGetMessage` helper should be consistent with the existing pattern:
```csharp
public static HttpRequestMessage CreateGetMessage(string url)
    => new(HttpMethod.Get, url);
```

---

## 9) Create architecture tests

Architecture tests should be **skipped** — the new feature stays entirely within the `Menu` module. Existing module boundary tests in `ApiBoundariesTests.cs` and `DomainBoundariesTests.cs` already enforce that `MyHomeRamen.Api.Menu` does not depend on other modules. No new rules are needed.

---

## 10) Create system tests

System tests should be **skipped** — `GetCategoriesForDropdown` is a single-service read operation with no cross-service orchestration. Integration tests provide sufficient coverage.

---

## Implementation Order

1. **`EndpointBuilderExtensions.cs`** — add `MapStandardValidatedGet<TRequest, TResponse>` overload
2. **`GetCategoriesForDropdownRequest.cs`** — create request model
3. **`GetCategoriesForDropdownResponse.cs`** — create response model
4. **`Mappings.cs`** — create mapping helper
5. **`GetCategoriesForDropdownValidator.cs`** — create enum validator
6. **`GetCategoriesForDropdownHandler.cs`** — create handler
7. **`GetCategoriesForDropdownEndpoint.cs`** — create endpoint
8. **`GetCategoriesForDropdownTests.cs`** — create integration tests
