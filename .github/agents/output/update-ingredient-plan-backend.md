# Feature Implementation Plan — UpdateIngredient (Backend)

- **Date**: 2025-07-15
- **Feature**: UpdateIngredient
- **Module**: Menu
- **Type**: Feature (Command — PUT endpoint)

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/
└── Menu/
    └── Features/
        └── Ingredients/
            └── UpdateIngredient/
                ├── Models/
                │   ├── Mappings.cs
                │   ├── UpdateIngredientRequest.cs
                │   └── UpdateIngredientResponse.cs
                ├── Policies/
                │   └── UpdateIngredientValidator.cs
                ├── UpdateIngredientEndpoint.cs
                └── UpdateIngredientHandler.cs
```

Also modified:
- `MyHomeRamen.Domain/Menu/Ingredients/Ingredient.cs` — add `Update()` method
- `MyHomeRamen.Persistance/Common/DbExtensions/DbExtensions.cs` — add `IsIngredientNameUniqueExcludingAsync`
- `MyHomeRamen.Api.Common/Endpoint/EndpointBuilderExtensions.cs` — add `MapStandardValidatedPutWithResponse<TRequest, TResponse>`

---

## 2) Create primitive rules and contracts

No new primitive validators needed — all body-field rules reuse the existing validators already used by `CreateIngredientValidator`:
- `IngredientNameValidator` (name length)
- `IngredientDescriptionValidator` (description length)
- `IngredientPriceValidator` (price range)

All in `MyHomeRamen.Common.Contracts`.

---

## 3) Domain changes — `Ingredient.Update()` method

### `MyHomeRamen.Domain/Menu/Ingredients/Ingredient.cs` (MODIFIED)

Add a public mutation method alongside the existing `Create` factory:

```csharp
public void Update(string name, string description, decimal price, Collection<Category> categories)
{
    Name = name;
    Description = description;
    Price = price;
    _categories.Clear();
    foreach (Category category in categories)
    {
        _categories.Add(category);
    }
    IngredientValidator.Validate(this);
}
```

- Sets all mutable fields, then calls `IngredientValidator.Validate(this)` to enforce domain invariants
- `_categories` is the private backing collection (same field used in `Create`)
- Reference: `Ingredient.Create()` method and `IngredientValidator.Validate()`

---

## 4) Create persistence extension

### `MyHomeRamen.Persistance/Common/DbExtensions/DbExtensions.cs` (MODIFIED)

Add a new uniqueness-check extension that excludes the ingredient being updated:

```csharp
public static async Task<bool> IsIngredientNameUniqueExcludingAsync(
    this IQueryable<Ingredient> query,
    string name,
    IngredientId excludeId,
    CancellationToken cancellationToken = default)
{
    return !await query.AnyAsync(
        i => i.Id != excludeId && i.Name.ToLower() == name.ToLower(),
        cancellationToken);
}
```

- Returns `true` when no **other** ingredient (Id ≠ excludeId) has the same name (case-insensitive)
- Called from `UpdateIngredientValidator` (not the handler) — see section 9
- Reference: existing `IsIngredientNameUniqueAsync` directly above this method

---

## 5) Create endpoint extension

### `MyHomeRamen.Api.Common/Endpoint/EndpointBuilderExtensions.cs` (MODIFIED)

Add a PUT variant that returns `200 OK` with a response body (parallel to `MapStandardValidatedPost`):

```csharp
public static RouteHandlerBuilder MapStandardValidatedPutWithResponse<TRequest, TResponse>(
    this IEndpointRouteBuilder builder, string pattern, Delegate handler)
{
    RouteHandlerBuilder routeHandler = builder.MapPut(pattern, handler)
                                              .Produces<TResponse>(StatusCodes.Status200OK)
                                              .ProducesProblem(StatusCodes.Status400BadRequest)
                                              .ProducesProblem(StatusCodes.Status404NotFound)
                                              .ProducesProblem(StatusCodes.Status500InternalServerError)
                                              .WithMetadata(typeof(TRequest).DeclaringType!);

    return routeHandler.WithValidationFilter<TRequest>();
}
```

- Returns `200 OK` with `TResponse` body (unlike `MapStandardValidatedPut` which produces `204 NoContent`)
- Reference: `MapStandardValidatedPost` and `MapStandardValidatedPut` in the same file

---

## 6) Create models, DTOs and mappings

### `Models/UpdateIngredientRequest.cs`

```csharp
public sealed record UpdateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds) : IRequest<UpdateIngredientResponse>;
```

- Body-only record — no `Guid Id` property
- `id` is bound from the route parameter in `UpdateIngredientEndpoint` and passed separately to the handler
- **Design note**: `IRequestId<T>` is not used here because `TryParse` can only bind route strings — it cannot also bind a JSON body. The body-only request is what the `ValidationFilter<TRequest>` finds in the handler's argument list and validates before the handler runs.
- Reference: `CreateIngredientRequest.cs`

### `Models/UpdateIngredientResponse.cs`

```csharp
public sealed record UpdateIngredientResponse(Guid Id);
```

- Returns the updated ingredient's ID, consistent with `CreateIngredientResponse`

### `Models/Mappings.cs`

```csharp
internal static class Mappings
{
    internal static UpdateIngredientResponse ToResponse(this Ingredient ingredient)
        => new(ingredient.Id.Value);
}
```

- Reference: `CreateIngredient/Models/Mappings.cs`

---

## 7) Create IRequestHandler implementation

### `UpdateIngredientHandler.cs`

```csharp
public sealed class UpdateIngredientHandler(IMenuDbContext dbContext)
    : IRequestHandler<UpdateIngredientRequest, UpdateIngredientResponse>
