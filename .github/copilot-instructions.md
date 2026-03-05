# Github Copilot instructions for My Home Ramen project

## Project overview:

This project is an application for complete Ramen restaurant management.

## Architecture & Patterns:
Project follows Modular Monolith architecture pattern with Vertical Slice architecture principles.

## Solution structure:
- Aspire orchiestration that setups Redis and RabbitMQ containers besides API, Blazor and Worker projects

- Backend API
- Core Api:
- MyHomeRamen.Common.Contracts: Shared contracts, DTOs, interfaces, and basic validation rules reusable across API, Worker, and Blazor projects
- MyHomeRamen.Api: Main API project exposing REST endpoints
- MyHomeRamen.Api.Common: Common utilities, extensions and helpers for API project
		- MyHomeRamen.Domain: Domain entities, value objects and domain services
		- MyHomeRamen.Persistence: Database context and configurations using Entity Framework Core
		- MyHomeRamen.Infrastructure: Infrastructure services like email, caching, messaging, etc.
	- Identity:
		- MyHomeRamen.Identity: ASP.NET Core Identity implementation for user management, employees management (via Keycloak admin api) and authentication/authorization
		
- Workers:
	- MyHomeRamen.Worker: Base project for background workers, provides Quarts configuration and common services
	- MyHomeRamen.Worker.DatabaseInitializer: Worker that starts on application startup which configures Database (schemas, roles, admin accounts, etc), applies pending migrations and seeds roles and permissions
	- MyHomeRamen.Worker.MailSender: Background worker for email sending
	- MyHomeRamen.Worker.MessagesHandler: Background worker for RabbitMq messaging handling
	
- Blazor Frontend:
	- MyHomeRamen.Blazor: Blazor Server frontend project
	- MyHomeRamen.Blazor.Client: Blazor WASM frontend additional project

- Testing:
	- MyHomeRamen.Tests.Unit: Unit tests project
	- MyHomeRamen.Tests.Integration: Integration tests project
	- MyHomeRamen.Tests.Architecture: Architecture tests project using NetArchRules

## Coding standards
Project uses global files for coding standards and practices:
- .editorconfig
- Directory.Build.props
- Directory.Packages.props

In case you need to add new package references, analyzers or change coding styles, please do so in these global files.

There are also nuget packages for code analysis and style enforcement:
- StyleCop.Analyzers
- SonarAnalyzer.CSharp

## Testing
Project uses xUnit for unit, integration and architecture tests.
- Architecture Tests: enforce architectural rules using NetArchRules
- Unit Tests: focus on testing domain logic and application services that do not have infrastructure or external dependencies
- Integration Tests (Test Containers): test individual services in isolation (e.g. API + DB) using TestContainers
- Integration Tests (Aspire): test complete distributed workflows spanning multiple independent services (API + Identity + Workers + External IdP) orchestrated by .NET Aspire