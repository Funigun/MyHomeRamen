
Task: Refactor integration tests to elimanate following issues:
      
Disclaimer: `MyHomeRamen.IdentityApi.IntegrationTests` is not part of refactor as it is already isolated and refactored to new approach.

Before refactor:
  - single project `MyHomeRamen.IntegrationTests` with single AssemblyFixture for WebApiFactory containing TestContainers definitions etc.
  - coupled data generators between modules e.g. Shopping cart needed data from Menu module so IMenuService would not fail
  - Order of tests matter
  - Tests are not isolated even within module
  - Calling DbContext directly in tests is valid, but current approach lead to concurrency issues (begin transaction before other completes)
  
  - Data generators were defined to generate not user-friendly data e.g. random string for name
  - Data generators contained all data creation even for calidation paths (finding related logic was hard)
  - Test cases relied on data generators including IDs which forced to generate here date per case e.g. product name containing "for delete" so DeleteProduct test case could filter data "easly"

Target solution:
  - Define custom DataSet for Bogus library for data generation e.g. `MenuDataSet`
  - Move TestContainers to separate `AssemblyFixture` e.g. `DbContainerFixture`, `RedisFixture`
  - Define `WebApiFixture` that injects `WebApiFactory`
  - Remove `AssemblyFixture` and `TestContainers` definitions from `WebApiFactory`
  - inject TestContainers into `WebApiFixture` and use them to create `WebApiFactory`
  - Remove "concrete" data generator logic (e.g. "for delete", "for update") and leave only generic data generation e.g. `DataGenerator.CreateProductCategory` from `MyHomeRamen.MenuApi.IntegrationTests`
  - Generate date per test case using new generic data generation methods and IAsyncLifetime interface to prepare data once per test case
  - `MyHomeRamen.IntegrationTests` will contain only shared extension methods and helpers for integration tests and will not contain any tests

Current state of migration
  - original project `MyHomeRamen.IntegrationTests` is ready - no tests, only shared helpers and extension methods left
  - `MyHomeRamen.MenuApi.IntegrationTests`:
    - already refactored to new approach in terms of data generation and tests isolation
    - Tests for Categories are fully refactored
    - Tests for Ingredients (`MyHomeRamen.MenuApi.IntegrationTests\Ingredients\*`) are only moved to new project, they require data generation refactor
    - Tests for Products (`MyHomeRamen.MenuApi.IntegrationTests\Products\*`) are only moved to new project, they require data generation refactor
  - `MyHomeRamen.ShoppingCart.IntegrationTests`:
    - only test cases moved to new project
    - DataSet, DataSeeder, DataGenerator, Mappings, TestContainers and WebApiFactory to be implemented
    - mock of IMenuService and IPaymentService to be implemented using NSubstitute
    - Test cases need namespaces cleanup and refactor to new approach


