# Plan: Get Basket Summary

## Metadata

**Type:** Feature  
**Layers Affected:** Api, Persistance  
**Created:** 2026-05-01

## References

- GET (single) endpoint pattern: `MyHomeRamen.Api/Menu/Features/Products/GetProductByIdForManage/GetProductByIdForManageEndpoint.cs`
- GET (no input params) endpoint pattern: `MyHomeRamen.Api/Menu/Features/Categories/GetMenuCategories/GetMenuCategoriesEndpoint.cs`
- `ICurrentUser` for user identity resolution (supports both authenticated and guest users): `MyHomeRamen.Api.Common/Authorization/CurrentUser.cs`
- DbContext extensions pattern: `MyHomeRamen.Persistance/Menu/Extensions/ProductDbExtensions.cs`
- Generic repository extensions: `MyHomeRamen.Persistance/Common/RepositoryDbExtensions.cs`
- Domain models: `MyHomeRamen.Domain/ShoppingCart/Baskets/Basket.cs`, `BasketItem.cs`, `Product.cs`, `Ingredient.cs`
- Database migrations required: **No**

## Implementation Plan

### Step 1: Domain Changes

No domain changes required. All necessary domain models already exist:
- `Basket` — contains `BasketId`, `User`, `Status`, `Items`
- `BasketItem` — contains `BasketItemId`, `Product`, `Quantity`, `Price`, `Comment`
- `Product` — contains `ProductId`, `Name`, `Description`, `Price`, `ImageUrl`, `BaseIngredients`, `CustomIngredients`
- `Ingredient` — contains `IngredientId`, `Name`

### Step 2: Database Changes

**No migrations required.** Only a new query-shape extension method is needed.

**File to create:** `MyHomeRamen.Persistance/ShoppingCart/Extensions/BasketDbExtensions.cs`

Add a new partial class in `namespace MyHomeRamen.Persistance.Common` extending `IQueryable<Basket>`:

```csharp
public static partial class DbExtensions
{
    extension(IQueryable<Basket> baskets)
    {
        public IQueryable<Basket> ForUser(UserId userId)
            => baskets
                .AsNoTracking()
                .Include(b => b.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.BaseIngredients)
                .Include(b => b.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.CustomIngredients)
                .Where(b => b.User.Id == userId && b.Status == BasketStatus.Active);
    }
}
```

> **Layer boundary:** this extension returns `IQueryable<Basket>` — no DTO types. The handler owns the final projection via `Mappings.ToResponse()`.

### Step 3: Shared Validators

No shared validators required. There is no request body or query parameters — the only input is resolved from `ICurrentUser`.

### Step 4: Backend Implementation

#### Create feature folder and structure

```
MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetCurrentBasketDetails/
├── GetCurrentBasketDetailsEndpoint.cs
├── GetCurrentBasketDetailsHandler.cs
└── Models/
    ├── GetCurrentBasketDetailsRequest.cs
    ├── GetCurrentBasketDetailsResponse.cs
    ├── BasketItemDto.cs
    ├── BasketItemProductDto.cs
    ├── BasketItemIngredientDto.cs
    └── Mappings.cs
```

#### Create models, DTOs and mappings

**`Models/GetCurrentBasketDetailsRequest.cs`**  
Empty request record (no parameters — user is resolved from `ICurrentUser` in the handler):
```csharp
public sealed record GetCurrentBasketDetailsRequest;
```

**`Models/GetCurrentBasketDetailsResponse.cs`**
```csharp
public sealed record GetCurrentBasketDetailsResponse(
    Guid Id,
    IEnumerable<BasketItemDto> Items);
```

**`Models/BasketItemDto.cs`**
```csharp
public sealed record BasketItemDto(
    Guid Id,
    int Quantity,
    decimal Price,
    string? Comment,
    BasketItemProductDto Product);
```

**`Models/BasketItemProductDto.cs`**
```csharp
public sealed record BasketItemProductDto(
    Guid Id,
    string Name,
    string Description,
    string ImageUrl,
    IEnumerable<BasketItemIngredientDto> BaseIngredients,
    IEnumerable<BasketItemIngredientDto> CustomIngredients);
```

**`Models/BasketItemIngredientDto.cs`**
```csharp
public sealed record BasketItemIngredientDto(Guid Id, string Name);
```

