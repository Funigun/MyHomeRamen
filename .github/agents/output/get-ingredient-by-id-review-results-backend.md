- **Date**: 2025-07-15
- **Feature**: GetIngredientById
- **Critical**: 2
- **Warnings**: 2
- **Information**: 1

---

# Review Report — GetIngredientById (Backend)

## Critical

---

### [1] [`GetIngredientByIdValidator.cs` : 9] — Existence check missing from validator; handler throws 404 instead of 400

**Severity**: Critical

**Description**:
The `GetIngredientByIdValidator` only validates that the `Id` is not empty. It does **not** check whether an ingredient with that ID actually exists in the database. Instead, the existence check is performed in `GetIngredientByIdHandler` by throwing a custom `IngredientNotFoundException` (→ 404 Not Found).

This violates two project standards simultaneously:

1. **Validation pattern** — The project convention (exemplified by `DeleteCategoryValidator`) mandates that all DB-backed existence checks live in the validator, using `IMenuDbContext` and `DbExtensions`. The validator is the single place responsible for "is this request valid?"; the handler must be able to trust the data it receives.
2. **Brief specification** — The brief explicitly describes: *"FluentValidation: ingredient with the given ID must exist"* and lists the corresponding test as `GetIngredientById_ShouldReturnBadRequest_ForNonExistentId | 400 Bad Request (validator fires)`.

**Solution proposal**:

Inject `IMenuDbContext` into the validator (via primary constructor) and add an existence check using `ExistsByIdAsync`, mirroring the `DeleteCategoryValidator` pattern:

```csharp
public sealed class GetIngredientByIdValidator : AbstractValidator<GetIngredientByIdRequest>
{
    public GetIngredientByIdValidator(IMenuDbContext menuDbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ingredient ID must not be empty.")
            .ChildRules(id =>
                id.RuleFor(id => id)
                    .MustAsync(async (id, ct) =>
                        await menuDbContext.Ingredients.ExistsByIdAsync((IngredientId)id, ct))
                    .WithMessage("Ingredient with the specified ID does not exist."));
    }
}
```

With this fix, the handler can use `GetBySelectorNotTrackedAsync` and eliminate the null guard and `IngredientNotFoundException`. See also: issue [5] regarding the unnecessary exception class.

- **Implementation status**: ✅ Fixed in iteration 1 — `GetIngredientByIdValidator.cs` created with `IMenuDbContext` injected and `ExistsByIdAsync` existence check inside `ChildRules`. Handler uses `GetBySelectorNotTrackedAsync` directly with no null guard.

---

### [2] [`GetIngredientByIdTests.cs` : 81] — Test name and expected status code contradict the brief

**Severity**: Critical

**Description**:
The test `GetIngredientById_ShouldReturnNotFound_ForNonExistentIngredient` asserts `HttpStatusCode.NotFound` (404). The brief specifies:

| Test | Expected result |
|---|---|
| `GetIngredientById_ShouldReturnBadRequest_ForNonExistentId` | `400 Bad Request` (validator fires) |

The test name explicitly claims "NotFound" but the brief mandates a 400 response triggered by the validator. This is both a wrong test name and a wrong assertion. It would have caught issue [1] (missing existence check in the validator) if it were correct.

**Solution proposal**:

After fixing issue [1], rename the test and update the assertion:

```csharp
[Fact]
public async Task GetIngredientById_ShouldReturnBadRequest_ForNonExistentId()
{
    // Arrange
    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreateGetMessage($"{EndpointBase}/{Guid.NewGuid()}")
        .AddAuthorizationHeader(UserRoles.Admin);

    // Act
    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, responseMessage.StatusCode);
}
```

- **Implementation status**: ✅ Fixed in iteration 1 — `GetIngredientByIdTests.cs` created with `GetIngredientById_ShouldReturnBadRequest_ForNonExistentId` asserting `HttpStatusCode.BadRequest`.

---

## Warnings

---

### [3] [`GetIngredientByIdResponse.cs` : 5] — Response shape deviates from brief: `IEnumerable<IngredientCategoryDto> Categories` instead of `IEnumerable<Guid> CategoryIds`

**Severity**: Warning

**Description**:
The brief defines the response contract as:
```
GetIngredientByIdResponse(Guid Id, string Name, string Description, decimal Price, IEnumerable<Guid> CategoryIds)
```
The implementation returns:
```csharp
public sealed record GetIngredientByIdResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    IEnumerable<IngredientCategoryDto> Categories);
```

