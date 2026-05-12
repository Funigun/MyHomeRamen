# Plan: Delete Basket Item – Backend

## References

- Existing `DeleteCategoryEndpoint`, `DeleteCategoryHandler`, `DeleteCategoryValidator`, `DeleteCategoryRequest` in
  `MyHomeRamen.Api/Menu/Features/Categories/DeleteCategory/` – primary DELETE pattern reference
- `BasketsGroup.cs` – `MyHomeRamen.Api/ShoppingCart/Features/Baskets/BasketsGroup.cs` – existing group with `RequireAuthorization()`
- `AddItemToBasketEndpoint/Handler/Validator` – sibling basket feature reference
- `EndpointBuilderExtensions.MapStandardValidatedDelete<TRequest>` in `MyHomeRamen.Api.Common/Endpoint/EndpointBuilderExtensions.cs`
- `IRequestId<TRequest>` in `MyHomeRamen.Api.Common/Endpoint/Models/IRequestId.cs` – route-param struct pattern
- `RepositoryDbExtensions.Exists(...)` / `GetById(...)` in `MyHomeRamen.Persistance/Common/RepositoryDbExtensions.cs`
- `BasketDbExtensions.ForUserTracked(UserId)` in `MyHomeRamen.Persistance/ShoppingCart/Extensions/BasketDbExtensions.cs`
- `IShoppingCartDbContext` in `MyHomeRamen.Domain/ShoppingCart/Database/IShoppingCartDbContext.cs`
- `BasketItemId` (strongly-typed ID) in `MyHomeRamen.Domain/ShoppingCart/BasketItems/BasketItemId.cs`
- `ICurrentUser` from `MyHomeRamen.Api.Common/Authorization/`
- Integration test patterns: siblings `AddItemToBasketTests`, `GetCurrentBasketDetailsTests`
- Database migrations required: **No** – delete operation, no schema changes

## Implementation Plan

### Step 1: Domain Changes

No domain model changes are required. `BasketItem` already supports removal via EF Core's `DbSet.Remove()`. The `Basket` domain entity does not need a `RemoveItem` method — EF Core can track and remove `BasketItem` entities directly through `dbContext.BasketItems`.

### Step 2: Database Changes

None required. The DELETE operation removes an existing row; no schema alterations are needed.

### Step 3: Shared Validators

No new `Common.Contracts` primitive validators needed. The only validation is structural (non-empty ID) and database-backed (ownership check), both of which live in the feature's `Policies/` folder.

### Step 4: Backend Implementation

#### Create feature folder and structure

```
MyHomeRamen.Api/ShoppingCart/Features/Baskets/
└── DeleteBasketItem/
    ├── Models/
    │   └── DeleteBasketItemRequest.cs
    ├── Policies/
    │   └── DeleteBasketItemValidator.cs
    ├── DeleteBasketItemEndpoint.cs
    └── DeleteBasketItemHandler.cs
```

#### Create Models

**`DeleteBasketItemRequest.cs`**

```csharp
using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.DeleteBasketItem.Models;

public record struct DeleteBasketItemRequest : IRequestId<DeleteBasketItemRequest>, IRequest<IResult>
{
    public Guid Id { get; set; }
}
```

- Pattern: mirrors `DeleteCategoryRequest` exactly.
- Implements `IRequestId<DeleteBasketItemRequest>` so the Minimal API framework can bind the `{id}` route segment via `TryParse`.
- Implements `IRequest<IResult>` – the handler returns `Results.NoContent()`.

#### Create Relevant Policies

**`Policies/DeleteBasketItemValidator.cs`**

```csharp
using FluentValidation;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.ShoppingCart.Features.Baskets.DeleteBasketItem.Models;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.DeleteBasketItem.Policies;

public sealed class DeleteBasketItemValidator : AbstractValidator<DeleteBasketItemRequest>
{
    public DeleteBasketItemValidator(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Basket item ID must not be empty.")
            .MustAsync(async (id, ct) =>
                await dbContext.ShoppingCarts
                    .Exists(
                        b => b.User.Id == (UserId)currentUser.UserId
                          && b.Items.Any(item => item.Id == (BasketItemId)id),
                        ct))
            .WithMessage("Basket item not found or does not belong to the current user.");
    }
}
```

