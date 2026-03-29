# Feature Implementation Plan — Backend

- **Date**: 2025-07-14
- **Feature**: CreateIngredient
- **Module**: Menu
- **Reference**: CreateCategory, CreateProduct

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/Menu/Features/Ingredients/
├── CreateIngredient/
│   ├── Models/
│   │   ├── CreateIngredientRequest.cs
│   │   ├── CreateIngredientResponse.cs
│   │   └── Mappings.cs
│   ├── Policies/
│   │   └── CreateIngredientValidator.cs
│   ├── CreateIngredientEndpoint.cs
│   └── CreateIngredientHandler.cs
```

---

## 2) Create primitive rules and contracts

Primitive validators already exist in `MyHomeRamen.Common.Contracts/Menu/Ingredients/`:
- `IngredientNameValidator.cs` — MinLength=10, MaxLength=50
- `IngredientDescriptionValidator.cs` — MinLength=5, MaxLength=200
- `IngredientPriceValidator.cs` — MinPrice=0.0m, MaxPrice=50.0m

**No new contracts needed.** Existing validators will be reused in the API `CreateIngredientValidator` and Blazor `IngredientValidator`.

---

## 3) Create models, DTOs and mappings

### `Models/CreateIngredientRequest.cs`
```csharp
public sealed record CreateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds) : IRequest<Guid>;
```
- References: `IRequest<Guid>` from `MyHomeRamen.Api.Common.Endpoint.Models`
- `CategoryIds` — list of category IDs of type `CategoryType.Ingredient`

### `Models/CreateIngredientResponse.cs`
```csharp
public sealed record CreateIngredientResponse(Guid Id);
```

### `Models/Mappings.cs`
```csharp
internal static class Mappings
{
    public static Ingredient ToDomain(this CreateIngredientRequest request, IEnumerable<Category> categories)
    {
        return Ingredient.Create(
            Guid.NewGuid(),
            request.Name,
            request.Description,
            request.Price,
            new Collection<Category>(categories.ToList()));
    }
}
```
- References: `Ingredient.Create(...)` factory from `MyHomeRamen.Domain.Menu.Ingredients`
- Categories are fetched by handler and passed to mapping

---

## 4) Create Persistence DB Extension

### `DbExtensions.cs` — add `IsIngredientNameUniqueAsync`
Add to `MyHomeRamen.Persistance/Common/DbExtensions.cs`:
```csharp
public static async Task<bool> IsIngredientNameUniqueAsync(
    this IQueryable<Ingredient> query,
    string name,
    CancellationToken cancellationToken = default)
{
    return !await query.AnyAsync(i => i.Name.ToLower() == name.ToLower(), cancellationToken);
}
```
- Follows existing pattern of `IsCategoryNameUniqueAsync` and `IsNameUniqueAsync` (Product)

---

## 5) Create `CreateIngredientValidator` (Validation Policy)

### `Policies/CreateIngredientValidator.cs`
```csharp
public sealed class CreateIngredientValidator : AbstractValidator<CreateIngredientRequest>
{
    public CreateIngredientValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Name)
            .SetValidator(new IngredientNameValidator());

        RuleFor(x => x.Description)
            .SetValidator(new IngredientDescriptionValidator());

        RuleFor(x => x.Price)
            .SetValidator(new IngredientPriceValidator());

        RuleFor(x => x.Name)
            .MustAsync(async (name, ct) => await dbContext.Ingredients.IsIngredientNameUniqueAsync(name, ct))
            .WithMessage("Ingredient with this name already exists.");

        RuleFor(x => x.CategoryIds)
            .NotEmpty()
            .WithMessage("At least one category must be selected.");
    }
}
```
- Reuses primitive validators from `MyHomeRamen.Common.Contracts`
- Uses `IsIngredientNameUniqueAsync` DB extension for uniqueness check
- Validates `CategoryIds` is not empty

---

## 6) Create `CreateIngredientHandler` (IRequestHandler)

### `CreateIngredientHandler.cs`
```csharp
public sealed class CreateIngredientHandler(IMenuDbContext dbContext) : IRequestHandler<CreateIngredientRequest, Guid>
{
    public async Task<Guid> Handle(CreateIngredientRequest request, CancellationToken cancellationToken)
    {
        IEnumerable<Category> categories = await dbContext.Categories
            .GetByIds(request.CategoryIds.Select(id => (CategoryId)id), cancellationToken);

        Ingredient ingredient = request.ToDomain(categories);

        dbContext.Ingredients.Add(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ingredient.Id.Value;
    }
}
```
- Fetches categories by IDs using existing `GetByIds` extension
- Maps request to domain using `Mappings.ToDomain()`
- Returns created ingredient ID (no re-query)

---

## 7) Create `CreateIngredientEndpoint` (IEndpoint)

### `CreateIngredientEndpoint.cs`
```csharp
public sealed class CreateIngredientEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedPost<CreateIngredientRequest, CreateIngredientResponse>("ingredients", HandleAsync)
                       .WithName("CreateIngredientEndpoint")
                       .WithDescription("Handles Create Ingredient operations.")
                       .RequireAuthorization(AuthorizationConfiguration.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateIngredientRequest request,
        [FromServices] IRequestHandler<CreateIngredientRequest, Guid> handler,
        CancellationToken cancellationToken)
    {
        Guid id = await handler.Handle(request, cancellationToken);
        CreateIngredientResponse response = new(id);

        return Results.Created($"/api/menu/ingredients/{id}", response);
    }
}
```
- Route: `POST /api/menu/ingredients` (GroupName "Menu" + segment "ingredients")
- Authorization: `RestaurantManagerPolicy`
- Returns `201 Created` with location header

---

## 8) Unit tests

**No unit tests required** as per feature brief. Existing domain validators (`IngredientValidator`) and contract validators (`IngredientNameValidator`, etc.) already have tests in `MyHomeRamen.UnitTests/MenuModule/Ingredients/`.

---

## 9) Integration tests

### File: `MyHomeRamen.IntegrationTests/MenuModule/CreateIngredientTests.cs`

Reference: `CreateCategoryTests.cs`

#### Test data setup

**`DataGenerator.cs`** — add:
- `public static TheoryData<CreateIngredientRequest> InvalidCreateIngredientRequests()` — theory data covering:
  - Name: empty
  - Name: too short (< `IngredientNameValidator.MinLength`)
  - Name: too long (> `IngredientNameValidator.MaxLength`)
  - Description: empty
  - Description: too short (< `IngredientDescriptionValidator.MinLength`)
  - Description: too long (> `IngredientDescriptionValidator.MaxLength`)
  - Price: negative (below `IngredientPriceValidator.MinPrice`, e.g. `-1`)
  - Price: too high (> `IngredientPriceValidator.MaxPrice`)
  - CategoryIds: empty

**`Mappings.cs`** — add:
```csharp
internal static CreateIngredientRequest ToCreateIngredientRequest(this Ingredient ingredient) =>
    new(
        ingredient.Name,
        ingredient.Description,
        ingredient.Price,
        ingredient.Categories.Select(c => (Guid)c.Id)
    );
```

#### Test cases

| Test method | Expected status | Description |
|---|---|---|
| `CreateIngredient_ShouldReturnCreated_ForValidRequest` | 201 Created | Valid request with name, description, price, ingredient-category IDs. Verify Location header is present. |
| `CreateIngredient_ShouldReturnUnauthorized_ForNotAuthenticatedUser` | 401 Unauthorized | No auth header |
| `CreateIngredient_ShouldReturnForbidden_ForNonAdminUser` | 403 Forbidden | Theory with `[InlineData]`: Employee, Customer roles |
| `CreateIngredient_ShouldReturnBadRequest_ForInvalidRequest` | 400 Bad Request | MemberData from `InvalidCreateIngredientRequests` |
| `CreateIngredient_ShouldReturnBadRequest_ForDuplicateName` | 400 Bad Request | Use existing seeded ingredient name |

All tests use:
- Route: `POST /api/menu/ingredients`
- `HttpClientExtensions.CreatePostMessage(...)` pattern
- `AddAuthorizationHeader(UserRoles.Admin)` for valid auth
- `WebApiFactory` injected via primary constructor

---

## 10) Architecture tests

**No architecture tests required** as per feature brief.

---

## 11) System tests

**No system tests required** as per feature brief.
