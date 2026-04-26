---
name: solution-structure
description: Project, folder organization, naming conventions, and layer boundaries for MyHomeRamen application including Aspire AppHost, ASP.NET Core Minimal API (modular monolith + clean architecture + vertical slices), Blazor App, shared contracts and background workers. Detects existing patterns first, enforces consistency.
---

# 1) Solution Structure

Maintain projects and folder organization.

# 2) High level guidance

a) Analyze existing patterns before suggesting files or folders location
b) Modular monolith: Domain, Main Api (`MyHomeRamen.Api`), Persistence, Message Worker, Tests are organized by business modules (e.g. Orders, Ingredients, Payments) and then features within those modules.
c) Clean Architecture: Domain, Api, Infrastructure, Persistence layers are physically separated for clear boundaries and reusability (e.g. by workers access domain, but now Api)
d) Vertical Slices: Api and Blazor projects are organized by features
	
# 3) Solution structure:
```
MyHomeRamen.slnx
├── .editorconfig
├── .gitignore
├── Directory.Build.props
├── Directory.Packages.props
├── README.md
├── .github/
│   ├── copilot-instructions.md
│   ├── agents/
│   ├── instructions/
│   ├── skills/
│   ├── prompts/
│   └── workflows/
├── MyHomeRamen.AppHost/                    ← .NET Aspire orchestration (entry point for dev)
├── MyHomeRamen.ServiceDefaults/            ← Shared Aspire service defaults (telemetry, health checks, global constants)
├── MyHomeRamen.Api/                        ← Main API project exposing REST endpoints
│   └── {Module}/
│       ├── Features/
│       │   └── {DomainModelPlural}/
│       │       ├── {FeatureName}/
│       │       │   ├── Models/
│       │       │   │   ├── {Entity}Dto.cs           ← optional
│       │       │   │   ├── Mappings.cs
│       │       │   │   ├── {FeatureName}Request.cs
│       │       │   │   └── {FeatureName}Response.cs
│       │       │   ├── Policies/
│       │       │   │   ├── {FeatureName}ValidationPolicy.cs
│       │       │   │   ├── {FeatureName}AuthorizationPolicy.cs  ← optional
│       │       │   ├── {FeatureName}Endpoint.cs
│       │       │   └── {FeatureName}Handler.cs
│       │       └── {DomainModel}Group.cs
│       ├── Services/						← Shared services for the module
│       └── ExternalApis/					← Integration points exposed to other modules
├── MyHomeRamen.Api.Common/                 ← Common utilities, extensions, and helpers for API
├── MyHomeRamen.Common.Contracts/           ← Shared validators, messages objects
│   ├── Account/                            ← Reusable FluentValidation extension methods for account fields
│   │   └── AccountValidationExtensions.cs ← e.g. ValidUserName, ValidName, ValidPassword rule builders
│   ├── Messaging/                          ← Integration event contracts shared between API and workers
│   │   └── {EventName}IntegrationEvent.cs ← e.g. UserRegisteredIntegrationEvent
│   ├── {Module}/                           ← Module-scoped primitive validators (reused by API + Blazor)
│   │   └── {DomainModel}/
│   │       └── {Property}Validator.cs      ← AbstractValidator<string|decimal|int> with exported constants
│   └── ICommonContractsAssemblyMarker.cs
├── MyHomeRamen.Domain/                     ← Domain entities, value objects, and services
│   ├── Common/                             ← Cross-module constants and errors (no entity definitions)
│   │   └── {DomainConcept}/				← e.g. Cross-module shared concepts like product max length
│   │       ├── {DomainConcept}Constants.cs ← e.g. CategoryConstants (MinNameLength, MaxNameLength)
│   │       └── {DomainConcept}Errors.cs    ← e.g. CategoryErrors (static DomainException factories)
│   ├── {Module}/                           ← One folder per bounded context / business module
│   │   ├── Database/
│   │   │   └── I{Module}DbContext.cs       ← DbSet<T> interface — implemented in Persistence layer
│   │   ├── Events/                         ← Domain events raised by aggregates in this module
│   │   │   └── {EntityName}{Action}Event.cs
│   │   └── {AggregateRoot}/                ← One subfolder per aggregate root
│   │       ├── {Entity}.cs                 ← Aggregate root: inherits AuditableEntity, implements IEntity<TId>
│   │       ├── {Entity}Id.cs               ← Strongly-typed ID: readonly record struct with implicit casts
│   │       ├── {Entity}Validator.cs        ← internal static class — validates entity in Create()
│   │       └── {Entity}Enum.cs             ← Enums related to this aggregate (e.g. CategoryType, OrderStatus)
│   └── IDomainAssemblyMarker.cs
├── MyHomeRamen.Persistance/                ← Database contexts and EF Core configurations
│   ├── Common/
│   │   └── RepositoryDbExtensions.cs       ← Generic IQueryable<T> extensions (Paged, Exists, GetList, GetById)
│   └── {Module}/
│       ├── Configurations/                 ← IEntityTypeConfiguration implementations
│       ├── Converters/                     ← EF Core value converters (e.g. strong-ID converters)
│       ├── Extensions/                     ← Entity-specific IQueryable<T> extensions for this module
│       │   └── {Entity}DbExtensions.cs     ← e.g. CategoryDbExtensions, ProductDbExtensions
│       ├── Migrations/                     ← EF Core migrations for the module
│       └── {Module}DbContext.cs
├── MyHomeRamen.Infrastructure/             ← Infrastructure services (caching, messaging, email, keycloak)
├── MyHomeRamen.Identity.Api/               ← Identity management via Keycloak
├── MyHomeRamen.Worker.Common/              ← Base worker with Quartz config and shared worker services
├── MyHomeRamen.Worker.DatabaseInitializer/ ← DB setup and seeding worker
├── MyHomeRamen.Worker.MailSender/          ← Email background worker
├── MyHomeRamen.Worker.MessagesHandler/     ← RabbitMQ message handler — consumes integration events
│   ├── Common/                             ← Shared constants and helpers across all module handlers
│   │   ├── AuthorizationConstants.cs       ← Role name constants used for mapping IdP roles to domain roles
│   │   └── WorkerUser.cs                   ← ICurrentUser implementation for worker context (no real user)
│   ├── {Module}/                           ← One folder per business module that reacts to messages
│   │   └── {Module}{EventName}Handler.cs   ← Implements IIntegrationEventHandler<TEvent>
│   │                                       ←   e.g. MenuUserRegisteredHandler, OrdersUserRegisteredHandler
│   ├── Worker.cs                           ← BackgroundService — subscribes via IMessagesService.ConsumeAsync,
│   │                                       ←   resolves module handlers from DI scope and dispatches events
│   ├── Program.cs
│   └── IWorkerMessagesHandlerAssemblyMarker.cs
├── MyHomeRamen.Blazor/                     ← Blazor Server + WASM frontend (two projects in one folder)
│   ├── MyHomeRamen.Blazor/                 ← Server project (prerendering, auth, layout, routing)
│   │   ├── Presentation/                   ← DI registrations and cross-cutting concerns
│   │   │   ├── ApiDependencyInjection.cs   ← Registers all typed HttpClients per module
│   │   │   ├── NavigationDependencyInjection.cs ← Registers all {Module}NavigationService instances
│   │   │   ├── AuthenticationDependencyInjection.cs
│   │   │   └── Authentication/				← JWT handlers, Claim transformer, Custom Auth State Provider
│   │   ├── Common/                         ← App-wide shared models, services and configuration
│   │   │   ├── Models/
│   │   │   │   ├── FormMode.cs             ← Enum: Create | View | Edit
│   │   │   │   └── BaseValidator.cs        ← AbstractValidator<T> with ValidateValue delegate for MudForm
│   │   │   ├── Services/                   ← Shared services (e.g. MessageService wrapping MudBlazor snackbar)
│   │   │   └── Configuration/              ← App config wrappers (e.g. ThemeConfiguration, RestaurantConfiguration)
│   │   ├── Components/                     ← Global Blazor components (App.razor, MainLayout, NavMenu)
│   │   ├── Features/                       ← Vertical slices organized by module and feature
│   │   │   └── {Module}/
│   │   │       ├── Common/
│   │   │       │   ├── Constants/          ← e.g. MenuRoleConstants.cs
│   │   │       │   ├── Models/             ← Shared read models across features (e.g. CategoryOption, IngredientOption)
│   │   │       │   └── Services/
│   │   │       │       ├── {Module}ApiClient.cs        ← Typed HttpClient: one per module, all API calls here
│   │   │       │       └── {Module}NavigationService.cs ← Routes static class + imperative navigation methods
│   │   │       └── {DomainModel}/          ← One folder per domain model / feature group
│   │   │           ├── Components/         ← Reusable form components shared across feature actions
│   │   │           │   ├── {DomainModel}Form.razor     ← Unified Create/View/Edit form using FormMode
│   │   │           │   ├── {DomainModel}Model.cs       ← UI model with ToXxxRequest() mapping method
│   │   │           │   └── {DomainModel}Validator.cs   ← BaseValidator<TModel> reusing Common.Contracts validators
│   │   │           └── {ActionName}/       ← e.g. CreateProduct/, CategoriesIndex/
│   │   │               ├── {ActionName}Page.razor      ← Routable page wrapping form/list components
│   │   │               ├── {ActionName}Page.razor.cs   ← Code-behind (only if page logic is complex)
│   │   │               └── {ActionName}Request.cs      ← API DTO record for this specific action
│   │   └── Program.cs
│   └── MyHomeRamen.Blazor.Client/          ← WASM project (interactive client-side rendering)
│       ├── Program.cs
│       └── IBlazorClientAssemblyMarker.cs
├── MyHomeRamen.UnitTests/                  ← Unit tests — domain validation + API contract validators
│   ├── {Module}Module/                     ← One folder per business module (e.g. MenuModule, OrdersModule)
│   │   └── {DomainModel}/                  ← One folder per domain model within the module
│   │       ├── {DomainModel}ValidationTests.cs  ← Tests for domain factory method (Create_Should_Throw...)
│   │       └── {DomainModel}ValidatorsTests.cs  ← Tests for Common.Contracts AbstractValidator<T> classes
│   └── IUnitTestsAssemblyMarker.cs
├── MyHomeRamen.IntegrationTests/           ← Integration tests — API vertical slice (API → Domain → DB)
│   ├── Common/                             ← Shared test infrastructure across all modules
│   │   ├── WebApiFactory.cs                ← WebApplicationFactory<IApiAssemblyMarker> + IAsyncLifetime
│   │   │                                   ←   spins up MsSqlContainer + RedisContainer, seeds data, creates HttpClient
│   │   │                                   ←   registered as [assembly: AssemblyFixture(typeof(WebApiFactory))]
│   │   ├── BaseIntegrationTest.cs
│   │   └── Configuration/
│   │       ├── ApiConfig.cs                ← Static config constants (e.g. RestaurantId)
│   │       ├── ApiServicesExtensions.cs    ← ReconfigureDatabase / ReconfigureCache / ReconfigureTokenOptions
│   │       ├── HttpClientExtensions.cs     ← CreatePostMessage / WithJsonContent / AddAuthorizationHeader / ResponseToDto
│   │       ├── JwtTokenFactory.cs          ← GenerateAdminToken / GenerateEmployeeToken / GenerateCustomerToken
│   │       ├── FakeUser.cs                 ← ICurrentUser stub for DbContext in tests
│   │       └── UserRoles.cs                ← Enum: Admin | Employee | Customer
│   ├── {Module}Module/                     ← One folder per business module under test
│   │   ├── Common/
│   │   │   └── Data/
│   │   │       ├── DataGenerator.cs        ← Bogus Faker<T> fakers; tracks generated entities;
│   │   │       │                           ←   provides GenerateValid*, GetRandom*, InvalidXxxRequests() TheoryData
│   │   │       ├── DataSeeder.cs           ← Seeds module data in dependency order via DbContext
│   │   │       └── Mappings.cs             ← Domain entity → API request extension methods for test setup
│   │   └── {FeatureName}Tests.cs           ← e.g. CreateProductTests.cs — [Fact]/[Theory] test class
│   └── IIntegrationTestsAssemblyMarker.cs
├── MyHomeRamen.ArchitectureTests/          ← Architecture rules enforced via NetArchTest
│   ├── Common/
│   │   ├── ArchitectureBuilder.cs          ← Builds ArchUnitNET Architecture from all project assemblies
│   │   └── BaseArchitectureTest.cs         ← Exposes typed Assembly constants for each project;
│   │                                       ←   all test classes inherit from this
│   ├── ModuleTests/                        ← Per-module boundary tests
│   │   └── {Module}ModuleBoundriesTests.cs ← Asserts module does not reference other modules' namespaces
│   ├── ProjectDependencyTests.cs           ← Asserts each project only references its allowed dependencies
│   └── IArchitectureTestsAssemblyMarker.cs (implicit via BaseArchitectureTest)
└── MyHomeRamen.SystemTests/                ← End-to-end tests orchestrated by .NET Aspire
	├── Config/
	│   └── AppConfigurationFixture.cs      ← IAsyncLifetime bootstrapping full AppHost via
	│                                       ←   DistributedApplicationTestingBuilder; waits for all
	│                                       ←   Aspire resources (DB, cache, RabbitMQ, Keycloak, workers)
	│                                       ←   registered as [assembly: AssemblyFixture(typeof(AppConfigurationFixture))]
	├── {WorkflowGroup}/                    ← e.g. KeycloakIntegrationTests/
	│   └── {WorkflowName}Tests.cs          ← e.g. UserRegistrationTests — full distributed flow test
	└── ISystemTestsAssemblyMarker.cs (implicit via AppConfigurationFixture)
```