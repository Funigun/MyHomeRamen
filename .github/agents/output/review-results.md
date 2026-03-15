- **Date**: 2026-03-14 00:15:00
- **Feature**: CreateProduct (Menu Module)

### Critical Issues

- **Title**: 1) [MyHomeRamen.IntegrationTests\MenuModule\CreateProductTests.cs : 39] - Test assertion contradicts test name
- **Severity level**: Critical
- **Description**: The test method `CreateProduct_ValidRequest_ReturnsCreated` explicitly suggests verifying a successful creation scenario (HTTP 201 Created). However, the assertion checks for `HttpStatusCode.Unauthorized` (HTTP 401). This is a logic error in the test suite that obscures the actual validation of the feature's core capability. Tests must explicitly validate the intended positive behavior without circumventing security bypass incorrectly.
- **Solution proposal**: Ensure the integration test sends a properly authenticated request with an Admin role test token (e.g. configuring `TestAuthHandler` in `WebApplicationFactory`). Then, change the assertion to: `Assert.Equal(HttpStatusCode.Created, response.StatusCode);` along with validating the `Location` header.

### Warnings

- **Title**: 2) [MyHomeRamen.Api\Menu\Features\Products\CreateProduct\Policies\CreateProductValidator.cs : 44] - Raw LINQ query used for unique validation in validator
- **Severity level**: Warning
- **Description**: The uniqueness check for `Name` is currently using a raw `.AnyAsync()` query against the `DbContext` directly inside the validator. According to the architecture guidelines for the persistence layer, complex validation rules querying the database should be extracted into extension methods extending `IQueryable<T>` or `DbSet<T>` inside the `MyHomeRamen.Persistance.Common.DbExtensions` class.
- **Solution proposal**: Create an extension method `public static Task<bool> IsNameUniqueAsync(this IQueryable<Product> query, string name, CancellationToken cancellationToken = default)` inside `MyHomeRamen.Persistance.Common.DbExtensions`. Update `BeUniqueNameAsync` to use this new extension method.

- **Title**: 3) [MyHomeRamen.Api\Menu\Features\Products\CreateProduct\Policies\CreateProductValidator.cs : 19] - Primitive validation logic duplicated rather than reused
- **Severity level**: Warning
- **Description**: The validation policy repeats basic primitive constraints for the `Name` property (`.NotEmpty().MaximumLength(200)`). According to the API guidelines, validations based on primitive types should reside in `MyHomeRamen.Common.Contracts`. There is already a placeholder class `ProductNameValidator` in the `Common.Contracts` project intended for this.
- **Solution proposal**: Implement the `ProductNameValidator` in `MyHomeRamen.Common.Contracts.Menu.Products`, then use `.SetValidator(new ProductNameValidator())` for the `Name` property in `CreateProductValidator`.

### Informational

- **Title**: 4) [MyHomeRamen.Api\Menu\Features\Products\CreateProduct\CreateProductEndpoint.cs : 26] - Method can be made static
- **Severity level**: Information
- **Description**: The `HandleAsync` delegate method in the endpoint implementation is defined as an instance method but does not use any instance state. The SonarAnalyzer rule (S2325) correctly suggests making it `static` to improve performance.
- **Solution proposal**: Add the `static` modifier to the `HandleAsync` method signature.
