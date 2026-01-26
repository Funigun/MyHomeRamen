# Github Copilot instructions for My Home Ramen project

## Project overview:

This project is an application for complete Ramen restaurant management.

## Architecture & Patterns:
Project follows Modular Monolith architecture pattern with Vertical Slice architecture principles.

## Solution structure:
- Aspire orchiestration that setups Redis and RabbitMQ containers besides API, Blazor and Worker projects

- Backend API
	- Core Api:
		- MyHomeRamen.Api: Main API project exposing REST endpoints
		- MyHomeRamen.Api.Common: Common utilities, extensions and helpers for API project
		- MyHomeRamen.Domain: Domain entities, value objects and domain services
		- MyHomeRamen.Persistence: Database context and configurations using Entity Framework Core
		- MyHomeRamen.Infrastructure: Infrastructure services like email, caching, messaging, etc.
	- Identity:
		- MyHomeRamen.Identity: ASP.NET Core Identity implementation for user management and authentication
		
- Workers:
	- MyHomeRamen.Worker: Base project for background workers, provides Quarts configuration and common services
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
- Unit Tests: focus on testing individual components in isolation using mocks for dependencies
- Integration Tests: test the interaction between multiple components and the database using a real database and redis instances using Testcontainers
- Architecture Tests: enforce architectural rules using NetArchRules