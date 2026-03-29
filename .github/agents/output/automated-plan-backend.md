Feature implementation plan:
- **Date**: 2025-01-17
- **Feature**: CreateIngredient

---

## 1) Create feature folder and structure

```
MyHomeRamen.Api/
└── Menu/
    └── Features/
        └── Ingredients/
            ├── IngredientsGroup.cs              ← already exists, no changes needed
            ├── CreateIngredient/                 ← already exists, no changes needed
            └── GetCategoriesForDropdown/         ← no changes to this feature; see CreateIngredient plan
```

---

## 2) Create primitive rules and contracts

**No new primitive validators needed.** The only validation required for `CreateIngredient` is string length and format checks for `Name` and `Description`, a range check for `Price`, and a required/non-empty check for `CategoryIds`. These rules are already expressed in existing validators in `MyHomeRamen.Common.Contracts\Menu\Ingredients\` and do not warrant their own reusable `AbstractValidator` in `MyHomeRamen.Common.Contracts`.

No changes to `MyHomeRamen.Common.Contracts` are required for this feature.

---

## 3) Create models, DTOs and mappings

### CreateIngredientRequest
```csharp
// MyHomeRamen.Api/Menu/Features/Ingredients/CreateIngredient/Models/CreateIngredientRequest.cs
using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;

public sealed record CreateIngredientRequest(string Name, string Description, decimal Price, List<Guid> CategoryIds) : IRequest<Guid>;
```

> **Note**: The record carries the following properties:
> - `string Name`: The name of the ingredient.
> - `string Description`: A brief description of the ingredient.
> - `decimal Price`: The price of the ingredient.
> - `List<Guid> CategoryIds`: The IDs of the categories to which the ingredient belongs.

### CreateIngredientResponse
```csharp
// MyHomeRamen.Api/Menu/Features/Ingredients/CreateIngredient/Models/CreateIngredientResponse.cs
namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;

public sealed record CreateIngredientResponse(Guid Id);
```

### Mappings
```csharp
// MyHomeRamen.Api/Menu/Features/Ingredients/CreateIngredient/Models/Mappings.cs
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;

internal static class Mappings
{
    public static Ingredient ToEntity(this CreateIngredientRequest request, List<Category> categories)
    {
        // Find the categories by ID
        List<Category> selectedCategories = categories.Where(c => request.CategoryIds.Contains(c.Id.Value)).ToList();

        // Create and return the Ingredient entity
        return new Ingredient(
            Id: IngredientId.NewIngredientId(),
            Name: request.Name,
            Description: request.Description,
            Price: request.Price,
            CategoryIds: selectedCategories.Select(c => c.Id).ToList());
    }
}
```

---

## 4) Create IRequestHandler implementation

```csharp
// MyHomeRamen.Api/Menu/Features/Ingredients/CreateIngredient/CreateIngredientHandler.cs
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient;

public sealed class CreateIngredientHandler(IMenuDbContext dbContext)
    : IRequestHandler<CreateIngredientRequest, Guid>
{
    public async Task<Guid> Handle(CreateIngredientRequest request, CancellationToken cancellationToken)
    {
        // Fetch the categories by ID
        List<Category> categories = await dbContext.Categories
            .Where(c => request.CategoryIds.Contains(c.Id.Value))
            .ToListAsync(cancellationToken);

        // Create the Ingredient entity
        Ingredient ingredient = request.ToEntity(categories);

        // Add and save the new ingredient to the database
        dbContext.Ingredients.Add(ingredient);
        await dbContext.SaveChangesAsync(cancellationToken);

        // Return the new ingredient's ID
        return ingredient.Id.Value;
    }
}
```

**Key decisions:**
- The handler directly uses the `Mappings.ToEntity()` extension method to convert the request to an `Ingredient` entity.
- Categories are fetched by their IDs from the request to establish the many-to-many relationship between ingredients and categories.
- The new ingredient is added to the `dbContext` and saved, with its ID returned to the caller.

---

## 5) No new IGroupEndpoint needed

`IngredientsGroup` already exists with `GroupName = "Menu"`, `WithTags("Ingredients")`, and `RequireAuthorization()`. The new endpoint reuses this group unchanged.

---

## 6) Create IEndpoint implementation

### Extension addition — MapStandardValidatedPost

Before creating the endpoint, add `MapStandardValidatedPost` to `EndpointBuilderExtensions.cs`:

```csharp
// MyHomeRamen.Api.Common/Endpoint/EndpointBuilderExtensions.cs
// Add after MapStandardAuthenticatedGet:

