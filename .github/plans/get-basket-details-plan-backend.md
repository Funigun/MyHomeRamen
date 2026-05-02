# Plan: Get Basket Details

## Metadata

**Type:** Feature
**Layers Affected:** `MyHomeRamen.Persistance` (ShoppingCart Extensions), `MyHomeRamen.Api` (ShoppingCart Feature Slice), `MyHomeRamen.IntegrationTests` (ShoppingCartModule)
**Created:** 2026-05-01

---

## References

- `MyHomeRamen.Api/Menu/Features/Products/GetProductByIdForManage/GetProductByIdForManageEndpoint.cs` — GET endpoint pattern with `MapStandardGet`
- `MyHomeRamen.Api/Menu/Features/Products/GetProductByIdForManage/GetProductByIdForManageHandler.cs` — handler with `Include` + DB extension
- `MyHomeRamen.Api/Menu/Features/Products/GetProductByIdForManage/Models/Mappings.cs` — `internal static class Mappings` mapping pattern
- `MyHomeRamen.Api/Menu/Features/Products/ProductsGroup.cs` — `IGroupEndpoint` group configuration pattern
- `MyHomeRamen.Persistance/Common/RepositoryDbExtensions.cs` — `GetListQuery` and `GetByIdQuery` DB extension base methods
- `MyHomeRamen.Api.Common/Authorization/ICurrentUser.cs` — `ICurrentUser` interface with `UserId: Guid`
- `MyHomeRamen.Api.Common/Authorization/CurrentUser.cs` — `UserId` reads `domain_id` JWT claim → fallback to guest cookie → fallback to `Guid.Empty`
- `MyHomeRamen.Api.Common/Exceptions/NotFoundException.cs` — abstract base; `ExceptionMiddleware` maps it to HTTP 404
- `MyHomeRamen.Domain/ShoppingCart/Baskets/Basket.cs` — aggregate root with `BasketId Id`, `User User`, `IReadOnlyList<BasketItem> Items`
- `MyHomeRamen.Domain/ShoppingCart/BasketItems/BasketItem.cs` — entity with `BasketItemId Id`, `Product Product`, `int Quantity`, `decimal Price`
- `MyHomeRamen.Domain/ShoppingCart/Products/Product.cs` — read-model with `string Name`, `string ImageUrl`
- `MyHomeRamen.Domain/ShoppingCart/Database/IShoppingCartDbContext.cs` — `DbSet<Basket> ShoppingCarts`
- `MyHomeRamen.Api/WebPresentation/AuthorizationDependencyInjection.cs` — policy name constants
- `MyHomeRamen.IntegrationTests/MenuModule/Products/GetProductByIdForManageTests.cs` — integration test structure pattern
- `MyHomeRamen.IntegrationTests/Common/WebApiFactory.cs` — factory setup: `DbContextOptions`, `DataSeeder`, `HttpClient` creation
- Database migrations required: No

---

## Implementation Plan

### Step 1: Domain Changes

**No domain changes required.**

All required domain entities already exist in the `ShoppingCart` module:
- `Basket` (aggregate root) — `MyHomeRamen.Domain/ShoppingCart/Baskets/`
- `BasketItem` (entity) — `MyHomeRamen.Domain/ShoppingCart/BasketItems/`
- `Product` (read-model copy) — `MyHomeRamen.Domain/ShoppingCart/Products/`
- `User` (read-model copy) — `MyHomeRamen.Domain/ShoppingCart/Users/`
- `IShoppingCartDbContext` — `MyHomeRamen.Domain/ShoppingCart/Database/`

---

### Step 2: Database Changes

**No migrations required.**

This is a read-only query endpoint. No new tables, columns, or relationships are added.

---

### Step 3: Shared Validators

**No shared validators required.**

This endpoint has no request body and no query parameters — the only input is the caller's identity resolved via `ICurrentUser.UserId` from the JWT claim, not from the request.

---

### Step 4: Backend Implementation

#### 4.1 — Create DB Extension

**File to create:** `MyHomeRamen.Persistance/ShoppingCart/Extensions/BasketDbExtensions.cs`

Create the directory `MyHomeRamen.Persistance/ShoppingCart/Extensions/` (does not exist yet).

Define a query-shape extension on `IQueryable<Basket>` that filters by the current user's ID. This encapsulates the business predicate per the persistence layer rules — the handler must never inline `Where(b => b.User.Id == ...)`.

