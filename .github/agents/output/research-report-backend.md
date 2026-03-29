# Research Report — Backend

- **Date**: 2025-07-17
- **Task**: GetCategoriesForDropdown
- **Module**: Menu
- **Reference feature**: CreateCategory

---

## 1) Reference Implementation Map

### CreateCategory — File Inventory

| Layer | File | Purpose |
|---|---|---|
| API Endpoint | `MyHomeRamen.Api/Menu/Features/Categories/CreateCategory/CreateCategoryEndpoint.cs` | Maps POST /categories, applies validation filter, requires RestaurantManagerPolicy |
| API Handler | `MyHomeRamen.Api/Menu/Features/Categories/CreateCategory/CreateCategoryHandler.cs` | Queries next sort order, creates domain entity, saves, returns ID |
| Request Model | `MyHomeRamen.Api/Menu/Features/Categories/CreateCategory/Models/CreateCategoryRequest.cs` | Record with `string Name, int CategoryType`, implements `IRequest<Guid>` |
| Response Model | `MyHomeRamen.Api/Menu/Features/Categories/CreateCategory/Models/CreateCategoryResponse.cs` | Record with `Guid Id` |
| Mappings | `MyHomeRamen.Api/Menu/Features/Categories/CreateCategory/Models/Mappings.cs` | `internal static` extension mapping request → domain entity |
| Validator | `MyHomeRamen.Api/Menu/Features/Categories/CreateCategory/Policies/CreateCategoryValidator.cs` | Extends `AbstractValidator<CreateCategoryRequest>`, validates name + enum guard |
| Domain Entity | `MyHomeRamen.Domain/Menu/Categories/Category.cs` | `Category.Create(id, name, sortOrder, categoryType)` factory, `Name`, `SortOrder`, `CategoryType` properties |
| EF Config | `MyHomeRamen.Persistance/Menu/Configurations/CategoryConfiguration.cs` | Entity configuration for Category |
| Group | `MyHomeRamen.Api/Menu/Features/Categories/CategoriesGroup.cs` | `WithTags("Categories")`, `RequireAuthorization()` |

### Key Code Patterns

#### Handler Pattern
```csharp
// Extracted from CreateCategoryHandler.cs
public sealed class CreateCategoryHandler(IMenuDbContext dbContext) : IRequestHandler<CreateCategoryRequest, Guid>
{
    public async Task<Guid> Handle(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        // For GetCategoriesForDropdown: query, filter, order, project to response list
        return category.Id.Value;
    }
}
```

#### Endpoint Pattern (POST — reference)
```csharp
// Extracted from CreateCategoryEndpoint.cs, lines 13-19
public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
{
    endpointBuilder.MapStandardValidatedPost<CreateCategoryRequest, CreateCategoryResponse>("categories", HandleAsync)
                   .WithName("CreateCategoryEndpoint")
                   .WithDescription("Handles Create Category operations.")
                   .RequireAuthorization(AuthorizationConfiguration.RestaurantManagerPolicy);
}
```

#### GET Endpoint Pattern (from EndpointBuilderExtensions)
```csharp
// MapStandardGet produces 200 OK + 404 + 500
public static RouteHandlerBuilder MapStandardGet<TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)

// MapStandardAuthenticatedGet adds 403 + authorization filter
public static RouteHandlerBuilder MapStandardAuthenticatedGet<TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
```

> ⚠️ **Gap identified**: There is NO `MapStandardValidatedGet` extension. The new endpoint needs validation of the `categoryType` query param.
> **Resolution**: Add `MapStandardValidatedGet<TRequest, TResponse>` to `EndpointBuilderExtensions.cs`, mirroring `MapStandardValidatedPost`, combining `MapStandardGet` + `WithValidationFilter<TRequest>`. The `ValidationFilter<TRequest>` extracts its argument from `context.Arguments` by type — the query-param-bound request record will be present as a handler argument and will be found correctly.

#### Validator Pattern (enum guard only — no async needed)
```csharp
// Extracted from CreateCategoryValidator.cs, lines 22-34
RuleFor(x => x.CategoryType)
    .Must(BeValidCategoryType).WithMessage("Please select a valid category type.");

private bool BeValidCategoryType(int categoryType)
{
    return Enum.IsDefined(typeof(CategoryType), (CategoryType)categoryType);
}
```

#### Domain Factory Pattern
```csharp
// Extracted from Category.cs, lines 24-36
public static Category Create(CategoryId id, string name, int sortOrder, CategoryType categoryType)
{
    Category category = new(id) { Name = name, SortOrder = sortOrder, CategoryType = categoryType };
    CategoryValidator.Validate(category);
    return category;
}
```

---

## 2) Conventions Discovered

| Convention | Example | Location |
|---|---|---|
| Endpoint class is `sealed`, implements `IEndpoint` | `public sealed class CreateCategoryEndpoint : IEndpoint` | `CreateCategoryEndpoint.cs` |
| Handler uses primary constructor DI | `CreateCategoryHandler(IMenuDbContext dbContext)` | `CreateCategoryHandler.cs` |
| Request is a `sealed record`, implements `IRequest<TResponse>` | `record CreateCategoryRequest(...) : IRequest<Guid>` | `CreateCategoryRequest.cs` |
| Response is a `sealed record` | `record CreateCategoryResponse(Guid Id)` | `CreateCategoryResponse.cs` |
| Mappings class is `internal static` | `internal static class Mappings` | `Mappings.cs` |
| Validator class is `sealed`, extends `AbstractValidator<TRequest>` | `sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryRequest>` | `CreateCategoryValidator.cs` |
| Group class is `sealed`, sets `GroupName = "Menu"` | `public sealed class CategoriesGroup : IGroupEndpoint` | `CategoriesGroup.cs` |
| `CategoryType` stored as `int` in requests, cast to enum in code | `(CategoryType)request.CategoryType` | `Mappings.cs` |
| DbExtensions are `static` extension methods on `IQueryable<TEntity>` | `GetNextSortOrderAsync(this IQueryable<Category> query, ...)` | `DbExtensions.cs` |
| Authorization policy constant from `AuthorizationConfiguration` | `AuthorizationConfiguration.RestaurantManagerPolicy` | `AuthorizationConfiguration.cs` |
| `IMenuDbContext` exposes `DbSet<Category> Categories` | `dbContext.Categories.Where(...)` | `IMenuDbContext.cs` |