```

In `Handle(UpdateIngredientRequest request, Guid ingredientId, CancellationToken cancellationToken)`:

> Note: the handler receives the `ingredientId` from the route (passed by the endpoint after combining route param with body request). The handler method signature follows the standard `IRequestHandler<TRequest, TResponse>` contract. The endpoint calls `handler.Handle(request, cancellationToken)` after injecting the id through additional context — see endpoint section for the exact flow.

In `Handle(UpdateIngredientRequest request, CancellationToken cancellationToken)`:

1. **Load ingredient** with categories:
   ```csharp
   Ingredient? ingredient = await dbContext.Ingredients
       .Include(i => i.Categories)
       .GetBySelectorAsync((IngredientId)request.Id, cancellationToken);
   ```
2. **Existence check** — if `ingredient is null`, return `Results.Problem(...)` with status `400`
3. **Load categories**:
   ```csharp
   List<Category> categories = await dbContext.Categories
       .Where(c => request.CategoryIds.Contains((Guid)c.Id))
       .ToListAsync(cancellationToken);
   ```
4. **Call domain method**: `ingredient.Update(request.Name, request.Description, request.Price, new Collection<Category>(categories))`
5. **Save**: `await dbContext.SaveChangesAsync(cancellationToken)`
6. **Return**: `Results.Ok(ingredient.ToResponse())`

> Name uniqueness is enforced by `UpdateIngredientValidator` before the handler runs — no uniqueness check needed here.

> **Handler signature adjustment**: Because `IRequestHandler<TRequest, TResponse>.Handle` takes only `(TRequest, CancellationToken)`, the route `id` is passed to the handler via a different mechanism. Two options:
> - **Option A (preferred)**: Extend `UpdateIngredientRequest` to include a settable `Guid Id` property that the endpoint sets before calling `handler.Handle`. The validator ignores `Id` (no `RuleFor(x => x.Id)` rule) — it is set after the filter runs.
> - **Option B**: Perform all checks inside the endpoint delegate itself, passing only the business call to the handler.
>
> Use **Option A**: add `public Guid Id { get; init; }` to `UpdateIngredientRequest` and update the endpoint to pass `request = request with { Id = id }` before calling the handler. The validator does not reference `Id` so it remains validated as `Guid.Empty` during filter execution without issues.

Reference for Option A flow:
- `UpdateIngredientRequest` gains `public Guid Id { get; init; }` (set from route, not from body JSON)
- `UpdateIngredientValidator` rules reference only `Name`, `Description`, `Price`, `CategoryIds`
- Endpoint: `request = request with { Id = id }` then `handler.Handle(request, ct)`
- Handler uses `request.Id` for the ingredient lookup

---

## 8) Create IEndpoint implementation

### `UpdateIngredientEndpoint.cs`

```csharp
public sealed class UpdateIngredientEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedPutWithResponse<UpdateIngredientRequest, UpdateIngredientResponse>(
                "ingredients/{id}", HandleAsync)
            .WithName("UpdateIngredientEndpoint")
            .WithDescription("Updates the name, description, price, and categories of an existing ingredient.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateIngredientRequest request,
        [FromServices] IRequestHandler<UpdateIngredientRequest, UpdateIngredientResponse> handler,
        CancellationToken cancellationToken)
    {
        request = request with { Id = id };
        UpdateIngredientResponse response = await handler.Handle(request, cancellationToken);
        return Results.Ok(response);
    }
}
```

- `[FromBody] UpdateIngredientRequest request` is the argument the `ValidationFilter<UpdateIngredientRequest>` finds (validating body fields before `HandleAsync` executes)
- `request = request with { Id = id }` merges the route param into the request record before passing it to the handler (after validation has already run)
- Reference: `CreateIngredientEndpoint.cs` (POST pattern), `DeleteIngredientEndpoint.cs` (route id pattern), `UpdateCategoriesOrderEndpoint.cs` (PUT pattern)

---

## 9) Create validation policy

### `Policies/UpdateIngredientValidator.cs`

```csharp
public sealed class UpdateIngredientValidator : AbstractValidator<UpdateIngredientRequest>
{
    public UpdateIngredientValidator(IMenuDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        RuleFor(x => x.Name)
            .SetValidator(new IngredientNameValidator());

        RuleFor(x => x.Description)
            .SetValidator(new IngredientDescriptionValidator());

        RuleFor(x => x.Price)
            .SetValidator(new IngredientPriceValidator());

        RuleFor(x => x.Name)
            .MustAsync(async (name, ct) =>
            {
                Guid id = (Guid)httpContextAccessor.HttpContext!.GetRouteValue("id")!;
                return await dbContext.Ingredients
                    .IsIngredientNameUniqueExcludingAsync(name, (IngredientId)id, ct);             
            })
            .WithMessage("Ingredient with this name already exists.");

        RuleFor(x => x.CategoryIds)
            .NotEmpty()
            .WithMessage("At least one category must be selected.");
    }
}
```

- Injects `IMenuDbContext` and `IHttpContextAccessor` (same DI pattern as `CreateCategoryValidator` / `CreateIngredientValidator`)
- `IHttpContextAccessor` is used to read the `id` route value because the `ValidationFilter` runs **before** `HandleAsync` executes — at that point `request.Id` is still `Guid.Empty` (the merge `request = request with { Id = id }` happens inside the handler, after validation)
- `MustAsync` calls `IsIngredientNameUniqueExcludingAsync` so a self-rename (same name, same ingredient) passes correctly
- `Id` is intentionally excluded from `RuleFor` — it is route-bound, not body-validated
- **Existence check** remains in the handler (see section 7)
- Reference: `CreateIngredientValidator.cs`, `CreateCategoryValidator.cs`

---

## 10) Create unit tests

### File: `MyHomeRamen.UnitTests/MenuModule/Ingredients/IngredientUpdateTests.cs`

- `public sealed class IngredientUpdateTests`
- Private helper method to create a valid seeded ingredient then call `Update`:
  ```csharp
  private static void UpdateIngredient(
      Ingredient ingredient,
      string? name = null,
      string? description = null,
      decimal? price = null,
      Collection<Category>? categories = null)
  ```
  - Uses valid defaults from `IngredientConstants` for any `null` argument
  - Calls `ingredient.Update(name, description, price, categories)` directly

| # | Test method | Scenario |
|---|---|---|
| 1 | `Update_Should_UpdateProperties_When_InputIsValid` | Valid name/description/price/categories → no exception; properties reflect new values |
| 2 | `Update_Should_ThrowDomainException_When_NameIsTooShort` | `name: string.Empty` → `DomainException` with `IngredientErrors.NameTooShort().Message` |
| 3 | `Update_Should_ThrowDomainException_When_NameIsTooLong` | `name` exceeding `IngredientConstants.NameMaxLength` → `DomainException` with `IngredientErrors.NameTooLong().Message` |
| 4 | `Update_Should_ThrowDomainException_When_DescriptionIsTooLong` | `description` exceeding `IngredientConstants.DescriptionMaxLength` → `DomainException` with matching error |
| 5 | `Update_Should_ThrowDomainException_When_PriceIsBelowMinimum` | `price: -1m` → `DomainException` with `IngredientErrors.PriceTooLow().Message` |
| 6 | `Update_Should_ThrowDomainException_When_PriceIsAboveMaximum` | `price` exceeding `IngredientConstants.MaxPrice` → `DomainException` with matching error |
| 7 | `Update_Should_ThrowDomainException_When_CategoriesAreEmpty` | `categories: new Collection<Category>()` → `DomainException` |
| 8 | `Update_Should_ThrowDomainException_When_CategoriesContainWrongType` | Category with `CategoryType != CategoryType.Ingredient` → `DomainException` |

- Reference: `MyHomeRamen.UnitTests/MenuModule/Ingredients/IngredientValidationTests.cs` — mirror the `Create_Should_*` test pattern exactly

---

## 11) Create integration tests

### File: `MyHomeRamen.IntegrationTests/MenuModule/Ingredients/UpdateIngredientTests.cs`

- `public sealed class UpdateIngredientTests(WebApiFactory apiFactory)`

| # | Test method | Expected result |
|---|---|---|
| 1 | `UpdateIngredient_ShouldReturnOk_ForValidRequest` | Auth as Admin; seed ingredient; `PUT /api/menu/ingredients/{id}`; assert `200 OK`; verify updated fields in DB |
| 2 | `UpdateIngredient_ShouldReturnUnauthorized_ForUnauthenticatedUser` | No auth header; assert `401 Unauthorized` |
| 3 | `UpdateIngredient_ShouldReturnForbidden_ForNonAdminRole` | `[Theory] [InlineData(UserRoles.Employee)] [InlineData(UserRoles.Customer)]`; assert `403 Forbidden` |
| 4 | `UpdateIngredient_ShouldReturnBadRequest_ForNonExistentId` | Auth as Admin; use `Guid.NewGuid()` as id; assert `400 Bad Request` |
| 5 | `UpdateIngredient_ShouldReturnBadRequest_ForInvalidRequest` | `[MemberData]` over `DataGenerator.InvalidUpdateIngredientRequests()`; covers: empty name, name too short, name too long, description too long, price below min, price above max, empty categoryIds; assert `400 Bad Request` |
| 6 | `UpdateIngredient_ShouldReturnBadRequest_WhenNameAlreadyTakenByDifferentIngredient` | Auth as Admin; seed two ingredients; try to update ingredient A with ingredient B's name; assert `400 Bad Request` |
| 7 | `UpdateIngredient_ShouldReturnOk_WhenNameIsUnchanged` | Auth as Admin; seed ingredient; PUT with the same name; assert `200 OK` (name-uniqueness check correctly excludes self) |

### Also modify: `MyHomeRamen.IntegrationTests/MenuModule/Common/Data/Mappings.cs` (MODIFIED)

Add:
```csharp
internal static UpdateIngredientRequest ToUpdateIngredientRequest(this Ingredient ingredient)
    => new(ingredient.Name, ingredient.Description, ingredient.Price,
           ingredient.Categories.Select(c => (Guid)c.Id));