```csharp
namespace MyHomeRamen.Persistance.Common;

public static partial class DbExtensions
{
    extension(IQueryable<Basket> baskets)
    {
        public IQueryable<Basket> ForCurrentUser(Guid userId)
            => baskets.GetListQuery(filter: b => b.User.Id == (UserId)userId);
    }
}
```

**Notes:**
- Use `namespace MyHomeRamen.Persistance.Common` so `partial class DbExtensions` merges with `RepositoryDbExtensions.cs`.
- `(UserId)userId` uses the existing implicit cast on `UserId` — do not manually unbox.
- `GetListQuery` applies `AsNoTracking()` internally.
- Required `using` statements: `MyHomeRamen.Domain.ShoppingCart.Baskets`, `MyHomeRamen.Domain.ShoppingCart.Users`.

---

#### 4.2 — Create Baskets Group

**File to create:** `MyHomeRamen.Api/ShoppingCart/Features/Baskets/BasketsGroup.cs`

```csharp
namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets;

public sealed class BasketsGroup : IGroupEndpoint
{
    public string GroupName { get; init; } = "ShoppingCart";

    public void Configure(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.WithTags("Baskets")
                    .WithDescription("Basket operations for the current authenticated user.")
                    .RequireAuthorization();   // mandatory — all basket endpoints require authentication
    }
}
```

**Notes:**
- `GroupName = "ShoppingCart"` maps to route prefix `api/shoppingcart` (lowercased by `MapEndpoints` in `DependencyInjection.cs`).
- `RequireAuthorization()` with no policy name = "authenticated user required, no role check". This satisfies the requirement "accessible to anyone beyond being authenticated".
- No per-endpoint policy is added on `GetBasketEndpoint` — it inherits the group-level restriction.

---

#### 4.3 — Create Request Model

**File to create:** `MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetBasket/Models/GetBasketRequest.cs`

```csharp
using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetBasket.Models;

public sealed record GetBasketRequest : IRequest<GetBasketResponse>;
```

**Notes:**
- This is intentionally empty — no route or query parameters exist.
- Required to satisfy the `IRequestHandler<TRequest, TResponse>` generic constraint.
- `sealed record` per the DTO conventions.

---

#### 4.4 — Create Basket Item DTO

**File to create:** `MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetBasket/Models/BasketItemDto.cs`

```csharp
namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetBasket.Models;

public sealed record BasketItemDto(
    Guid Id,
    string ProductName,
    string ProductImageUrl,
    int Quantity,
    decimal Price);
```

---

#### 4.5 — Create Response Model

**File to create:** `MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetBasket/Models/GetBasketResponse.cs`

```csharp
namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetBasket.Models;

public sealed record GetBasketResponse(
    Guid Id,
    IEnumerable<BasketItemDto> Items);
```

---

#### 4.6 — Create Mappings

**File to create:** `MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetBasket/Models/Mappings.cs`

```csharp
using MyHomeRamen.Domain.ShoppingCart.BasketItems;
using MyHomeRamen.Domain.ShoppingCart.Baskets;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetBasket.Models;

internal static class Mappings
{
    public static GetBasketResponse ToResponse(this Basket basket)
        => new(
            basket.Id.Value,
            basket.Items.Select(item => item.ToDto()));

    public static BasketItemDto ToDto(this BasketItem item)
        => new(
            item.Id.Value,
            item.Product.Name,
            item.Product.ImageUrl,
            item.Quantity,
            item.Price);
}
```

**Notes:**
- Never return domain entities directly — always map to DTOs.
- `basket.Items` is `IReadOnlyList<BasketItem>` — `Select` is valid.
- `item.Product` is loaded via `ThenInclude` in the handler (see §4.8) — it will not be `null` at this point.

---

#### 4.7 — Create Basket Not Found Exception

**File to create:** `MyHomeRamen.Api/ShoppingCart/Exceptions/BasketNotFoundException.cs`

```csharp
using MyHomeRamen.Api.Common.Exceptions;

namespace MyHomeRamen.Api.ShoppingCart.Exceptions;

public sealed class BasketNotFoundException()
    : NotFoundException("Basket not found for the current user.");
```

**Notes:**
- Extends `NotFoundException` from `MyHomeRamen.Api.Common.Exceptions`.
- `ExceptionMiddleware` catches `NotFoundException` and returns HTTP `404 Not Found`.
- Placed at module level (`MyHomeRamen.Api/ShoppingCart/Exceptions/`) so it can be reused by other basket features.

