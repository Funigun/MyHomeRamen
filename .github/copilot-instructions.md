# GitHub Copilot instructions for My Home Ramen project

## 1) Project overview:

This project is an application for complete Ramen restaurant management.

## 2) Solution Architecture:

```
┌──────────────────────────────────────────────────────────────────────────-┐
│                               Aspire AppHost                              │
│   - Local orchestration                                                   │
│   - Service discovery                                                     │
│   - Global configuration and constants (e.g. ServiceName)                 │
├───────────────────────────────────────────────────────────────────────────┤
│                               API Layer                                   │
│   - MyHomeRamen.Api: Main API project exposing REST endpoints             │
│   - MyHomeRamen.Api.Common: Common utilities, extensions, and helpers     │
│   - MyHomeRamen.Identity.Api: Identity management via Keycloak            │
├───────────────────────────────────────────────────────────────────────────┤
│                               Domain Layer                                │
│   - MyHomeRamen.Domain: Domain entities, value objects, and services      │
│     (Modules: Menu, Orders, Payments, Reservations, ShoppingCart, Users)  │
├───────────────────────────────────────────────────────────────────────────┤
│                               Infrastructure Layer                        │
│   - MyHomeRamen.Persistance: Database contexts and EF Core configurations │
│   - MyHomeRamen.Infrastructure: Services like caching, messaging, email   │
├───────────────────────────────────────────────────────────────────────────┤
│                               Worker Services                             │
│   - MyHomeRamen.Worker: Base worker with Quartz config                    │
│   - MyHomeRamen.Worker.DatabaseInitializer: DB setup and seeding          │
│   - MyHomeRamen.Worker.MailSender: Email background worker                │
│   - MyHomeRamen.Worker.MessagesHandler: RabbitMQ message handler          │
├───────────────────────────────────────────────────────────────────────────┤
│                               Frontend                                    │
│   - MyHomeRamen.Blazor: Blazor Server frontend                            │
│   - MyHomeRamen.Blazor.Client: Blazor WASM client                         │
├───────────────────────────────────────────────────────────────────────────┤
│                               Testing                                     │
│   - MyHomeRamen.UnitTests: Unit tests                                     │
│   - MyHomeRamen.IntegrationTests: Integration tests                       │
│   - MyHomeRamen.ArchitectureTests: Architecture tests                     │
│   - MyHomeRamen.SystemTests: Tests using Aspire.Hosting.Testing package   │
│                              to orchestrate comple workflows              │
└───────────────────────────────────────────────────────────────────────────┘
```

## 3) Solution structure:
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
├── MyHomeRamen.Common.Contracts/           ← Shared validators, messages objects
├── MyHomeRamen.Domain/                     ← Domain entities, value objects, and services
├── MyHomeRamen.Persistance/                ← Database contexts and EF Core configurations
├── MyHomeRamen.Infrastructure/             ← Infrastructure services (caching, messaging, email, keycloak)
├── MyHomeRamen.Identity.Api/               ← Identity management via Keycloak
├── MyHomeRamen.Worker.Common/              ← Base worker with Quartz config and shared worker services
├── MyHomeRamen.Worker.DatabaseInitializer/ ← DB setup and seeding worker
├── MyHomeRamen.Worker.MailSender/          ← Email background worker
├── MyHomeRamen.Worker.MessagesHandler/     ← RabbitMQ message handler
├── MyHomeRamen.Blazor/                     ← Blazor Server frontend
├── MyHomeRamen.Blazor.Client/              ← Blazor WASM frontend
├── MyHomeRamen.UnitTests/                  ← Unit tests (XUnit, NSubstitute)
├── MyHomeRamen.IntegrationTests/           ← Integration tests (XUnit, TestContainers)
├── MyHomeRamen.ArchitectureTests/          ← Architecture tests (XUnit, NetArchRules)
└── MyHomeRamen.SystemTests/                ← System tests (XUnit, Aspire.Hosting.Testing)
```

## 4) Technology
| Layer | Technology |
|---|---|
| Orchestration | Aspire (.NET 10) |
| Backend | ASP.NET Core Minimal API (.NET 10) |
| Frontend | Blazor Server (.NET 10) |
| ORM | Entity Framework Core 10 |
| Validation | FluentValidation |
| Mediator | Own implementation (`MyHomeRamen.Api.Common`) |
| Auth | Keycloak |
| Testing | XUnit, NSubstitute, TestContainers, NetArchRules, Aspire.Hosting.Testing |
| CI/CD | GitHub Actions |

## 5) Coding standards
Project uses global files for coding standards and practices:
- .editorconfig
- Directory.Build.props
- Directory.Packages.props
- defined through instructions files in `.github/instructions/` folder and subfolders

In case you need to add new package references, analyzers or change coding styles, please do so in these global files.

There are also NuGet packages for code analysis and style enforcement:
- StyleCop.Analyzers
- SonarAnalyzer.CSharp

## 6) Testing
Project uses xUnit for unit, integration and architecture tests.
- Architecture Tests: enforce architectural rules using NetArchRules
- Unit Tests: focus on testing domain logic and application services that do not have infrastructure or external dependencies
- Integration Tests (Test Containers): test individual services in isolation (e.g. API + DB) using TestContainers
- Integration Tests (Aspire): test complete distributed workflows spanning multiple independent services (API + Identity + Workers + External services) orchestrated by .NET Aspire

## 7) Copilot standards
- Load all relevant instruction files as described in the agent instructions.
- Follow the instructions and guidelines from the loaded files strictly to ensure high quality output.
- Always refer to the coding standards, architecture guidelines, and best practices defined in the instruction files when implementing features or making code changes.
- Include user input and additional details gathered during planning, implementation or review processes when applicable

IMPORTANT:
- always learn from user feedback and code review results to improve the quality of your work
- propose and discuss valuable changes to instruction/agent files to improve the quality of the output and the project in general