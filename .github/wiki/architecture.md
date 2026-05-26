# Architecture

The MyHomeRamen project is structured as a modular monolith, with the potential to transition to microservices in the future if needed. 
The architecture is designed to support multiple domains (Menu, Orders, Basket, Reservations, Payments, Identity) while maintaining high development velocity and operational simplicity.

## Clean + Vertical slice combination

Project structure combines Clean Architecture principles with Vertical Slice Architecture and modular monolith by:
- separating code into distinct modules where each module has its own domain, persistence (separate contexts and db schemas), API folders,
- organizing into layers (Domain, Persistence, Infrastructure, API) to separate concerns and allow code reuse across API, Workers and Blazor
- organizing features by vertical slices within each module 

## Domain structure
```
MyHomeRamen.Domain/                     ← Domain entities, value objects, and services
├── Common/                             ← Cross-module constants and errors (no entity definitions)
│   └── {DomainConcept}/				← e.g. Cross-module shared concepts like product max length
│       ├── {DomainConcept}Constants.cs ← e.g. CategoryConstants (MinNameLength, MaxNameLength)
│       └── {DomainConcept}Errors.cs    ← e.g. CategoryErrors (static DomainException factories)
├── {Module}/                           ← One folder per bounded context / business module
│   ├── Database/
│   │   └── I{Module}DbContext.cs       ← DbSet<T> interface — implemented in Persistence layer
│   ├── Events/                         ← Domain events raised by aggregates in this module
│   │   └── {EntityName}{Action}Event.cs
│   └── {AggregateRoot}/                ← One subfolder per aggregate root
│       ├── {Entity}.cs                 ← Aggregate root: inherits AuditableEntity, implements IEntity<TId>
│       ├── {Entity}Id.cs               ← Strongly-typed ID: readonly record struct with implicit casts
│       ├── {Entity}Validator.cs        ← internal static class — validates entity in Create()
│       └── {Entity}Enum.cs             ← Enums related to this aggregate (e.g. CategoryType, OrderStatus)
└── IDomainAssemblyMarker.cs
```

## Persistence structure
```
MyHomeRamen.Persistance/                ← Database contexts and EF Core configurations
├── Common/
│   └── RepositoryDbExtensions.cs       ← Generic IQueryable<T> extensions (Paged, Exists, GetList, GetById)
└── {Module}/
    ├── Configurations/                 ← IEntityTypeConfiguration implementations
    ├── Converters/                     ← EF Core value converters (e.g. strong-ID converters)
    ├── Extensions/                     ← Entity-specific IQueryable<T> extensions for this module
    │   └── {Entity}DbExtensions.cs     ← e.g. CategoryDbExtensions, ProductDbExtensions
    ├── Migrations/                     ← EF Core migrations for the module
    ├── {Module}DbContext.cs
	└── {Module}DbContextFactory.cs     ← Configuration for design-time DbContext creation (e.g. for CLI migrations)
```

## API structure
```
MyHomeRamen.Api/
└── {Module}/
    ├── Features/
    │   └── {DomainModelPlural}/
    │       ├── {FeatureName}/
    │       │   ├── {FeatureName}Command.cs / {FeatureName}Query.cs 
    │       │   ├── {FeatureName}Endpoint.cs
    │       │   ├-─ {FeatureName}Handler.cs
    │       │   ├─- {FeatureName}ValidationPolicy.cs
    │       │   ├─- {FeatureName}AuthorizationPolicy.cs  ← optional
    │       │   └── Mappings.cs
    │       └── {DomainModel}Group.cs
    ├── Services/						← Shared services for the module
    └── ExternalApis/					← Integration points exposed to other modules
MyHomeRamen.Api.Common/                 ← Common utilities, extensions, and helpers for API
MyHomeRamen.Common.Contracts/           ← Shared validators, messages objects
├── Account/                            ← Reusable FluentValidation extension methods for account fields
│   └── AccountValidationExtensions.cs ← e.g. ValidUserName, ValidName, ValidPassword rule builders
├── Messaging/                          ← Integration event contracts shared between API and workers
│   └── {EventName}IntegrationEvent.cs ← e.g. UserRegisteredIntegrationEvent
├── {Module}/                           ← Module-scoped primitive validators (reused by API + Blazor)
│   └── {DomainModel}/
│       └── {Property}Validator.cs      ← AbstractValidator<string|decimal|int> with exported constants
└── ICommonContractsAssemblyMarker.cs
```