---

#### 4.8 — Create Handler

**File to create:** `MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetBasket/GetBasketHandler.cs`

```csharp
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.ShoppingCart.Exceptions;
using MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetBasket.Models;
using MyHomeRamen.Domain.ShoppingCart.Baskets;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetBasket;

public sealed class GetBasketHandler(IShoppingCartDbContext dbContext, ICurrentUser currentUser)
    : IRequestHandler<GetBasketRequest, GetBasketResponse>
{
    public async Task<GetBasketResponse> Handle(GetBasketRequest request, CancellationToken cancellationToken)
    {
        Guid userId = currentUser.UserId;

        Basket? basket = await dbContext.ShoppingCarts
            .Include(b => b.Items)
                .ThenInclude(i => i.Product)
            .ForCurrentUser(userId)
            .FirstOrDefaultAsync(cancellationToken);

        if (basket is null)
        {
            throw new BasketNotFoundException();
        }

        return basket.ToResponse();
    }
}
```

**Notes:**
- Primary constructor injects both `IShoppingCartDbContext` and `ICurrentUser` per DI conventions.
- `Include` + `ThenInclude` are placed **before** `ForCurrentUser(userId)` — consistent with the `GetProductByIdForManage` handler pattern where `.Include(...).GetByIdQuery(...)` is used.
- `ForCurrentUser` calls `GetListQuery` which applies `AsNoTracking()` — no tracking required for a read query.
- If `basket is null`, throw `BasketNotFoundException` — `ExceptionMiddleware` maps it to HTTP 404.
- Do **not** re-query the DB; `basket.ToResponse()` uses the already-loaded navigation properties.

---

#### 4.9 — Create Endpoint

**File to create:** `MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetBasket/GetBasketEndpoint.cs`

```csharp
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetBasket.Models;

namespace MyHomeRamen.Api.ShoppingCart.Features.Baskets.GetBasket;

public sealed class GetBasketEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "ShoppingCart";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetBasketResponse>("baskets", HandleAsync)
            .WithName("GetBasketEndpoint")
            .WithDescription("Returns the active basket and its items for the current authenticated user.");
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IRequestHandler<GetBasketRequest, GetBasketResponse> handler,
        CancellationToken cancellationToken)
    {
        GetBasketResponse response = await handler.Handle(new GetBasketRequest(), cancellationToken);
        return Results.Ok(response);
    }
}
```

**Notes:**
- `GroupName = "ShoppingCart"` → route becomes `GET /api/shoppingcart/baskets`.
- `MapStandardGet<GetBasketResponse>` is used (no `Validated` variant) because there are no input parameters to validate.
- No `.RequireAuthorization()` per-endpoint — authentication is inherited from `BasketsGroup.RequireAuthorization()`.
- `new GetBasketRequest()` creates the empty marker record to call the handler.
- `ICurrentUser` is NOT injected into the endpoint — it is injected into the handler via primary constructor, maintaining the REPR pattern boundary.
- `[FromServices]` is required for `IRequestHandler` — consistent with all other endpoint handlers in the codebase.

---

### Step 5: Tests

#### 5.1 — Extend WebApiFactory

**File to modify:** `MyHomeRamen.IntegrationTests/Common/WebApiFactory.cs`

Add a `ShoppingCartDbContext` property and seed the ShoppingCart module during `InitializeAsync`.

**Changes:**
1. Add using: `using MyHomeRamen.Persistance.ShoppingCart;` and `using MyHomeRamen.IntegrationTests.ShoppingCartModule.Common.Data;`
2. Add property: `public ShoppingCartDbContext ShoppingCartDbContext { get; private set; } = default!;`
3. In `InitializeAsync`, after `MenuDbContext` setup, add:
```csharp
DbContextOptions<ShoppingCartDbContext> basketOptions =
    new DbContextOptionsBuilder<ShoppingCartDbContext>()
        .UseSqlServer(_sqlContainer.GetConnectionString())
        .Options;
ShoppingCartDbContext = new ShoppingCartDbContext(basketOptions, user);
await ShoppingCartDataSeeder.SeedShoppingCartModule(ShoppingCartDbContext);
```
4. In `DisposeAsync`, add: `await ShoppingCartDbContext.DisposeAsync();`

