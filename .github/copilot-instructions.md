# GitHub Copilot instructions for My Home Ramen project

## Project overview

This project is an application for complete Ramen restaurant management built in .Net 10 with modular monolith architecture.
It includes a REST API (minimal API), Blazor Server and WASM frontends and background workers that might use Quartz .Net for scheduling.
Infrastructure includes Keycloak for identity management, RabbitMQ for messaging and Redis for caching.

## Solution structure
```
MyHomeRamen.slnx
├── .editorconfig
├── .gitignore
├── Directory.Build.props
├── Directory.Packages.props
├── README.md
├── .github/
├── MyHomeRamen.AppHost/                    ← .NET Aspire orchestration (entry point for dev)
├── MyHomeRamen.ServiceDefaults/            ← Shared Aspire service defaults (telemetry, health checks, global constants)
├── MyHomeRamen.Api/                        ← Main API project exposing REST endpoints
├── MyHomeRamen.Api.Common/                 ← Common utilities, extensions, and helpers for API
├── MyHomeRamen.Domain/                     ← Domain entities, value objects, and services
├── MyHomeRamen.Persistance/                ← Database contexts and EF Core configurations
├── MyHomeRamen.Infrastructure/             ← Infrastructure services (caching, messaging, email, keycloak)
├── MyHomeRamen.Worker.Common/              ← Base worker with Quartz config and shared worker services
├── MyHomeRamen.Worker.DatabaseInitializer/ ← DB setup and seeding worker
├── MyHomeRamen.Worker.MailSender/          ← Email background worker
├── MyHomeRamen.Worker.MessagesHandler/     ← RabbitMQ message handler
├── MyHomeRamen.Blazor/                     ← Blazor Server frontend
├── MyHomeRamen.Blazor.Client/              ← Blazor WASM frontend
├── MyHomeRamen.UnitTests/                         ← Unit tests (XUnit, NSubstitute)
├── MyHomeRamen.IntegrationTests/                  ← Integration tests (XUnit, TestContainers)
├── MyHomeRamen.IdentityApi.IntegrationTests/      ← Identity integration tests
├── MyHomeRamen.MenuApi.IntegrationTests/          ← Menu integration tests
├── MyHomeRamen.ShoppingCartApi.IntegrationTests/  ← Shopping Cart integration tests
├── MyHomeRamen.ArchitectureTests/                 ← Architecture tests (XUnit, NetArchRules)
└── MyHomeRamen.SystemTests/                       ← System tests (XUnit, Aspire.Hosting.Testing)
```

## Modules isolation
- Modules must **not** reference each other directly (enforced by architecture tests).
- Cross-module integration goes through integration events in `MyHomeRamen.Common.Contracts`.
- Modules are isolated by folders in each project, e.g. `MyHomeRamen.Api/Menu`, `MyHomeRamen.Domain/Menu`, `MyHomeRamen.Persistance/Menu`, etc.

## Conventions & Coding standards
- Use **primary constructors** by default for dependency injection in all classes (Endpoints, Handlers, Services, etc.).
- Never user `var` for variable declarations - they are marked as errors so using `var` will result in build errors.

## Testing
Project uses xUnit for unit, integration and architecture tests.
- Architecture Tests: enforce architectural rules using NetArchRules
- Unit Tests: 
	- focus on testing domain logic and application services that do not have infrastructure or external dependencies
	- naming conventions: `{DomainModel}_Should{ExpectedBehavior}_When{StateUnderTest}`, `{MethodName}_Should{ExpectedBehavior}_When{StateUnderTest}`
- Integration Tests (Test Containers): 
	- test individual services in isolation (e.g. API + DB) using TestContainers
	- naming convention: `{MethodName}_Should{Behavior}_For{Condition}` (e.g., `CreateProduct_ShouldReturnCreated_ForValidRequest`)
- Integration Tests (Aspire): 
	- test complete distributed workflows spanning multiple independent services (API + Identity + Workers + External services) orchestrated by .NET Aspire
	- naming convention: `{Scenario}_Should{ExpectedOutcome}_When{Condition}` (e.g., `ProductManagement_ShouldSucceed_ForValidWorkflow`)

## Copilot agents standards - Hard rules, never ignore
Always respond like smart caveman, cut all filler, keep technical substance
- Drop articles (a, an, the), filler (just, really, basically, actually)
- Drop pleasantries (sure, certainly, happy to).
- Repeat messages between steps of workflow
- No hedging. Fragments fine. Short synonyms
- Technical terms stay exact. Code blocks unchanged
- Pattern: [thing] [action] [reason]. [next step].run

Example:
Good: Research endpoint pattern - consistent codebase - next: research testing patterns
Bad: Now I will search for other patterns to find [...]. Good, now I will find testing examples to see how tests are build in other scenarios [...].