## Blazor structure
```
MyHomeRamen.Blazor/                 ← Server project (prerendering, auth, layout, routing)
├── Presentation/                   ← DI registrations and cross-cutting concerns
│   ├── ApiDependencyInjection.cs   ← Registers all typed HttpClients per module
│   ├── NavigationDependencyInjection.cs ← Registers all {Module}NavigationService instances
│   ├── AuthenticationDependencyInjection.cs
│   └── Authentication/				← JWT handlers, Claim transformer, Custom Auth State Provider
├── Common/                         ← App-wide shared models, services and configuration
│   ├── Models/
│   │   ├── FormMode.cs             ← Enum: Create | View | Edit
│   │   └── BaseValidator.cs        ← AbstractValidator<T> with ValidateValue delegate for MudForm
│   ├── Services/                   ← Shared services (e.g. MessageService wrapping MudBlazor snackbar)
│   └── Configuration/              ← App config wrappers (e.g. ThemeConfiguration, RestaurantConfiguration)
├── Components/                     ← Global Blazor components (App.razor, MainLayout, NavMenu)
├── Features/                       ← Vertical slices organized by module and feature
│   └── {Module}/
│       ├── Common/
│       │   ├── Constants/          ← e.g. MenuRoleConstants.cs
│       │   ├── Models/             ← Shared read models across features (e.g. CategoryOption, IngredientOption)
│       │   └── Services/
│       │       ├── {Module}ApiClient.cs        ← Typed HttpClient: one per module, all API calls here
│       │       └── {Module}NavigationService.cs ← Routes static class + imperative navigation methods
│       └── {DomainModel}/          ← One folder per domain model / feature group
│           ├── Components/         ← Reusable form components shared across feature actions
│           │   ├── {DomainModel}Form.razor     ← Unified Create/View/Edit form using FormMode
│           │   ├── {DomainModel}Model.cs       ← UI model with ToXxxRequest() mapping method
│           │   └── {DomainModel}Validator.cs   ← BaseValidator<TModel> reusing Common.Contracts validators
│           └── {ActionName}/       ← e.g. CreateProduct/, CategoriesIndex/
│               ├── {ActionName}Page.razor      ← Routable page wrapping form/list components
│               ├── {ActionName}Page.razor.cs   ← Code-behind (only if page logic is complex)
└── Program.cs
```

## Tests structure
```
MyHomeRamen.UnitTests/                  ← Unit tests — domain validation + API contract validators
├── {Module}Module/                     ← One folder per business module (e.g. MenuModule, OrdersModule)
│   └── {DomainModel}/                  ← One folder per domain model within the module
│       ├── {DomainModel}ValidationTests.cs  ← Tests for domain factory method (Create_Should_Throw...)
│       └── {DomainModel}ValidatorsTests.cs  ← Tests for Common.Contracts AbstractValidator<T> classes
└── IUnitTestsAssemblyMarker.cs

MyHomeRamen.IntegrationTests/           ← Integration tests — API vertical slice (API → Domain → DB)
├── Common/                             ← Shared test infrastructure across all modules
│   ├── WebApiFactory.cs                ← WebApplicationFactory<IApiAssemblyMarker> + IAsyncLifetime
│   │                                   ←   spins up MsSqlContainer + RedisContainer, seeds data, creates HttpClient
│   │                                   ←   registered as [assembly: AssemblyFixture(typeof(WebApiFactory))]
│   ├── BaseIntegrationTest.cs
│   └── Configuration/
│       ├── ApiConfig.cs                ← Static config constants (e.g. RestaurantId)
│       ├── ApiServicesExtensions.cs    ← ReconfigureDatabase / ReconfigureCache / ReconfigureTokenOptions
│       ├── HttpClientExtensions.cs     ← CreatePostMessage / WithJsonContent / AddAuthorizationHeader / ResponseToDto
│       ├── JwtTokenFactory.cs          ← GenerateAdminToken / GenerateEmployeeToken / GenerateCustomerToken
│       ├── FakeUser.cs                 ← ICurrentUser stub for DbContext in tests
│       └── UserRoles.cs                ← Enum: Admin | Employee | Customer
├── {Module}Module/                     ← One folder per business module under test
│   ├── Common/
│   │   └── Data/
│   │       ├── DataGenerator.cs        ← Bogus Faker<T> fakers; tracks generated entities;
│   │       │                           ←   provides GenerateValid*, GetRandom*, InvalidXxxRequests() TheoryData
│   │       ├── DataSeeder.cs           ← Seeds module data in dependency order via DbContext
│   │       └── Mappings.cs             ← Domain entity → API request extension methods for test setup
│   └── {FeatureName}Tests.cs           ← e.g. CreateProductTests.cs — [Fact]/[Theory] test class
└── IIntegrationTestsAssemblyMarker.cs
```