---

## 3) Common Utilities Available

| Utility | Purpose | Namespace |
|---|---|---|
| `MapStandardGet<TResponse>` | Maps GET endpoint, produces 200/404/500 | `MyHomeRamen.Api.Common.Endpoint` |
| `MapStandardAuthenticatedGet<TResponse>` | MapStandardGet + 403 + auth filter | `MyHomeRamen.Api.Common.Endpoint` |
| `WithValidationFilter<TRequest>` | Adds `ValidationFilter<TRequest>` to route | `MyHomeRamen.Api.Common.Endpoint` |
| `ValidationFilter<TRequest>` | Extracts `TRequest` from handler args, runs FluentValidation | `MyHomeRamen.Api.Common.Filter` |
| `IMenuDbContext.Categories` | `DbSet<Category>` for querying categories | `MyHomeRamen.Domain.Menu.Database` |
| `DbExtensions.GetNextSortOrderAsync` | Calculates next sort order for a CategoryType | `MyHomeRamen.Persistance.Common` |
| `IRequestHandler<TRequest, TResponse>` | Handler interface with async `Handle` method | `MyHomeRamen.Api.Common.Endpoint.Models` |
| `AuthorizationConfiguration.RestaurantManagerPolicy` | Policy constant string `"RestaurantManager"` | `MyHomeRamen.Api.WebPresentation` |

---

## 4) Architecture Boundaries

### Existing tests for Menu
- Domain: enforces Menu → no dep on ShoppingCart, Orders, Payments, Reservations — `DomainBoundariesTests.cs`
- Persistence: `PersistanceBoundariesTests.cs` (exists, not read)
- API: enforces MenuApi → no dep on Orders, Payments, Reservations, ShoppingCart — `ApiBoundariesTests.cs`

### New boundaries needed
- None identified — `GetCategoriesForDropdown` stays within the Menu module and follows existing patterns.

---

## 5) Planned File Set for GetCategoriesForDropdown

| Layer | New File Path |
|---|---|
| Request Model | `MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/Models/GetCategoriesForDropdownRequest.cs` |
| Response Model | `MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/Models/GetCategoriesForDropdownResponse.cs` |
| Mappings | `MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/Models/Mappings.cs` |
| Validator | `MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/Policies/GetCategoriesForDropdownValidator.cs` |
| Handler | `MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/GetCategoriesForDropdownHandler.cs` |
| Endpoint | `MyHomeRamen.Api/Menu/Features/Categories/GetCategoriesForDropdown/GetCategoriesForDropdownEndpoint.cs` |
| Extension (new) | `MyHomeRamen.Api.Common/Endpoint/EndpointBuilderExtensions.cs` — add `MapStandardValidatedGet` overload |
| Integration Test | `MyHomeRamen.IntegrationTests/MenuModule/GetCategoriesForDropdownTests.cs` |

---

## 6) Potential Pitfalls

- **No `MapStandardValidatedGet` exists**: Must add `MapStandardValidatedGet<TRequest, TResponse>` to `EndpointBuilderExtensions.cs`. It should call `MapStandardGet<TResponse>` and chain `WithValidationFilter<TRequest>`. Also needs `ProducesProblem(StatusCodes.Status400BadRequest)` since validation can fail.
- **Query parameter binding**: The GET request record fields map from query string — ASP.NET Core Minimal API binds simple `record` params from query string automatically. Use `[AsParameters]` attribute on the handler argument: `([AsParameters] GetCategoriesForDropdownRequest request, ...)`.
- **`ValidationFilter` argument resolution**: The filter finds request by `context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(TRequest))`. With `[AsParameters]`, the record is NOT a single bound argument — its individual properties are bound separately. This means `ValidationFilter` will NOT find the request object. **Solution**: manually validate inside the handler, OR define the request as a standard query-string bound object using `[FromQuery]` attribute on the entire record. Test this carefully. The safest pattern for the new validator is to call `IValidator<GetCategoriesForDropdownRequest>` directly in the handler, or bind the entire request as `[AsParameters]` and verify filter behavior.
- **`IRequest<TResponse>` for GET**: The request record must still implement `IRequest<IEnumerable<GetCategoriesForDropdownResponse>>` (or a list type) for the handler interface.
- **Response type**: Should be `IEnumerable<GetCategoriesForDropdownResponse>` with fields `Guid Id`, `string Name` (matching `CategoryOption`).
- **SortOrder filtering**: Handler must filter by `CategoryType` and order by `SortOrder` ascending.
- **Integration test data**: `DataSeeder` seeds 5 random categories with random `CategoryType`. Tests that assert on ordered list or specific type must account for the seeded data distribution — prefer generating dedicated test data within the test itself rather than relying on `DataGenerator.GeneratedCategories`.
