---
description: Guidelines for writing Integration Tests using Testcontainers and WebApplicationFactory
applyTo: '**/MyHomeRamen.IntegrationTests/**/*.cs'
---

# Integration Tests Instructions

## Overview
Integration Tests (`MyHomeRamen.IntegrationTests`) focus on bounded component testing, vertical slices (API → Domain → DB) in isolation using `WebApplicationFactory` and Testcontainers. They provide faster execution speed than full system tests while maintaining realism for persistence dependencies.

## Guidelines
- Spin up Testcontainers for persistence (DB, Cache) once per test assembly run via `WebApiFactory`.
- Use mocked/stubbed external boundaries (e.g., mock RabbitMQ publishers or Keycloak APIs) to avoid massive testing configurations.
- Reset database state between tests (using tools like Respawn or EF Core transaction rollbacks) to ensure test isolation.
- Focus on testing vertical slices within a single module.
- Tests must follow the Arrange-Act-Assert pattern and be self-contained so they can run independently of each other.
- Tests must follow the naming convention `MethodName_ShouldBehavior_ForCondition` (e.g., `CreateProduct_ShouldReturnCreated_ForValidRequest`).

## Infrastructure Setup

### WebApiFactory
- `WebApiFactory` extends `WebApplicationFactory<IApiAssemblyMarker>` and implements `IAsyncLifetime`.
- Registered as an assembly-wide fixture via `[assembly: AssemblyFixture(typeof(WebApiFactory))]`.
- Starts MsSql and Redis Testcontainers in `InitializeAsync`, applies DB migrations, seeds initial data, and creates the `HttpClient`.
- `ConfigureWebHost` reconfigures database connection strings and Redis to point at the running containers.
- JWT authentication is reconfigured via `ReconfigureTokenOptions` so tests can issue valid tokens without a real Keycloak instance.

### Test class structure
- Inject `WebApiFactory` via primary constructor — do not use field injection or base classes.
- Mark test classes `public sealed`.

```csharp
public sealed class CreateProductTests(WebApiFactory apiFactory)
{
    [Fact]
    public async Task CreateProduct_ShouldReturnCreated_ForValidRequest() { ... }
}
```

## Test Data Layer
Each module owns a `Common/Data/` folder with three responsibilities:

### DataGenerator
- Static class using **Bogus** `Faker<T>` with `CustomInstantiator` to build valid domain entities.
- IDs are always pre-generated client-side (`Guid.NewGuid()`) — never rely on DB-assigned IDs.
- Tracks generated entities via `internal static IEnumerable<T>` properties (e.g., `GeneratedCategoryIds`) so later generators and tests can reference them.
- Provides `GetRandom*()` helpers (e.g., `GetRandomIngredient()`, `GetRandomProductCategory()`) that pick from the tracked collections using `RandomNumberGenerator.GetInt32`.
- Provides `public static TheoryData<TRequest>` methods for `[MemberData]` theory data — **never use `IEnumerable<object[]>`**.
- Boundary values in invalid-data methods must reference the shared validator constants directly (e.g., `ProductNameValidator.MinLength`) so test cases stay in sync if limits change.

```csharp
// Valid entity generation
internal static Product GenerateValidProduct() => ValidProductFaker.Generate();

// Strongly-typed theory data for [MemberData]
public static TheoryData<CreateProductRequest> InvalidCreateProductRequests() => new()
{
    new CreateProductRequest(string.Empty, validDescription, ...),   // Name: empty
    new CreateProductRequest(tooShortName,  validDescription, ...),  // Name: too short
    ...
};
```

### DataSeeder
- Static class with one `internal static async Task Seed{Module}(IDb context)` method per module.
- Seeds entities in dependency order: e.g., Categories → Ingredients → Products.
- Called once from `WebApiFactory.InitializeAsync`.

```csharp
internal static async Task SeedMenuModule(IMenuDbContext dbContext)
{
    List<Category>   categories  = DataGenerator.GenerateValidCategories(5);
    List<Ingredient> ingredients = DataGenerator.GenerateValidIngredients(10);
    List<Product>    products    = DataGenerator.GenerateValidProducts(20, categories, ingredients);

    dbContext.Categories.AddRange(categories);
    dbContext.Ingredients.AddRange(ingredients);
    dbContext.Products.AddRange(products);

    await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
}
```

### Mappings
- Static class with `internal static` extension methods that map domain entities to API request models.
- Used in tests to build requests from seeded entities without duplicating field mappings.

```csharp
internal static CreateProductRequest ToCreateProductRequest(this Product product) =>
    new(product.Name, product.Description, product.Price,
        product.Categories[0].Id,
        product.BaseIngredients.Select(i => (Guid)i.Id));
```

## xUnit Attribute Guidelines
| Scenario | Attribute |
|---|---|
| Single case | `[Fact]` |
| Simple scalar variants (enums, primitives) | `[Theory] + [InlineData]` |
| Complex object variants (request models) | `[Theory] + [MemberData(nameof(...), MemberType = typeof(DataGenerator))]` |

Always use `TheoryData<T>` (not `IEnumerable<object[]>`) for `[MemberData]` sources to satisfy **xUnit1042** and get compile-time type safety. The `[MemberData]` source method must be `public static`.

## Authorization Testing
Use `HttpClientExtensions.AddAuthorizationHeader(UserRoles role)` to attach a JWT token issued by `JwtTokenFactory`:
- **Admin** — set before requests that should test validation or business logic.
- **Employee / Customer** — use `[InlineData]` to test each forbidden role in one theory.
- **No header** — omit the call entirely to test unauthenticated scenarios.

```csharp
// Admin: test validation, not auth
apiFactory.HttpClient.AddAuthorizationHeader(UserRoles.Admin);

// Multiple forbidden roles via InlineData
[Theory]
[InlineData(UserRoles.Employee)]
[InlineData(UserRoles.Customer)]
public async Task Endpoint_ShouldReturnForbidden_ForNonAdminUser(UserRoles role)
{
    apiFactory.HttpClient.AddAuthorizationHeader(role);
    ...
}
```

## Tools
- `WebApplicationFactory` + `xUnit AssemblyFixture`
- `xUnit` (`[Fact]`, `[Theory]`, `TheoryData<T>`)
- `Testcontainers.MsSql`, `Testcontainers.Redis`
- `Bogus` — domain entity generation
- Custom `JwtTokenFactory` — JWT token generation for auth testing

## Examples
- `CreateProduct_ShouldReturnCreated_ForValidRequest` — generates a valid product via `DataGenerator`, maps it to a request via `Mappings`, authenticates as Admin, posts to the endpoint, asserts 201 and a `Location` header.
- `CreateProduct_ShouldReturnBadRequest_ForInvalidRequest` — feeds all invalid field permutations from `DataGenerator.InvalidCreateProductRequests()` via `[MemberData]`, asserts each returns 400.