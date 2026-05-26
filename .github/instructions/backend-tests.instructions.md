---
description: 'Instructions for backend projects'
applyTo: '**/MyHomeRamen.UnitTests/**/*.cs,**/MyHomeRamen.IntegrationTests/**/*.cs'
---

# Backend Layer Instructions

## 1) General Guidelines
- Testing framework: XUnit.v3 library
- Mocking framework: NSubstitute
- Assertions: XUnit syntax
- Use AAA pattern
- Use `Theory` and `InlineData` for tests that cover up to 5 scenarios
- Use `Theory` and `MemberData` for complex tests that require multiple parameters or complex objects
- Define test data in separate files
- Always use `TheoryData<T>` (not `IEnumerable<object[]>`) for `public static [MemberData]` sources

## 2) Unit tests (`MyHomeRamen.UnitTests`)

### 2.1) Conventions and strategy
- Domain models creation, validation and behavior.
- Location pattern: `{Module}/{Model}/{Concern}Tests.cs`.
- E.g. concern `ProductValidationTests`, `ProductValidatorTests`, `ProductBehaviorTests`, `ProductEventsTests`, etc.

### 2.2) Domain Creation & Validation
- Use a private helper factory method (e.g., `CreateProduct(...)`) with optional named parameters and sensible valid defaults so each test only sets the value under test.
- Every min/max boundary for string lengths, numeric ranges, and collections.
- Uniqueness constraints within collections (e.g., duplicate ingredients, duplicate categories).
- Type/Enum constraints on related entities (e.g., a `CategoryType.Ingredient` category is invalid for a product).
- Invalid state can cover one possibility per single run and verify results according to pattern:
  - `Assert.Throws<DomainException>(() => ...)` is thrown.
  - The exception message matches the corresponding `{Model}Errors.*().Message` constant.

### 2.3) Domain behavior and events
- Focus on whole domain logic from Aggregate root methods to domain events.

### 2.4) API Validators
API validator tests verify that FluentValidation validators in `MyHomeRamen.Common.Contracts`
Add one test **per exported constant** that matches corresponding `{Model}Constants` value in the Domain layer

Verify FluentValidation messages fragment as per table:
| Rule | Fragment to assert |
|------|--------------------|
| `NotEmpty` | `"not be empty"` |
| `MinimumLength` | `"minimum length"` |
| `MaximumLength` | `"maximum length"` |
| `GreaterThanOrEqualTo` | `"greater than or equal to"` |
| `LessThanOrEqualTo` | `"less than or equal to"` |

## 3) Integration tests (`MyHomeRamen.IntegrationTests`)

### 3.1) Conventions and strategy
- Focus on testing API endpoints and their integration with the database and other services.
- Organize tests by module e.g. `MenuModule`, `OrderModule`.
- Organize module folders by API endpoints e.g. `Products`, `Orders`.
- Organize module folders by features e.g. `CreateProducTests` etc.
- Set `Common` folder for each module to define shared test data generators and seeders.
- Inject `WebApiFactory` via primary constructor — do not use field injection or base classes.

### 3.2) Test data management (`/Common/Data/` folder)

#### 3.2.1) DataGenerator
- Static class using **Bogus** `Faker<T>` with `CustomInstantiator` to build valid domain entities.
- IDs are always pre-generated client-side (`Guid.NewGuid()`) — never rely on DB-assigned IDs.
- Tracks generated entities via `internal static IEnumerable<T>` properties (e.g., `GeneratedCategoryIds`) so later generators and tests can reference them.
- Provides `GetRandom*()` helpers (e.g., `GetRandomIngredient()`, `GetRandomProductCategory()`) that pick from the tracked collections using `RandomNumberGenerator.GetInt32`.
- Provides `public static TheoryData<TRequest>` methods for `[MemberData]` theory data — **never use `IEnumerable<object[]>`**.
- Boundary values in invalid-data methods must reference the shared validator constants directly (e.g., `ProductNameValidator.MinLength`) so test cases stay in sync if limits change.

#### 3.2.2) DataSeeder
- Static class with one `internal static async Task Seed{Module}(IDb context)` method per module.
- Seeds entities in dependency order: e.g., Categories → Ingredients → Products.
- Called once from `WebApiFactory.InitializeAsync`.

#### 3.2.3) Mappings
- Static class with `internal static` extension methods that map domain entities to API request models.
- Used in tests to build requests from seeded entities without duplicating field mappings.

### 3.3) Authorization Testing
Use `HttpClientExtensions.AddAuthorizationHeader(UserRoles role)` to attach a JWT token issued by `JwtTokenFactory`:
- **Admin** — set before requests that should test validation or business logic.
- **Employee / Customer** — use `[InlineData]` to test each forbidden role in one theory.
- **No header** — clear current header and omit the call entirely to test unauthenticated scenarios.

### 3.4) Status code assertions
Always use `await responseMessage.AssertStatusCode(HttpStatusCode.Xxx)` instead of `Assert.Equal(HttpStatusCode.Xxx, responseMessage.StatusCode)` or `Assert.True(responseMessage.StatusCode == ...)`.

The extension method is defined in `HttpClientExtensions` and automatically includes the response body in the failure message, making failures easy to diagnose without any additional setup.
- `CreateProduct_ShouldReturnCreated_ForValidRequest` — generates a valid product via `DataGenerator`, maps it to a request via `Mappings`, authenticates as Admin, posts to the endpoint, asserts 201 via `AssertStatusCode` and checks `Location` header.
- `CreateProduct_ShouldReturnBadRequest_ForInvalidRequest` — feeds all invalid field permutations from `DataGenerator.InvalidCreateProductRequests()` via `[MemberData]`, asserts each returns 400 via `AssertStatusCode`.