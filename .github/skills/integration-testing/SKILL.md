---
name: integration-testing
description: Create, update, and review MyHomeRamen API integration tests using Testcontainers, WebApiFactory fixtures, seeded identities, and permission-based authorization.
---

# MyHomeRamen Integration Testing

Use this skill when creating, updating, or reviewing tests in:

- `MyHomeRamen.IntegrationTests`
- `MyHomeRamen.*Api.IntegrationTests`

## 0. Overall convention
1. Each module has dedicated project that refer to common integration test helpers in `MyHomeRamen.IntegrationTests` project
2. Each project run single TestContainer for DB and Redis
3. Each test class has its own database within shared TestContainer so tests are isolated and can run in parallel

## 1. Discover before editing

1. Identify target API module, aggregate, endpoint, request/response contracts, validator, authorization policy, and persistence context.
2. Inspect the closest existing integration tests in the same module.
3. Inspect the module `WebApiFactory`, database and Redis fixtures, `DataGenerator`, `Mappings`, and shared `IdentityTestData`.
4. Confirm endpoint route, expected status codes, response shape, required permissions, and domain side effects.
5. Check whether test data requires related entities and determine dependency order.

Do not invent test infrastructure when an existing module pattern covers the scenario.

## 2. Test class structure

Use a primary-constructor factory fixture:

```csharp
public sealed class CreateProductTests(WebApiFactory apiFactory) : IClassFixture<WebApiFactory>, IAsyncLifetime
```

Implement `IAsyncLifetime` when setup creates users or database entities:

- Seed test-owned identities and entities in `InitializeAsync`.
- Store generated IDs and identity tuples on the test class.
- Delete test-owned identities and entities in `DisposeAsync`.
- Pass `TestContext.Current.CancellationToken` to every asynchronous database and HTTP operation.

Use unique identity names. Never rely on test execution order or data from another test class.

## 3. Test data

Use module-local `Common/Data` helpers:

- `DataGenerator` creates valid domain entities with Bogus and `CustomInstantiator`.
- Generate IDs client-side with `Guid.NewGuid()` or `Guid.CreateVersion7()`.
- `Mappings` converts domain entities into API request models.
- `TheoryData<T>` supplies public `[MemberData]` sources.
- Invalid requests use shared domain or contract constants for all boundaries.

Seed entities in dependency order. Keep setup data minimal and specific to each test class.

Do not add a generic global seeder contract. Follow the current module factory and data-helper APIs.

## 4. Authorization

Authorization tests use permission-based identities:

1. Seed a user with exact required permissions through `IdentityTestData.SeedUser(...)`, or seed a guest through `SeedGuest(...)`.
2. Use only minimum permissions needed for successful requests.
3. Attach tokens with `HttpClientExtensions.AddAuthorizationHeader(...)`.
4. Use `WithGuestCookie(...)` for guest flows.
5. Omit authorization headers entirely for unauthenticated scenarios.
6. Test forbidden access with identities that lack required permissions. Use `[InlineData]` for equivalent forbidden identities.
7. Delete identities created by tests during cleanup.

Do not fabricate user IDs, reuse production identities, or treat role names as a substitute for required permissions.

## 5. HTTP test pattern

Create and dispose requests explicitly:

```csharp
using HttpRequestMessage request = HttpClientExtensions.CreatePostMessage(endpoint);
request.WithJsonContent(body);
request.AddAuthorizationHeader(user);

HttpResponseMessage response = await apiFactory.HttpClient.SendAsync(
    request,
    TestContext.Current.CancellationToken);

await response.AssertStatusCode(HttpStatusCode.Created);
```

Use `HttpClientExtensions.AssertStatusCode` instead of direct status-code equality. The helper includes response content in failures.

For response assertions:

- Deserialize with `ReadFromJsonAsync<T>` or `ResponseToDto<T>`.
- Assert important response IDs and values.
- Assert `Location` for creation endpoints when provided by the contract.
- Verify persistence or domain effects where endpoint behavior requires it.

## 6. Scenario coverage

Cover scenarios relevant to endpoint behavior:

- Valid authenticated request.
- Valid guest request when supported.
- Invalid request for every meaningful validation boundary.
- Unauthenticated request.
- Authenticated identity without required permission.
- Missing resource.
- Conflicting or duplicate resource.
- Resource owned by a different user.
- Important persistence and response effects.

Combine equivalent invalid or forbidden cases with `Theory` and `MemberData`/`InlineData` according to data complexity.

## 7. Factory and isolation checks

Before changing a factory:

- Preserve unique database naming per fixture.
- Keep migrations applied before tests execute.
- Preserve required SQL Server, Redis, authentication, and test-environment overrides.
- Register module and identity contexts using the existing replacement helpers.
- Keep shared fixture lifetime aligned with `IClassFixture<TFactory>`.

Never add sleeps, fixed ports, shared mutable static state, or order-dependent setup to hide infrastructure timing or isolation problems.

## 8. Validation

Run the smallest existing targeted command for the changed integration-test project or test class. If it fails:

1. Separate compilation failures from container, database, authentication, and assertion failures.
2. Fix test setup or implementation rather than weakening assertions.
3. Re-run the targeted test.
4. Escalate to the broader integration-test project only when shared fixture or infrastructure changes affect multiple tests.

Before finishing, review that:

- Test names describe method, behavior, and condition.
- Test data is isolated and cleaned up.
- Permissions match endpoint policy.
- Async operations use the current xUnit cancellation token.
- Assertions verify more than status code where contract behavior warrants it.