public static RouteHandlerBuilder MapStandardValidatedPost<TRequest, TResponse>(this IEndpointRouteBuilder builder, string pattern, Delegate handler)
{
    return builder.MapStandardPost<TResponse>(pattern, handler)
                  .WithValidationFilter<TRequest>()
                  .ProducesProblem(StatusCodes.Status400BadRequest);
}
```

> **Note**: `MapStandardPost` already produces 200/201/204/404/500. Adding `.ProducesProblem(400)` is necessary because validation can fail with `400 Bad Request` on this endpoint.

### CreateIngredientEndpoint

```csharp
// MyHomeRamen.Api/Menu/Features/Ingredients/CreateIngredient/CreateIngredientEndpoint.cs
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient;

public sealed class CreateIngredientEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedPost<CreateIngredientRequest, CreateIngredientResponse>(
                "ingredients", HandleAsync)
            .WithName("CreateIngredientEndpoint")
            .WithDescription("Creates a new ingredient and returns its ID.")
            .RequireAuthorization(AuthorizationConfiguration.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        CreateIngredientRequest request,
        [FromServices] IRequestHandler<CreateIngredientRequest, Guid> handler,
        CancellationToken cancellationToken)
    {
        // Handle the request and get the new ingredient ID
        Guid newIngredientId = await handler.Handle(request, cancellationToken);

        // Return 201 Created with the new ingredient ID
        return Results.Created($"/api/menu/ingredients/{newIngredientId}", new CreateIngredientResponse(newIngredientId));
    }
}
```

**Route resolution:**
- GroupName = `"Menu"` → route prefix = `api/menu`
- Pattern = `"ingredients"` → full URL = `api/menu/ingredients` ✓

### CreateIngredientValidator

```csharp
// MyHomeRamen.Api/Menu/Features/Ingredients/CreateIngredient/Policies/CreateIngredientValidator.cs
using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Policies;

public sealed class CreateIngredientValidator : AbstractValidator<CreateIngredientRequest>
{
    public CreateIngredientValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .Length(2, 100).WithMessage("Name must be between 2 and 100 characters.");

        RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.")
            .Length(5, 500).WithMessage("Description must be between 5 and 500 characters.");

        RuleFor(x => x.Price)
            .InclusiveBetween(0.01m, 999.99m).WithMessage("Price must be between $0.01 and $999.99.");

        RuleFor(x => x.CategoryIds)
            .NotEmpty().WithMessage("At least one category is required.")
            .Must(ids => ids.All(id => id != Guid.Empty)).WithMessage("Invalid category ID.");
    }
}
```

> **No async needed** — all checks are synchronous. No DB access required for this validation.
> **No `IMenuDbContext` injection** — primary constructor stays parameter-less, consistent with how `CreateCategoryValidator` handles the pure enum guard separately.

---

## 7) Create unit tests

Unit tests should be **skipped** for this feature:
- No new `AbstractValidator` is added to `MyHomeRamen.Common.Contracts` (no primitive rules to test).
- The validation logic for `CreateIngredient` is covered by existing primitive validators in `MyHomeRamen.Common.Contracts\Menu\Ingredients\`.

---

## 8) Create integration tests

### File location
`MyHomeRamen.IntegrationTests/MenuModule/CreateIngredientTests.cs`

### Prerequisites — updates to existing files

**`DataSeeder.cs`** — no changes required. Existing seeder setup provides a variety of categories to test ingredient creation.

**`DataGenerator.cs`** — no changes required for this feature.

**`DataMappings.cs` (or `Mappings.cs` in Common/Data/)** — no new mapping method is needed because the POST request directly uses request parameters and does not require domain entity mapping.

### Test class structure

```csharp
// MyHomeRamen.IntegrationTests/MenuModule/CreateIngredientTests.cs
using System.Net;
using System.Net.Http.Json;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.IntegrationTests.Common;
using MyHomeRamen.IntegrationTests.Common.Configuration;
using MyHomeRamen.IntegrationTests.MenuModule.Common.Data;

namespace MyHomeRamen.IntegrationTests.MenuModule;

