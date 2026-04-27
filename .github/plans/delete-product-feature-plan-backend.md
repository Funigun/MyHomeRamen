# Plan: Delete Product — Backend

## Metadata

**Type:** Feature  
**Layers Affected:** Api, Persistance  
**Created:** 2025-07-14

## References

- Existing DELETE pattern: `MyHomeRamen.Api/Menu/Features/Ingredients/DeleteIngredient/`
- Cache invalidation pattern: `MyHomeRamen.Api/Menu/Features/Categories/DeleteCategory/DeleteCategoryHandler.cs`
- Product cache keys: `MyHomeRamen.Api/Menu/Features/Products/Caching/ProductCacheInvalidation.cs`
- DB existence check pattern: `MyHomeRamen.Persistance/Common/RepositoryDbExtensions.cs` (`Exists`)
- Product DB extensions: `MyHomeRamen.Persistance/Menu/Extensions/ProductDbExtensions.cs`
- Authorization policy: `AuthorizationDependencyInjection.RestaurantManagerPolicy` (same as Delete Ingredient)
- Database migrations required: **No** (no domain model changes)

---

## Implementation Plan

### Step 1: Domain Changes

No domain model changes required. `Product` entity already supports deletion via EF Core's `DbContext.Remove`.

---

### Step 2: Database Changes

No new migrations needed.  
However, a new DB extension method is needed to check product existence before deletion (following the mandatory DB extension rule).

**File to modify:** `MyHomeRamen.Persistance/Menu/Extensions/ProductDbExtensions.cs`

Add the following extension inside the existing `extension(IQueryable<Product> products)` block:

```csharp
public async Task<bool> ExistsAsync(ProductId productId, CancellationToken cancellationToken = default)
    => await products.Exists(p => p.Id == productId, cancellationToken);
```

> **Note:** Check whether the generic `Exists` overload from `RepositoryDbExtensions` already covers this case (e.g. `dbContext.Products.Exists(p => p.Id == id, ct)`) — if so, the validator can call it inline without a dedicated extension. Follow the pattern used in `DeleteIngredientValidator` which calls `Exists` directly.

---

### Step 3: Shared Validators

No changes required in `MyHomeRamen.Common.Contracts`. Product ID existence is a backend-only concern validated in the feature validator.

---

### Step 4: Backend Implementation

#### 4.1 Create feature folder and structure

```
MyHomeRamen.Api/Menu/Features/Products/DeleteProduct/
├── Models/
│   └── DeleteProductRequest.cs
├── Policies/
│   └── DeleteProductValidator.cs
├── DeleteProductEndpoint.cs
└── DeleteProductHandler.cs
```

#### 4.2 Create request model

**File:** `MyHomeRamen.Api/Menu/Features/Products/DeleteProduct/Models/DeleteProductRequest.cs`

Mirror the `DeleteIngredientRequest` pattern — implement `IRequestId<DeleteProductRequest>` and `IRequest<IResult>`:

```csharp
using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Products.DeleteProduct.Models;

public record struct DeleteProductRequest : IRequestId<DeleteProductRequest>, IRequest<IResult>
{
    public Guid Id { get; set; }
}
```

> No `Mappings.cs` or `Response` needed — DELETE returns `204 No Content`.

#### 4.3 Create validation policy

**File:** `MyHomeRamen.Api/Menu/Features/Products/DeleteProduct/Policies/DeleteProductValidator.cs`

Validates:
- `Id` is not empty.
- Product with the given ID exists (calls `dbContext.Products.Exists(p => p.Id == (ProductId)id, ct)`).

```csharp
using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Products.DeleteProduct.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.DeleteProduct.Policies;

public sealed class DeleteProductValidator : AbstractValidator<DeleteProductRequest>
{
    public DeleteProductValidator(IMenuDbContext menuDbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Product ID must not be empty.")
            .ChildRules(id =>
                id.RuleFor(id => id)
                    .MustAsync(ProductExists(menuDbContext))
                    .WithMessage("Product with the specified ID does not exist."));
    }

    private static Func<Guid, CancellationToken, Task<bool>> ProductExists(IMenuDbContext menuDbContext)
        => async (id, cancellationToken)
            => await menuDbContext.Products.Exists(p => p.Id == (ProductId)id, cancellationToken);
}
```

#### 4.4 Create request handler

**File:** `MyHomeRamen.Api/Menu/Features/Products/DeleteProduct/DeleteProductHandler.cs`

Steps:
1. Load product including `Categories` (needed to derive cache keys for all affected category caches).
2. Remove entity.
3. Save changes.
4. Invalidate relevant caches via `ProductCacheInvalidation.GetAffectedKeys(categoryIds)`.
5. Return `204 No Content`.

```csharp
using MyHomeRamen.Api.Common.Cache;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.Caching;
using MyHomeRamen.Api.Menu.Features.Products.DeleteProduct.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;
using Microsoft.EntityFrameworkCore;

namespace MyHomeRamen.Api.Menu.Features.Products.DeleteProduct;

public sealed class DeleteProductHandler(IMenuDbContext dbContext, ICacheService cacheService)
    : IRequestHandler<DeleteProductRequest, IResult>
{
    public async Task<IResult> Handle(DeleteProductRequest id, CancellationToken cancellationToken)
    {
        Product product = await dbContext.Products
            .Include(p => p.Categories)
            .GetById((ProductId)id.Id, cancellationToken);

        IEnumerable<Guid> categoryIds = product.Categories.Select(c => (Guid)c.Id);

        dbContext.Products.Remove(product);

        await dbContext.SaveChangesAsync(cancellationToken);

        await ClearCacheAsync(categoryIds, cancellationToken);

        return Results.NoContent();
    }

    private async Task ClearCacheAsync(IEnumerable<Guid> categoryIds, CancellationToken cancellationToken)
    {
        IEnumerable<Task> cacheClearance = ProductCacheInvalidation
            .GetAffectedKeys(categoryIds)
            .Select(key => cacheService.RemoveByKeyAsync(key, cancellationToken));

        await Task.WhenAll(cacheClearance);
    }
}
```

#### 4.5 Create endpoint

**File:** `MyHomeRamen.Api/Menu/Features/Products/DeleteProduct/DeleteProductEndpoint.cs`

Mirror `DeleteIngredientEndpoint` — use `MapStandardValidatedDelete`, `RestaurantManagerPolicy`:

```csharp
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.DeleteProduct.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Products.DeleteProduct;

public sealed class DeleteProductEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedDelete<DeleteProductRequest>("products/{id}", HandleAsync)
                       .WithName("DeleteProductEndpoint")
                       .WithDescription("Deletes a product by its ID.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        DeleteProductRequest id,
        [FromServices] IRequestHandler<DeleteProductRequest, IResult> handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(id, cancellationToken);
    }
}
```

#### 4.6 No ProductGroup changes needed

The existing `ProductGroup.cs` already groups all product endpoints under the `"Menu"` group — no changes required.
