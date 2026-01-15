# Github Copilot instructions for My Home Ramen project

## Project overview:

This project is an application for complete Ramen restaurant management.

## Features


## Architecture & Patterns:
Project follows Modular Monolith architecture pattern with Vertical Slice architecture principles.



## Solution structure:


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