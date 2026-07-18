# Integration tests refactor plan

## Goal
Refactor integration tests to remove shared-state coupling, eliminate test-order dependency, and standardize data generation and fixtures across module-specific test projects.

## Scope
- Refactor module-level integration test projects under:
  - `MyHomeRamen.MenuApi.IntegrationTests`
  - `MyHomeRamen.ShoppingCartApi.IntegrationTests`
- Keep `MyHomeRamen.IdentityApi.IntegrationTests` out of scope; it is already isolated and uses the new approach.
- Keep `MyHomeRamen.IntegrationTests` as shared infrastructure only, with helpers/extensions and no direct test cases.

## Working assumptions from prompt
- `MyHomeRamen.IntegrationTests` already contains shared helpers and extension methods only.
- Menu API tests already follow the new pattern for categories; ingredients and products still need data-generation refactor.
- Shopping cart tests still need the new fixture/data-generation/test isolation infrastructure implemented.

## Verification
- Do not run build or tests during whole refactor, once all work is done let the uses verify build and test results.

## Phases

### 1. Baseline and inventory
- Review current test projects and identify remaining old-style patterns:
  - `AssemblyFixture` usage
  - container setup inside `WebApiFactory`
  - data generators with special-case test-specific IDs or names
  - tests that depend on shared state or ordering
- Confirm target file layout for shared fixtures and module-specific test classes.

### 2. Establish shared fixture model
- Introduce/standardize shared fixture types for test infrastructure:
  - `DbContainerFixture`
  - `RedisFixture`
- Move container lifecycle management out of `WebApiFactory` and into the fixture layer.
- Remove Assembly fixture from `WebApiFactory`
- Ensure `WebApiFactory` is created from the fixture-managed infrastructure and remains focused on application wiring.
- Keep shared authentication and extension helpers in `MyHomeRamen.IntegrationTests`.

### 3. Refactor Menu API tests
- Finish the migration for ingredients and products to the new pattern.
- Replace old data-generation patterns with generic data generation based on `DataGenerator` that utilize Bogus with dedicated `DataSet`.
- Remove hard-coded test-case-specific data semantics such as names containing `for delete` or `for update`.
- Ensure each test case prepares data with `IAsyncLifetime` or equivalent per-test setup and does not rely on shared state.
- Preserve existing behavior while making tests deterministic and isolated.

### 4. Refactor Shopping Cart tests
- Implement missing infrastructure for the shopping cart project:
  - `DataSet`
  - `DataSeeder`
  - `DataGenerator`
  - mappings/helpers
  - fixture wiring for containers
  - `WebApiFactory` for the shopping cart module
- Add mocks for external dependencies using `NSubstitute` for services such as `IMenuService` and `IPaymentService`.
- Refactor tests to the new naming/structure and remove old coupling to module-specific data generators.
- Clean up namespaces and ensure tests are isolated.

### 5. Validate and clean up
- Remove obsolete helper code and old test-specific data generation paths.
- Verify test order no longer matters and each test can run independently.

## 6. User checkpoints
- This is huge refactor se we need to have checkpoints where user can verify current state and adjust things if needed, checkpoints are:
  - Phase 1 - Menu Module - Shared configuration validation, plan for changes (if any required)
  - Phase 2.1 - MenuApi integration tests - Ingredients tests refactor plan
  - Phase 2.2 - MenuApi integration tests - Ingredients tests refactor plan
  - Phase 3 - ShoppingCartApi integration tests - Shared configuration validation, plan for changes (if any required)
  - Phase 4 - ShoppingCartApi integration tests - ShoppingCart tests refactor plan
  
Example workflow:
- Agent: Verifies current state with notification if any changes are required or no
- User: Approves or requests adjustment to the plan
- Agent: If changes for these step are required - implement and notify user to review implementation before moving further

## Acceptance criteria
- `MyHomeRamen.IntegrationTests` contains only shared helpers/extensions and no tests.
- Test containers are owned by dedicated fixture classes rather than `WebApiFactory`.
- Module tests use generic data generation and per-test setup rather than special-case generator logic.
- Menu API ingredients/products and shopping cart tests pass independently without ordering dependencies.
- Direct DbContext access only to seed data for test case (ideally only within IAsyncLifeTime.StartAsync method) / read-queries for assertion

## Suggested execution order
1. Shared fixtures and factory wiring
2. Menu API ingredients/products refactor
3. Shopping cart infrastructure and tests
4. Validation and cleanup
