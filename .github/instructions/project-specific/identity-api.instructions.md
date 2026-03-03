---
description : Guidelines Identity API project structure
applyTo: '*MyHomeRamen.Identity.Api*.cs'
---

# Identity API Instructions

## Overview
The identity API (`MyHomeRamen.Identity.Api`) handles user management, authentication, and authorization using ASP.NET Core Identity integrated with Keycloak for admin operations.

## Guidelines
- Use minimal APIs 
- Domain models are defined within `MyHomeRamen.Domain` project under respective module folder - `Users`
- Follow Vertivcal Slice architecture for features implementation
- Separate features between main two groups: Account and Admin
- Feature folders should be organized according to `feature-structure.instructions.md` guidelines
- {FeatureName}Group.cs files should implement `IGroupEndpoint` interface from `MyHomeRamen.Api.Common` to define group common settings
- Cross-cutting concerns are handled by abstractions, filters and middlewares defined in `MyHomeRamen.Api.Common` project:
	- Global:	
		- Logging: implemented using `LoggingMiddleware`
		- Exception handling: implemented using `ExceptionHandlingMiddleware`
		- Performance monitoring: implemented using `PerformanceMiddleware`
	- Per feature:
		- Authorization requires `IAuthorizationPolicy` implementation for the feature
		- Validation requires `IValidator` implementation (FluentValidation library)
		- Caching requires `ICachePolicy` implementation for the feature feature handler should use proper repository implementation instead of DbContext abstraction and self cache implementation

## Structure
There are no separate modules defined for Identity API, but features are organized into groups which should follow the same structure:

|-- Features/{GroupName}/
|   -- {FeatureName1}/
|   -- {FeatureName2}/
|	-- {FeatureNameGroup}.cs
