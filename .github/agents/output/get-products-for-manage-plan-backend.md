# Feature plan — GetProductsForManage (Backend)

- **Date**: 2025-07-15
- **Feature**: GetProductsForManage — `GET /api/menu/products/manage` returning a paged, filtered, and sorted list of products for the admin management view
- **Reference**: `GetIngredientsForManage` (Menu module)

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/Menu/Features/Products/GetProductsForManage/
├── Models/
│   ├── GetProductsForManageRequest.cs
│   ├── GetProductsForManageResponse.cs
│   ├── ProductDto.cs
│   └── Mappings.cs
├── Policies/
│   └── GetProductsForManageValidator.cs
├── GetProductsForManageEndpoint.cs
└── GetProductsForManageHandler.cs
```

---

## 2) Create primitive rules and contracts

No new primitive validators needed — reuses existing `ProductNameValidator` constants for max name length validation.

---

## 3) Create models, DTOs and mappings

### `Models/GetProductsForManageRequest.cs`
- `public sealed record GetProductsForManageRequest(string? Name, IEnumerable<Guid>? CategoryIds, IEnumerable<Guid>? IngredientIds, decimal? PriceFrom, decimal? PriceTo, string? OrderBy) : IRequest<GetProductsForManageResponse>`
- Mutable property: `public PageParameters PageParameters { get; set; }` (set from endpoint)
- Reference: `GetIngredientsForManageRequest`

### `Models/GetProductsForManageResponse.cs`
- `public sealed record GetProductsForManageResponse(int Page, int PageSize, int TotalCount, IEnumerable<ProductDto> Products)`
- Reference: `GetIngredientsForManageResponse`

### `Models/ProductDto.cs`
- `public sealed record ProductDto(Guid Id, string Name, string Description, decimal Price)`
- Reference: `IngredientDto`

### `Models/Mappings.cs`
- `internal static class Mappings`
- Extension method: `public static ProductDto ToResponse(this Product product)` → `new(product.Id.Value, product.Name, product.Description, product.Price)`
- Reference: `GetIngredientsForManage/Models/Mappings.cs`

---

## 4) Create IRequestHandler implementation

### `GetProductsForManageHandler.cs`
- `public sealed class GetProductsForManageHandler(IMenuDbContext dbContext) : IRequestHandler<GetProductsForManageRequest, GetProductsForManageResponse>`
- Steps:
  1. Build query: `IQueryable<Product> query = dbContext.Products.ForManage(request.Name, request.CategoryIds, request.IngredientIds, request.PriceFrom, request.PriceTo)`
  2. Count: `int totalCount = await query.CountAsync(ct)`
  3. Apply ordering:
     - If `request.OrderBy` equals `"Price"` (case-insensitive) → `query.OrderBy(p => p.Price)`
     - Else (default) → `query.OrderBy(p => p.Name)`
  4. Apply paging: `.Paged(request.PageParameters.PageNumber, request.PageParameters.PageSize)`
  5. Project: `List<ProductDto> products = await query.Select(p => p.ToResponse()).ToListAsync(ct)`
  6. Return `new GetProductsForManageResponse(Page, PageSize, TotalCount, Products)`
- Reference: `GetIngredientsForManageHandler`

---

## 5) Create IGroupedEndpoint implementation

Not needed — `ProductsGroup` already exists.

---

## 6) Create IEndpoint implementation

### `GetProductsForManageEndpoint.cs`
- `public sealed class GetProductsForManageEndpoint : IEndpoint`
- `GroupName = "Menu"`
- Maps `MapStandardValidatedGet<GetProductsForManageRequest, GetProductsForManageResponse>("products/manage", HandleAsync)`
- `.WithName("GetProductsForManageEndpoint")`
- `.WithDescription("Returns a filtered, sorted, and paged list of products for the admin management view.")`
- `.RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy)`
- Handler signature: `HandleAsync([AsParameters] GetProductsForManageRequest request, [AsParameters] PageParameters pageParameters, [FromServices] IRequestHandler<...> handler, CancellationToken cancellationToken)`
- Sets `request.PageParameters = pageParameters`
- Returns `Results.Ok(response)`
- Reference: `GetIngredientsForManageEndpoint`

---

## Persistence changes — `MyHomeRamen.Persistance/Common/DbExtensions.cs`

### New extension: `ForManage` on `DbSet<Product>`
```csharp
public static IQueryable<Product> ForManage(
    this DbSet<Product> products,
    string? name,
    IEnumerable<Guid>? categoryIds,
    IEnumerable<Guid>? ingredientIds,
    decimal? priceFrom,
    decimal? priceTo)
{
    IQueryable<Product> query = products.AsNoTracking();

    if (!string.IsNullOrWhiteSpace(name))
    {
        query = query.Where(p => p.Name.ToLower().Contains(name.ToLower()));
    }

    if (categoryIds is not null && categoryIds.Any())
    {
        List<CategoryId> ids = categoryIds.Select(id => (CategoryId)id).ToList();
        query = query.Where(p => p.Categories.Any(c => ids.Contains(c.Id)));
    }

    if (ingredientIds is not null && ingredientIds.Any())
    {
        List<IngredientId> ids = ingredientIds.Select(id => (IngredientId)id).ToList();
        query = query.Where(p =>
            p.BaseIngredients.Any(i => ids.Contains(i.Id)) ||
            p.CustomIngredients.Any(i => ids.Contains(i.Id)));
    }

    if (priceFrom.HasValue)
    {
        query = query.Where(p => p.Price >= priceFrom.Value);
    }

    if (priceTo.HasValue)
    {
        query = query.Where(p => p.Price <= priceTo.Value);
    }

    return query;
}
```
- Note: ordering is handled in the handler (not in the extension) to keep `ForManage` as a pure filter
- Reference: `ForManage` extension on `DbSet<Ingredient>`

---

## Validation — `Policies/GetProductsForManageValidator.cs`

### `GetProductsForManageValidator : AbstractValidator<GetProductsForManageRequest>`
- Rules:
  | Rule | Implementation |
  |---|---|
  | Name max length | `RuleFor(x => x.Name).MaximumLength(ProductConstants.MaxNameLength)` |
  | PriceFrom positive | `RuleFor(x => x.PriceFrom).GreaterThanOrEqualTo(0).When(x => x.PriceFrom.HasValue)` |
  | PriceTo positive | `RuleFor(x => x.PriceTo).GreaterThanOrEqualTo(0).When(x => x.PriceTo.HasValue)` |
  | PriceFrom ≤ PriceTo | `RuleFor(x => x).Must(x => !x.PriceFrom.HasValue \|\| !x.PriceTo.HasValue \|\| x.PriceFrom <= x.PriceTo).WithMessage("PriceFrom must not exceed PriceTo.")` |
- Reference: `GetIngredientsForManageValidator`

---

## 7) Create unit tests

### `MyHomeRamen.UnitTests/MenuModule/Products/GetProductsForManageValidatorTests.cs`

Test cases:
| Test | Description |
|---|---|
| `Validate_ShouldPass_ForValidRequest` | All fields valid → no errors |
| `Validate_ShouldFail_WhenPriceFromIsNegative` | `PriceFrom = -1` → validation failure |
| `Validate_ShouldFail_WhenPriceToIsNegative` | `PriceTo = -1` → validation failure |
| `Validate_ShouldFail_WhenPriceFromExceedsPriceTo` | `PriceFrom = 50, PriceTo = 10` → validation failure |
| `Validate_ShouldFail_WhenNameExceedsMaxLength` | Name longer than `ProductConstants.MaxNameLength` → validation failure |

---

## 8) Create integration tests

### `MyHomeRamen.IntegrationTests/MenuModule/Products/GetProductsForManageTests.cs`
- `public sealed class GetProductsForManageTests(WebApiFactory apiFactory)`
- Endpoint: `/api/menu/products/manage`

Test cases:
| Test | Description |
|---|---|
| `GetProductsForManage_ShouldReturnOk_ForAuthenticatedAdmin` | GET with Admin auth → 200 OK, non-empty products |
| `GetProductsForManage_ShouldReturnUnauthorized_ForUnauthenticatedUser` | No auth → 401 |
| `GetProductsForManage_ShouldReturnForbidden_ForNonAdminRole` | `[Theory] [InlineData(Employee)] [InlineData(Customer)]` → 403 |
| `GetProductsForManage_ShouldReturnFilteredResults_ByName` | Pick seeded product, filter by partial name, assert all results contain the substring |
| `GetProductsForManage_ShouldReturnFilteredResults_ByCategoryId` | Pick a product's category, filter, assert results belong to that category |
| `GetProductsForManage_ShouldReturnFilteredResults_ByIngredientId` | Pick a product's base ingredient, filter by `ingredientIds`, verify match |
| `GetProductsForManage_ShouldReturnFilteredResults_ByPriceRange` | Filter `priceFrom` and `priceTo`, assert all results within range |
| `GetProductsForManage_ShouldReturnPagedResults` | Request `pageSize=1`, assert `TotalCount > 1` and `Products.Count() == 1` |

Reference: `GetIngredientsForManageTests.cs`

---

## 9) Create architecture tests

Skip — no new architectural patterns introduced.

---

## 10) Create system tests

Skip — covered by integration tests.
