---
description : Guidelines for the API Presentation layer including Main API and Identity API
applyTo: '*MyHomeRamen.Api*.cs, *MyHomeRamen.Identity.Api*.cs'
---

# API Layer Instructions

## Overview
The Presentation API layer consists of the Main API (`MyHomeRamen.Api`) and Identity API (`MyHomeRamen.Identity.Api`). 
Both expose REST endpoints following the minimal API, modular monolith, and vertical slice architecture patterns.

## General API Guidelines
- Use minimal APIs.
- Domain models are defined within the `MyHomeRamen.Domain` project under respective module folders.
- Cross-cutting concerns are handled by abstractions, filters, and middlewares defined in `MyHomeRamen.Api.Common`:
	- Global:	
		- Logging: implemented using `LoggingMiddleware`
		- Exception handling: implemented using `ExceptionHandlingMiddleware`
		- Performance monitoring: implemented using `PerformanceMiddleware`
	- Per feature:
		- Authorization requires `IAuthorizationPolicy` implementation for the feature
		- Validation requires `AbstractValidator<T>` (FluentValidation library)
		- Caching requires `ICachePolicy` implementation for the feature
- Feature folders must be organized according to `feature-structure.instructions.md`.
- Modules should never reference each other directly (enforced by architecture tests).
- Feature handlers should use proper repository implementation instead of DbContext abstraction and self cache implementation.

## Main API (`MyHomeRamen.Api`) Structure
Organized by business Modules (e.g., Orders, Ingredients, Payments) and then features within those modules.

|-- {Module}
|	-- Features/
|		-- {DomainModelPlural}/
|			-- {FeatureName1}/
|			-- {FeatureName2}/
|			-- {FeatureName}Group.cs
|		-- {OtherDomainModelPlural}/
|			-- {FeatureName1}/
|			-- {FeatureName2}/
|			-- {FeatureName}Group.cs
|	-- Services/ (shared services for the module)
|	-- ExternalApis/ (integration to expose features for other modules)

## Identity API (`MyHomeRamen.Identity.Api`) Structure
Handles user management, authentication, and authorization using ASP.NET Core Identity integrated with Keycloak for admin operations.
There are no separate modules defined for Identity API, but features are organized into two main groups: Account and Admin. Follows Vertical Slice architecture for features implementation.

|-- Features/
|	-- {GroupName}/
|		-- {FeatureName1}/
|		-- {FeatureName2}/
|		-- {FeatureNameGroup}.cs
|	-- {OtherGroupName}/
|		-- {FeatureName1}/
|		-- {FeatureName2}/
|		-- {FeatureNameGroup}.cs

`{FeatureName}Group.cs` files should implement `IGroupEndpoint` interface from `MyHomeRamen.Api.Common` to define group common settings.