This introduces an extra `IngredientCategoryDto` type (with `Name`) and renames the property to `Categories`. The frontend multi-select only needs the IDs to pre-select values — the category names are already available from the `GetCategoriesByType` call. The deviation also creates a constraint on the Blazor response type: the architecture test `BlazorResponse_ShouldMatch_ApiResponseShape` will enforce that the Blazor `GetIngredientByIdResponse` mirrors this shape, so the mismatch with the brief propagates to the frontend.

**Solution proposal**:

Revert to the brief-specified shape:
```csharp
public sealed record GetIngredientByIdResponse(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);
```

Update `Mappings.cs` accordingly:
```csharp
internal static class Mappings
{
    public static GetIngredientByIdResponse ToResponse(this Ingredient ingredient) =>
        new(ingredient.Id.Value,
            ingredient.Name,
            ingredient.Description,
            ingredient.Price,
            ingredient.Categories.Select(c => c.Id.Value));
}
```

`IngredientCategoryDto` can then be deleted.

- **Implementation status**: ✅ Fixed in iteration 1 — `GetIngredientByIdResponse.cs` created with `IEnumerable<Guid> CategoryIds`. `Mappings.cs` maps `ingredient.Categories.Select(c => c.Id.Value)`. No `IngredientCategoryDto` introduced.

---

### [4] [`GetIngredientByIdTests.cs`] — Missing `GetIngredientById_ResponseShouldContainCategoryIds` integration test

**Severity**: Warning

**Description**:
The brief explicitly lists this test as required:

| Test | Expected result |
|---|---|
| `GetIngredientById_ResponseShouldContainCategoryIds` | Response includes correct `CategoryIds` matching the ingredient's categories |

This test is absent. The `GetIngredientById_ShouldReturnOk_ForAuthenticatedAdmin` test does include a category assertion, but the brief requires a dedicated test for this behaviour to make the intent explicit and ensure the `Include(i => i.Categories)` in the handler is covered by a purpose-named test.

**Solution proposal**:

Add a focused test that seeds an ingredient with known categories and asserts the response `CategoryIds` (or `Categories` if issue [3] is not fixed) are correct:

```csharp
[Fact]
public async Task GetIngredientById_ResponseShouldContainCategoryIds()
{
    // Arrange
    Ingredient ingredient = DataGenerator.GeneratedIngredients.First(i => i.Categories.Count > 0);
    IEnumerable<Guid> expectedIds = ingredient.Categories.Select(c => c.Id.Value);

    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreateGetMessage($"{EndpointBase}/{ingredient.Id.Value}")
        .AddAuthorizationHeader(UserRoles.Admin);

    // Act
    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
    GetIngredientByIdResponse? result = await responseMessage.Content
        .ReadFromJsonAsync<GetIngredientByIdResponse>(TestContext.Current.CancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
    Assert.NotNull(result);
    Assert.Equal(expectedIds.OrderBy(id => id), result.CategoryIds.OrderBy(id => id));
}
```

- **Implementation status**: ✅ Fixed in iteration 1 — `GetIngredientById_ResponseShouldContainCategoryIds` added to `GetIngredientByIdTests.cs` asserting ordered `CategoryIds` match.

---

## Information

---

### [5] [`IngredientNotFoundException.cs` : 1] — Unnecessary new exception class introduced to support handler-based existence check

**Severity**: Information

**Description**:
`IngredientNotFoundException` was created exclusively to support the null check in `GetIngredientByIdHandler`. Once issue [1] is resolved (existence check moved to the validator), this exception class has no use and should be deleted. Keeping it leaves dead code and implies a pattern (exception-based not-found handling) that does not align with the project's validation-first approach.

**Solution proposal**:

After fixing issue [1]:
1. Delete `MyHomeRamen.Api\Menu\Exceptions\IngredientNotFoundException.cs`.
2. Remove the null guard and `throw` from `GetIngredientByIdHandler` — the handler can use `GetBySelectorNotTrackedAsync` (already available via `DbExtensions`) and call `ToResponse()` directly:


- **Implementation status**: ✅ Fixed in iteration 1 — `IngredientNotFoundException.cs` was never created. `GetIngredientByIdHandler.cs` uses `GetBySelectorNotTrackedAsync` with `Include(i => i.Categories)` and calls `ToResponse()` directly.

```csharp
public async Task<GetIngredientByIdResponse> Handle(GetIngredientByIdRequest request, CancellationToken cancellationToken)
{
    IngredientId ingredientId = request.Id;

    Ingredient ingredient = await dbContext.Ingredients
        .AsNoTracking()
        .Include(i => i.Categories)
        .GetBySelectorNotTrackedAsync(ingredientId, cancellationToken);

    return ingredient.ToResponse();
}
```