- Combines existence and ownership checks into a single `MustAsync` to avoid leaking information about other users' basket items.
- `UserId` is from `MyHomeRamen.Domain.ShoppingCart.Users` (implicit operator cast from `Guid`).
- `BasketItemId` uses the implicit operator `(BasketItemId)id`.

#### Create `IRequestHandler` Implementation

**`DeleteBasketItemHandler.cs`**

```csharp
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.ShoppingCart.Features.Baskets.DeleteBasketItem.Models;
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.DeleteBasketItem;

public sealed class DeleteBasketItemHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    : IRequestHandler<DeleteBasketItemRequest, IResult>
{
    public async Task<IResult> Handle(DeleteBasketItemRequest request, CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        BasketItem basketItem = await dbContext.BasketItems
            .GetById((BasketItemId)request.Id, cancellationToken);

        dbContext.BasketItems.Remove(basketItem);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Results.NoContent();
    }
}
```

- Follows the CQRS command rules: no query after `SaveChangesAsync`, returns `204 NoContent`.
- `GetById` throws `InvalidOperationException` if not found – validator guarantees the item exists and belongs to the user before the handler is reached.
- Security is enforced at the validator layer (ownership check) and reinforced by the group-level `RequireAuthorization()`.

#### Create `IGroupedEndpoint` Implementation

No new `IGroupEndpoint` needed. The existing `BasketsGroup.cs` already groups all basket endpoints under `GroupName = "ShoppingCart"` with `RequireAuthorization()`. `DeleteBasketItemEndpoint` declares the same `GroupName`.

#### Create `IEndpoint` Implementation

**`DeleteBasketItemEndpoint.cs`**

```csharp
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.ShoppingCart.Features.Baskets.DeleteBasketItem.Models;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.DeleteBasketItem;

public sealed class DeleteBasketItemEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "ShoppingCart";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedDelete<DeleteBasketItemRequest>(
                "basket/items/{id}", HandleAsync)
            .WithName("DeleteBasketItemEndpoint")
            .WithDescription("Removes a basket item from the current authenticated user's active basket.");
        // No .AllowAnonymous() – inherits RequireAuthorization() from BasketsGroup
    }

    private static async Task<IResult> HandleAsync(
        DeleteBasketItemRequest id,
        [FromServices] IRequestHandler<DeleteBasketItemRequest, IResult> handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(id, cancellationToken);
    }
}
```

- Route: `basket/items/{id}` – consistent with existing `basket/items` (POST) in the same group.
- No role-specific policy applied beyond the group's `RequireAuthorization()`.

### Step 5: Tests

#### Unit Tests

Not needed – the handler has no branching logic beyond what is guaranteed by the validator.

#### Integration Tests

**Create:** `MyHomeRamen.IntegrationTests/ShoppingCartModule/Baskets/DeleteBasketItemTests.cs`

| # | Test Method | Expected |
|---|-------------|---------|
| 1 | `DeleteBasketItem_ShouldReturnNoContent_WhenItemExistsAndBelongsToCurrentUser` | 204 NoContent |
| 2 | `DeleteBasketItem_ShouldReturnUnauthorized_WhenRequestHasNoAuthToken` | 401 Unauthorized |
| 3 | `DeleteBasketItem_ShouldReturnBadRequest_WhenBasketItemIdIsEmpty` | 400 Bad Request |
| 4 | `DeleteBasketItem_ShouldReturnBadRequest_WhenBasketItemDoesNotExist` | 400 Bad Request |
| 5 | `DeleteBasketItem_ShouldReturnBadRequest_WhenBasketItemBelongsToDifferentUser` | 400 Bad Request |

**Test infrastructure notes:**

- `EndpointBase = "/api/shoppingcart/basket/items"` – route is `{EndpointBase}/{id}`
- Test 1: Use a seeded basket item ID from `DataGenerator`; authenticate as the basket owner via `AddAuthorizationHeader(UserRoles.Customer, userId.Value.ToString())`
- Test 2: Send DELETE with no auth header
- Test 3: Send DELETE to `{EndpointBase}/{Guid.Empty}`
- Test 4: Send DELETE with a random `Guid.NewGuid()` that was never seeded
- Test 5: Seed a second user's basket item; send DELETE authenticated as the first user targeting the second user's basket item – expect 400 (ownership validation fails)