```

### Also modify: `MyHomeRamen.IntegrationTests/MenuModule/Common/Data/DataGenerator.cs` (MODIFIED)

Add `InvalidUpdateIngredientRequests()` static method:
```csharp
public static TheoryData<UpdateIngredientRequest> InvalidUpdateIngredientRequests()
{
    string validName = new Faker().Random.String2(IngredientNameValidator.MinLength, IngredientNameValidator.MaxLength);
    string validDescription = new Faker().Random.String2(1, IngredientDescriptionValidator.MaxLength);
    decimal validPrice = new Faker().Finance.Amount(IngredientPriceValidator.MinPrice, IngredientPriceValidator.MaxPrice);
    IEnumerable<Guid> validCategoryIds = [Guid.NewGuid()];

    return new TheoryData<UpdateIngredientRequest>
    {
        new(string.Empty,           validDescription, validPrice, validCategoryIds),  // name: empty
        new(tooShortName,           validDescription, validPrice, validCategoryIds),  // name: too short
        new(tooLongName,            validDescription, validPrice, validCategoryIds),  // name: too long
        new(validName,              tooLongDescription, validPrice, validCategoryIds), // description: too long
        new(validName,              validDescription, belowMinPrice, validCategoryIds), // price: below min
        new(validName,              validDescription, aboveMaxPrice, validCategoryIds), // price: above max
        new(validName,              validDescription, validPrice, []),                 // categoryIds: empty
    };
}
```

- Boundary values reference `IngredientNameValidator.MinLength`, `IngredientNameValidator.MaxLength`, `IngredientDescriptionValidator.MaxLength`, `IngredientPriceValidator.MinPrice`, `IngredientPriceValidator.MaxPrice` so tests stay in sync when constants change
- Reference: `DataGenerator.InvalidCreateIngredientRequests()` (if present) or `DataGenerator.InvalidCreateCategoryRequests()` pattern
- Reference: `MyHomeRamen.IntegrationTests/MenuModule/Categories/UpdateCategoriesOrderTests.cs` for `CreatePutMessage + WithJsonContent + AddAuthorizationHeader` test setup pattern

---

## 12) Architecture tests

No new architecture test rules needed — `UpdateIngredient` follows the existing REPR + CQRS pattern already enforced by existing tests.
