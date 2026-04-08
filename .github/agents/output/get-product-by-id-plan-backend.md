# Feature plan — GetProductByIdForManage (Backend)

- **Date**: 2025-07-15
- **Feature**: GetProductByIdForManage — `GET /api/menu/products/{id}` returning full product details for the admin management/edit view
- **Reference**: `GetIngredientById` (Menu module), `UpdateProduct` brief (prerequisite section)

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/Menu/Features/Products/GetProductByIdForManage/
├── Models/
│   ├── GetProductByIdForManageRequest.cs
│   ├── GetProductByIdForManageResponse.cs
│   └── Mappings.cs
├── Policies/
│   └── GetProductByIdForManageValidator.cs
├── GetProductByIdForManageEndpoint.cs
└── GetProductByIdForManageHandler.cs
```

---

## 2) Create primitive rules and contracts

No new primitive validators needed — this is a read-only query endpoint with only an ID parameter.

---

## 3) Create models, DTOs and mappings

### `Models/GetProductByIdForManageRequest.cs`
- `public record struct GetProductByIdForManageRequest : IRequestId<GetProductByIdForManageRequest>, IRequest<GetProductByIdForManageResponse>`
- Property: `Guid Id { get; set; }`
- Reference: `GetIngredientByIdRequest`

### `Models/GetProductByIdForManageResponse.cs`
- `public sealed record GetProductByIdForManageResponse(Guid Id, string Name, string Description, decimal Price, Guid CategoryId, IEnumerable<Guid> IngredientIds)`
- `CategoryId` → first category from `product.Categories`
- `IngredientIds` → base ingredients (`product.BaseIngredients`)
- Reference: `GetIngredientByIdResponse`

### `Models/Mappings.cs`
- `internal static class Mappings`
- Extension method: `public static GetProductByIdForManageResponse ToResponse(this Product product)`
  - Maps `product.Id.Value`, `product.Name`, `product.Description`, `product.Price`
  - Maps `product.Categories.First().Id.Value` → `CategoryId`
  - Maps `product.BaseIngredients.Select(i => i.Id.Value)` → `IngredientIds`
- Reference: `GetIngredientById/Models/Mappings.cs`

---

## 4) Create IRequestHandler implementation

### `GetProductByIdForManageHandler.cs`
- `public sealed class GetProductByIdForManageHandler(IMenuDbContext dbContext) : IRequestHandler<GetProductByIdForManageRequest, GetProductByIdForManageResponse>`
- Load product with `.Include(p => p.Categories).Include(p => p.BaseIngredients)` using `GetBySelectorNotTrackedAsync` with `(ProductId)request.Id`
- Return `product.ToResponse()`
- Reference: `GetIngredientByIdHandler`

---

## 5) Create IGroupedEndpoint implementation

Not needed — `ProductsGroup` already exists.

---

## 6) Create IEndpoint implementation

### `GetProductByIdForManageEndpoint.cs`
- `public sealed class GetProductByIdForManageEndpoint : IEndpoint`
- `GroupName = "Menu"`
- Maps `MapStandardValidatedGet<GetProductByIdForManageRequest, GetProductByIdForManageResponse>("products/{id}", HandleAsync)`
- `.WithName("GetProductByIdForManageEndpoint")`
- `.WithDescription("Returns the full details of a single product by its ID for the management view.")`
- `.RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy)`
- Handler signature: `HandleAsync(GetProductByIdForManageRequest id, [FromServices] IRequestHandler<...> handler, CancellationToken cancellationToken)`
- Returns `Results.Ok(response)`
- Reference: `GetIngredientByIdEndpoint`

---

## 7) Create unit tests

Skip — no domain logic to test. The endpoint is a read-only query with validation handled by FluentValidation.

---

## 8) Create integration tests

### `MyHomeRamen.IntegrationTests/MenuModule/Products/GetProductByIdForManageTests.cs`
- `public sealed class GetProductByIdForManageTests(WebApiFactory apiFactory)`
- Endpoint base: `/api/menu/products`

Test cases:
| Test | Description |
|---|---|
| `GetProductByIdForManage_ShouldReturnOk_ForAuthenticatedAdmin` | Picks a seeded product from `DataGenerator.GeneratedProducts`, sends GET, asserts 200 OK with matching `Id`, `Name`, `Price` |
| `GetProductByIdForManage_ShouldReturnUnauthorized_ForUnauthenticatedUser` | No auth header → 401 |
| `GetProductByIdForManage_ShouldReturnForbidden_ForNonAdminRole` | `[Theory] [InlineData(UserRoles.Employee)] [InlineData(UserRoles.Customer)]` → 403 |
| `GetProductByIdForManage_ShouldReturnBadRequest_ForNonExistentId` | `Guid.NewGuid()` → 400 |
| `GetProductByIdForManage_ResponseShouldContainCategoryAndIngredientIds` | Picks product with categories/ingredients, asserts `CategoryId` and `IngredientIds` match |

Reference: `GetIngredientByIdTests.cs`

---

## 9) Create architecture tests

Skip — no new architectural patterns introduced.

---

## 10) Create system tests

Skip — covered by integration tests.