**Notes:**
- The same `FakeUser user` instance (already declared for `MenuDbContext`) is reused.
- `ShoppingCartDbContext` accepts `(DbContextOptions, ICurrentUser)` per its constructor signature.

---

#### 5.2 — Create ShoppingCart Data Generator

**File to create:** `MyHomeRamen.IntegrationTests/ShoppingCartModule/Common/Data/DataGenerator.cs`

```csharp
namespace MyHomeRamen.IntegrationTests.ShoppingCartModule.Common.Data;

internal static class DataGenerator
{
    // The test user ID matches FakeUser.UserId = Guid.Empty
    // This ensures currentUser.UserId resolves to Guid.Empty in tests
    // (no domain_id JWT claim → TryGetUserId() returns null → fallback to Guid.Empty)
    internal static readonly Guid TestUserId = Guid.Empty;

    internal static IEnumerable<Basket> GeneratedBaskets { get; private set; } = [];

    internal static Basket GenerateValidBasket(User testUser, IEnumerable<Product> products)
    {
        // Use Bogus Faker<BasketItem> with CustomInstantiator to build items
        // Pick products from the provided list
        // Return a Basket linked to testUser with 1–3 BasketItem entries
    }

    internal static User GenerateTestUser()
        => User.Create(TestUserId, roles: [], permissions: [], isGuest: false);

    internal static IEnumerable<Product> GenerateValidProducts(int count)
    {
        // Use Bogus Faker<Product> with CustomInstantiator
        // Each product has Name, ImageUrl, Price, Description
        // OriginalId = Id (self-referencing copy)
    }
}
```

**Implementation notes for the developer:**
- `BasketItem.Create(id, product, quantity, price, comment)` — use `Guid.NewGuid()` for IDs.
- `Basket.Create(id, user)` — then add items via `_items` field (or via seeded DB relationship).
- Since `Basket` exposes `Items` as `IReadOnlyList<BasketItem>` with a private backing `List<BasketItem>`, items should be added through `dbContext.BasketItems.AddRange(items)` with the basket FK set by EF, OR if `Basket` provides no `AddItem` method, items are seeded separately with the basket reference.
- Track `GeneratedBaskets` so tests can reference seeded data by index.

---

#### 5.3 — Create ShoppingCart Data Seeder

**File to create:** `MyHomeRamen.IntegrationTests/ShoppingCartModule/Common/Data/ShoppingCartDataSeeder.cs`

```csharp
namespace MyHomeRamen.IntegrationTests.ShoppingCartModule.Common.Data;

internal static class ShoppingCartDataSeeder
{
    internal static async Task SeedShoppingCartModule(ShoppingCartDbContext dbContext)
    {
        await dbContext.Migrate(TestContext.Current.CancellationToken);
        await dbContext.Seed(ApiConfig.RestaurantId, TestContext.Current.CancellationToken);

        // 1. Generate and seed Products
        List<Product> products = DataGenerator.GenerateValidProducts(5).ToList();
        dbContext.Products.AddRange(products);

        // 2. Generate and seed the test User (UserId = Guid.Empty = FakeUser.UserId)
        User testUser = DataGenerator.GenerateTestUser();
        dbContext.Users.Add(testUser);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // 3. Generate and seed Basket (with BasketItems referencing seeded products)
        Basket basket = DataGenerator.GenerateValidBasket(testUser, products);
        dbContext.ShoppingCarts.Add(basket);
        dbContext.BasketItems.AddRange(basket.Items);

        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
```

**Notes:**
- Seed order: Products → Users → Baskets + BasketItems (respects FK dependency order).
- `ApiConfig.RestaurantId` is the shared test restaurant GUID from `MyHomeRamen.IntegrationTests/Common/Configuration/ApiConfig.cs`.

---

#### 5.4 — Create Integration Tests

**File to create:** `MyHomeRamen.IntegrationTests/ShoppingCartModule/GetBasketTests.cs`