**`Models/Mappings.cs`**  
Internal static mapping extensions:
```csharp
internal static class Mappings
{
    public static GetCurrentBasketDetailsResponse ToResponse(this Basket basket)
        => new(
            basket.Id.Value,
            basket.Items.Select(i => i.ToDto()));

    private static BasketItemDto ToDto(this BasketItem item)
        => new(
            item.Id.Value,
            item.Quantity,
            item.Price,
            item.Comment,
            item.Product.ToProductDto());

    private static BasketItemProductDto ToProductDto(this Product product)
        => new(
            product.Id.Value,
            product.Name,
            product.Description,
            product.ImageUrl,
            product.BaseIngredients.Select(i => new BasketItemIngredientDto(i.Id.Value, i.Name)),
            product.CustomIngredients.Select(i => new BasketItemIngredientDto(i.Id.Value, i.Name)));
}
```

#### Create relevant policies

No validator required. The request has no user-supplied parameters to validate — the user identity is resolved automatically from `ICurrentUser`.

#### Create IRequestHandler implementation

**`GetCurrentBasketDetailsHandler.cs`**

```csharp
public sealed class GetCurrentBasketDetailsHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    : IRequestHandler<GetCurrentBasketDetailsRequest, GetCurrentBasketDetailsResponse?>
{
    public async Task<GetCurrentBasketDetailsResponse?> Handle(GetCurrentBasketDetailsRequest request, CancellationToken cancellationToken)
    {
        UserId userId = new(currentUser.UserId);

        Basket? basket = await dbContext.ShoppingCarts
            .ForUser(userId)
            .FirstOrDefaultAsync(cancellationToken);

        return basket?.ToResponse();
    }
}
```

> When `currentUser.UserId` is `Guid.Empty` (i.e., no authenticated user and no guest cookie), the query returns `null` and the endpoint responds with `404 Not Found`.

#### Create IGroupedEndpoint implementation

No group endpoint needed. The `ShoppingCart` group is registered via `GroupName = "ShoppingCart"` on the endpoint.

#### Create IEndpoint implementation

**`GetCurrentBasketDetailsEndpoint.cs`**

```csharp
public sealed class GetCurrentBasketDetailsEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "ShoppingCart";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetCurrentBasketDetailsResponse>("basket/summary", HandleAsync)
            .WithName("GetCurrentBasketDetailsEndpoint")
            .WithDescription("Returns the active basket summary for the current user or guest.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IRequestHandler<GetCurrentBasketDetailsRequest, GetCurrentBasketDetailsResponse?> handler,
        CancellationToken cancellationToken)
    {
        GetCurrentBasketDetailsResponse? response = await handler.Handle(new GetCurrentBasketDetailsRequest(), cancellationToken);
        return response is null ? Results.NotFound() : Results.Ok(response);
    }
}
```

> **Route:** `GET /api/shoppingcart/basket/details`  
> **Auth:** `AllowAnonymous` — both authenticated users and guests (via `guest_id` cookie) are supported. The `ICurrentUser.UserId` resolves the correct identity in both cases.

### Step 5: Tests

#### Integration Tests

**File:** `MyHomeRamen.IntegrationTests/ShoppingCartModule/GetCurrentBasketDetailsTests.cs`

Test cases:
1. `GetCurrentBasketDetails_ShouldReturnOk_WhenAuthenticatedUserHasActiveBasket`  
   — Seed a basket for the test user, call `GET /api/shoppingcart/basket/summary`, assert `200 OK`, assert response contains `Id`, `Items` with full product shape (`Name`, `Description`, `ImageUrl`, `BaseIngredients`, `CustomIngredients`)

2. `GetCurrentBasketDetails_ShouldReturnOk_WhenGuestHasActiveBasket`  
   — Seed a basket for the guest user id (from cookie), send request with `guest_id` cookie, assert `200 OK` with expected basket data

3. `GetCurrentBasketDetails_ShouldReturnNotFound_WhenUserHasNoActiveBasket`  
   — Call `GET /api/shoppingcart/basket/summary` for a user with no seeded basket, assert `404 Not Found`

4. `GetCurrentBasketDetails_ShouldReturnNotFound_WhenUserIdIsEmpty`  
   — Call `GET /api/shoppingcart/basket/summary` with no auth token and no guest cookie, assert `404 Not Found`

5. `GetCurrentBasketDetails_ShouldReturnCorrectItemShape_WhenBasketContainsItemsWithBaseAndCustomIngredients`  
   — Seed a basket with items whose products have both base and custom ingredients, assert each item has the correct `BasketItemIngredientDto` lists