public sealed class CreateIngredientTests(WebApiFactory apiFactory)
{
    // Test 1: Valid request — returns 201 Created and the new ingredient ID
    // Test 2: Invalid request (missing name) — returns 400 Bad Request
    // Test 3: Price out of range — returns 400 Bad Request
    // Test 4: Invalid category ID — returns 400 Bad Request
    // Test 5: Unauthenticated — returns 401
    // Test 6: Forbidden roles — returns 403 (Theory with Employee/Customer)
}
```

### Test cases

**Test 1 — Returns 201 Created with the new ingredient ID for a valid request:**
```csharp
[Fact]
public async Task CreateIngredient_ShouldReturnCreated_ForValidRequest()
{
    // Arrange
    var request = new CreateIngredientRequest("New Ingredient", "Description of new ingredient", 9.99m, new List<Guid> { /* valid category ID */ });
    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreatePostMessage("/api/menu/ingredients", request)
        .AddAuthorizationHeader(UserRoles.Admin);

    // Act
    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);
    var response = await responseMessage.Content.ReadFromJsonAsync<CreateIngredientResponse>(TestContext.Current.CancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.Created, responseMessage.StatusCode);
    Assert.NotNull(response);
    Assert.NotEqual(Guid.Empty, response.Id);
}
```

**Test 2 — Returns 400 Bad Request for an invalid request (missing required fields):**
```csharp
[Fact]
public async Task CreateIngredient_ShouldReturnBadRequest_ForInvalidRequest()
{
    // Arrange
    var request = new CreateIngredientRequest("", "", 0, new List<Guid> { Guid.Empty }); // Invalid data
    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreatePostMessage("/api/menu/ingredients", request)
        .AddAuthorizationHeader(UserRoles.Admin);

    // Act
    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, responseMessage.StatusCode);
}
```

**Test 3 — Returns 400 Bad Request for price out of range:**
```csharp
[Fact]
public async Task CreateIngredient_ShouldReturnBadRequest_ForPriceOutOfRange()
{
    // Arrange
    var request = new CreateIngredientRequest("Test Ingredient", "Valid description", 1000, new List<Guid> { /* valid category ID */ });
    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreatePostMessage("/api/menu/ingredients", request)
        .AddAuthorizationHeader(UserRoles.Admin);

    // Act
    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, responseMessage.StatusCode);
}
```

**Test 4 — Returns 400 Bad Request for invalid category ID:**
```csharp
[Fact]
public async Task CreateIngredient_ShouldReturnBadRequest_ForInvalidCategoryId()
{
    // Arrange
    var request = new CreateIngredientRequest("Test Ingredient", "Valid description", 9.99m, new List<Guid> { Guid.Empty });
    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreatePostMessage("/api/menu/ingredients", request)
        .AddAuthorizationHeader(UserRoles.Admin);

    // Act
    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.BadRequest, responseMessage.StatusCode);
}
```

**Test 5 — Returns 401 for unauthenticated request:**
```csharp
[Fact]
public async Task CreateIngredient_ShouldReturnUnauthorized_ForNotAuthenticatedUser()
{
    // Arrange
    var request = new CreateIngredientRequest("New Ingredient", "Description", 9.99m, new List<Guid> { /* valid category ID */ });
    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreatePostMessage("/api/menu/ingredients", request);

    // Act
    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.Unauthorized, responseMessage.StatusCode);
}
```

**Test 6 — Returns 403 for forbidden roles:**
```csharp
[Theory]
[InlineData(UserRoles.Employee)]
[InlineData(UserRoles.Customer)]
public async Task CreateIngredient_ShouldReturnForbidden_ForNonAdminUser(UserRoles role)
{
    // Arrange
    var request = new CreateIngredientRequest("New Ingredient", "Description", 9.99m, new List<Guid> { /* valid category ID */ });
    using HttpRequestMessage httpRequest = HttpClientExtensions
        .CreatePostMessage("/api/menu/ingredients", request)
        .AddAuthorizationHeader(role);

    // Act
    HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

    // Assert
    Assert.Equal(HttpStatusCode.Forbidden, responseMessage.StatusCode);
}
```

### HttpClientExtensions note
Verify that `HttpClientExtensions.CreatePostMessage<T>(string url, T value)` exists and is accessible — it should match the pattern of other `Create*Message` methods:
```csharp
public static HttpRequestMessage CreatePostMessage<T>(string url, T value)
{
    var request = new HttpRequestMessage(HttpMethod.Post, url)
    {
        Content = JsonContent.Create(value)
    };
    return request;
}
```

---

## 9) Create architecture tests

Architecture tests should be **skipped** — the new feature stays entirely within the `Menu` module. Existing module boundary tests in `ApiBoundariesTests.cs` and `DomainBoundariesTests.cs` already enforce that `MyHomeRamen.Api.Menu` does not depend on other modules. No new rules are needed.

---

## 10) Create system tests

System tests should be **skipped** — `CreateIngredient` is a single-service write operation with no cross-service orchestration. Integration tests provide sufficient coverage.

---

## Implementation Order

1. **`EndpointBuilderExtensions.cs`** — add `MapStandardValidatedPost<TRequest, TResponse>` overload
2. **`CreateIngredientRequest.cs`** — create request model
3. **`CreateIngredientResponse.cs`** — create response model
4. **`Mappings.cs`** — create mapping helper
5. **`CreateIngredientValidator.cs`** — create validator
6. **`CreateIngredientHandler.cs`** — create handler
7. **`CreateIngredientEndpoint.cs`** — create endpoint
8. **`CreateIngredientTests.cs`** — create integration tests