```csharp
namespace MyHomeRamen.IntegrationTests.ShoppingCartModule;

public sealed class GetBasketTests(WebApiFactory apiFactory)
{
    private const string EndpointBase = "/api/shoppingcart/baskets";

    // ── Happy Path ─────────────────────────────────────────────────────────

    [Fact]
    public async Task GetBasket_ShouldReturnOk_ForAuthenticatedCustomer()
    // Arrange: auth as Customer; Act: GET /api/shoppingcart/baskets
    // Assert: 200 OK, response.Id == seeded basket Id, response.Items is non-empty

    [Fact]
    public async Task GetBasket_ShouldReturnBasketWithCorrectItems_ForAuthenticatedCustomer()
    // Arrange: auth as Customer; pick first seeded basket item
    // Act: GET /api/shoppingcart/baskets
    // Assert: 200 OK; each item in response has Id, ProductName, ProductImageUrl, Quantity, Price
    //         matching the seeded BasketItem and its Product

    // ── Authorization ──────────────────────────────────────────────────────

    [Theory]
    [InlineData(UserRoles.Employee)]
    [InlineData(UserRoles.Admin)]
    public async Task GetBasket_ShouldReturnOk_ForAnyAuthenticatedRole(UserRoles role)
    // Arrange: auth as Employee or Admin (endpoint has no role restriction)
    // Act: GET /api/shoppingcart/baskets
    // Assert: 200 OK — confirms no role restriction beyond authentication
}
```

**Implementation notes for the developer:**
- Inject `WebApiFactory apiFactory` via primary constructor.
- Use `HttpClientExtensions.CreateGetMessage(EndpointBase).AddAuthorizationHeader(role)` to build requests.
- Use `responseMessage.Content.ReadFromJsonAsync<GetBasketResponse>(...)` to deserialize.
- Use `DataGenerator.GeneratedBaskets.First()` and `DataGenerator.GeneratedBaskets.First().Items.First()` to get expected values for assertions.

---

## File Summary

| # | Action | File Path |
|---|--------|-----------|
| 1 | **CREATE** | `MyHomeRamen.Persistance/ShoppingCart/Extensions/BasketDbExtensions.cs` |
| 2 | **CREATE** | `MyHomeRamen.Api/ShoppingCart/Exceptions/BasketNotFoundException.cs` |
| 3 | **CREATE** | `MyHomeRamen.Api/ShoppingCart/Features/Baskets/BasketsGroup.cs` |
| 4 | **CREATE** | `MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetBasket/Models/GetBasketRequest.cs` |
| 5 | **CREATE** | `MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetBasket/Models/BasketItemDto.cs` |
| 6 | **CREATE** | `MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetBasket/Models/GetBasketResponse.cs` |
| 7 | **CREATE** | `MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetBasket/Models/Mappings.cs` |
| 8 | **CREATE** | `MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetBasket/GetBasketHandler.cs` |
| 9 | **CREATE** | `MyHomeRamen.Api/ShoppingCart/Features/Baskets/GetBasket/GetBasketEndpoint.cs` |
| 10 | **CREATE** | `MyHomeRamen.IntegrationTests/ShoppingCartModule/Common/Data/DataGenerator.cs` |
| 11 | **CREATE** | `MyHomeRamen.IntegrationTests/ShoppingCartModule/Common/Data/ShoppingCartDataSeeder.cs` |
| 12 | **CREATE** | `MyHomeRamen.IntegrationTests/ShoppingCartModule/GetBasketTests.cs` |
| 13 | **MODIFY** | `MyHomeRamen.IntegrationTests/Common/WebApiFactory.cs` |

---

## Key Design Decisions

| Decision | Rationale |
|---|---|
| Empty `GetBasketRequest` record | No route/query params exist; required to satisfy `IRequestHandler<TRequest,TResponse>` generic constraint and maintain REPR pattern uniformity |
| `ICurrentUser` injected in **Handler** (not Endpoint) | Handler owns business logic; endpoint is a thin routing adapter — consistent with all other handlers in the codebase |
| `MapStandardGet<TResponse>` (no `Validated` variant) | No input to validate; validation filter would add overhead with no benefit |
| `BasketNotFoundException` at module level | Reusable across all basket features; `ExceptionMiddleware` maps `NotFoundException` → HTTP 404 |
| `ForCurrentUser(Guid userId)` returns `IQueryable<Basket>` | Persistence layer must not return API DTOs; handler owns the final projection via `Mappings.ToResponse()` |
| `Include` + `ThenInclude` **before** `ForCurrentUser` | Consistent with `GetProductByIdForManageHandler` where `Include` precedes `GetByIdQuery` extension |
| Test `UserId = Guid.Empty` | `JwtTokenFactory` does not include a `domain_id` claim → `CurrentUser.TryGetUserId()` returns null → fallback to `Guid.Empty`; seeding with `Guid.Empty` aligns with this |
