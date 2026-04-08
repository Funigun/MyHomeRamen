# Feature plan — UpdateProduct (Backend)

- **Date**: 2025-07-15
- **Feature**: UpdateProduct — `PUT /api/menu/products/{id}` updating an existing product's name, description, price, category, and ingredients
- **Reference**: `UpdateIngredient` (Menu module), `CreateProduct` (Menu module)

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/Menu/Features/Products/UpdateProduct/
├── Models/
│   ├── UpdateProductIRequestId.cs
│   ├── UpdateProductRequest.cs
│   ├── UpdateProductResponse.cs
│   └── Mappings.cs
├── Policies/
│   └── UpdateProductValidator.cs
├── UpdateProductEndpoint.cs
└── UpdateProductHandler.cs
```

---

## 2) Create primitive rules and contracts

No new primitive validators needed — reuse existing `ProductNameValidator`, `ProductDescriptionValidator`, `ProductPriceValidator` from `MyHomeRamen.Common.Contracts.Menu.Products`.

---

## 3) Create models, DTOs and mappings

### `Models/UpdateProductIRequestId.cs`
- `public record struct UpdateProductIRequestId : IRequestId<UpdateProductIRequestId>`
- Property: `Guid Id { get; set; }`
- Reference: `UpdateIngredientIRequestId`

### `Models/UpdateProductRequest.cs`
- `public sealed record UpdateProductRequest(string Name, string? Description, decimal Price, Guid CategoryId, IEnumerable<Guid> IngredientIds) : IRequest<UpdateProductResponse>`
- Mutable property: `public Guid Id { get; set; }` (set from route in endpoint)
- Reference: `UpdateIngredientRequest` (API layer)

### `Models/UpdateProductResponse.cs`
- `public sealed record UpdateProductResponse(Guid Id)`
- Reference: `UpdateIngredientResponse`

### `Models/Mappings.cs`
- `internal static class Mappings`
- Extension method: `public static UpdateProductResponse ToResponse(this Product product)` → `new(product.Id.Value)`
- Reference: `UpdateIngredient/Models/Mappings.cs`

---

## 4) Create IRequestHandler implementation

### `UpdateProductHandler.cs`
- `public sealed class UpdateProductHandler(IMenuDbContext dbContext) : IRequestHandler<UpdateProductRequest, UpdateProductResponse>`
- Steps:
  1. Load `Product` with `.Include(p => p.Categories).Include(p => p.BaseIngredients)` using `GetBySelectorAsync((ProductId)request.Id, ct)`
  2. Load `Category` via `dbContext.Categories.FirstAsync(c => c.Id == (CategoryId)request.CategoryId, ct)`
  3. Load `IEnumerable<Ingredient>` via `dbContext.Ingredients.GetByIds(request.IngredientIds.Select(id => (IngredientId)id), ct)`
  4. Call `product.Update(request.Name, request.Description ?? string.Empty, request.Price, category, ingredients)`
  5. `await dbContext.SaveChangesAsync(ct)`
  6. Return `product.ToResponse()`
- Reference: `UpdateIngredientHandler`, `CreateProductHandler`

---

## 5) Create IGroupedEndpoint implementation

Not needed — `ProductsGroup` already exists.

---

## 6) Create IEndpoint implementation

### `UpdateProductEndpoint.cs`
- `public sealed class UpdateProductEndpoint : IEndpoint`
- `GroupName = "Menu"`
- Maps `MapStandardValidatedPutWithResponse<UpdateProductRequest, UpdateProductResponse>("products/{id}", HandleAsync)`
- `.WithName("UpdateProductEndpoint")`
- `.WithDescription("Updates the name, description, price, category, and ingredients of an existing product.")`
- `.RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy)`
- Handler signature: `HandleAsync([FromRoute] UpdateProductIRequestId id, [FromBody] UpdateProductRequest request, [FromServices] IRequestHandler<...> handler, CancellationToken cancellationToken)`
- Sets `request.Id = id.Id` before invoking handler
- Returns `Results.Ok(response)`
- Reference: `UpdateIngredientEndpoint`

---

## Domain changes — `MyHomeRamen.Domain/Menu/Products/Product.cs`

### New method: `Update`
- Signature: `public void Update(string name, string description, decimal price, Category category, IEnumerable<Ingredient> ingredients)`
- Steps:
  1. Set `Name = name`
  2. Set `Description = description`
  3. Set `Price = price`
  4. Clear `_categories` and add the new category
  5. Clear `_baseIngredients` and add new ingredients
  6. Call `ProductValidator.ValidateProduct(this)`
- Reference: `Ingredient.Update()` method pattern

---

## Persistence changes — `MyHomeRamen.Persistance/Common/DbExtensions.cs`

### New extension: `IsProductNameUniqueExcludingAsync`
```csharp
public static async Task<bool> IsProductNameUniqueExcludingAsync(
    this IQueryable<Product> query,
    string name,
    ProductId excludeId,
    CancellationToken cancellationToken = default)
{
    return !await query.AnyAsync(p => p.Id != excludeId && p.Name.ToLower() == name.ToLower(), cancellationToken);
}
```
- Reference: `IsIngredientNameUniqueExcludingAsync`

---

## Validation — `Policies/UpdateProductValidator.cs`

### `UpdateProductValidator : AbstractValidator<UpdateProductRequest>`
- Constructor injects: `IMenuDbContext dbContext`, `IHttpContextAccessor httpContextAccessor`
- Rules:
  | Rule | Implementation |
  |---|---|
  | Name format/length | `.SetValidator(new ProductNameValidator())` |
  | Description format/length | `.SetValidator(new ProductDescriptionValidator()!)` |
  | Price range/format | `.SetValidator(new ProductPriceValidator())` |
  | Product exists by ID | `RuleFor(x => x).MustAsync(...)` using `httpContextAccessor.HttpContext!.GetRouteValue("id")` → `dbContext.Products.ExistsByIdAsync((ProductId)id, ct)` |
  | Name unique excluding current | `RuleFor(x => x.Name).MustAsync(...)` using `httpContextAccessor` → `dbContext.Products.IsProductNameUniqueExcludingAsync(name, (ProductId)id, ct)` |
  | Category exists | `RuleFor(x => x.CategoryId).NotEmpty().MustAsync(...)` → `dbContext.Categories.AnyAsync(c => c.Id == new CategoryId(id), ct)` |
  | Ingredients not empty | `RuleFor(x => x.IngredientIds).NotEmpty()` |
- Reference: `UpdateIngredientValidator`, `CreateProductValidator`

---

## 7) Create unit tests

### `MyHomeRamen.UnitTests/MenuModule/Products/ProductUpdateTests.cs`
- New test class mirroring existing `Product*Tests` patterns

Test cases:
| Test | Description |
|---|---|
| `Update_ShouldUpdateProperties_WhenInputIsValid` | Create valid product, call `Update(...)` with new values, assert all fields updated |
| `Update_ShouldThrow_WhenNameIsEmpty` | Call `Update(name: "")`, assert `DomainException` with `ProductErrors.NameTooShort().Message` |
| `Update_ShouldThrow_WhenNameIsTooShort` | Call with name shorter than `ProductConstants.MinNameLength` |
| `Update_ShouldThrow_WhenPriceIsBelowMinimum` | Call with price < `ProductConstants.MinPrice` |
| `Update_ShouldThrow_WhenPriceIsAboveMaximum` | Call with price > `ProductConstants.MaxPrice` |
| `Update_ShouldThrow_WhenCategoryIsNull` | Call with empty categories, assert `DomainException` |

Reference: `MyHomeRamen.UnitTests/MenuModule/Products/ProductValidationTests.cs`

---

## 8) Create integration tests

### `MyHomeRamen.IntegrationTests/MenuModule/Products/UpdateProductTests.cs`
- `public sealed class UpdateProductTests(WebApiFactory apiFactory)`
- Endpoint: `/api/menu/products/{id}`

Test cases:
| Test | Description |
|---|---|
| `UpdateProduct_ShouldReturnOk_ForValidRequest` | Create product, send PUT with valid data, assert 200 OK, verify persisted changes |
| `UpdateProduct_ShouldReturnNotFound_WhenProductDoesNotExist` | `Guid.NewGuid()` → 400 (BadRequest via validator) |
| `UpdateProduct_ShouldReturnUnauthorized_ForUnauthenticatedUser` | No auth header → 401 |
| `UpdateProduct_ShouldReturnForbidden_ForNonAdminUser` | `[Theory] [InlineData(Employee)] [InlineData(Customer)]` → 403 |
| `UpdateProduct_ShouldReturnBadRequest_WhenNameAlreadyExistsOnAnotherProduct` | Create two products, try updating one with the other's name → 400 |
| `UpdateProduct_ShouldReturnBadRequest_ForInvalidRequest` | Missing name, invalid price, missing category, empty ingredients → 400 |

Reference: `UpdateIngredientTests.cs`, `CreateProductTests.cs`

### Update `MyHomeRamen.IntegrationTests/MenuModule/Common/Data/Mappings.cs`
- Add `ToUpdateProductRequest` extension method on `Product` domain entity
  ```csharp
  internal static UpdateProductRequest ToUpdateProductRequest(this Product product) =>
      new(product.Name, product.Description, product.Price, product.Categories[0].Id, product.BaseIngredients.Select(i => (Guid)i.Id));
  ```

---

## 9) Create architecture tests

Skip — no new architectural patterns introduced.

---

## 10) Create system tests

Skip — covered by integration tests.
