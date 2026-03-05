---
description : Guidelines for how main API project is structured
applyTo: '*MyHomeRamen.Api*.cs'
---

# API Instructions

## Overview
The main API (`MyHomeRamen.Api`) exposes REST endpoints for restaurant management, following modular monolith and vertical slice patterns.

## Architecture
- Organized by Modules (e.g., Orders, Ingredients, Payments) then features within those modules.
- Uses common utilities from `MyHomeRamen.Api.Common`.
- Integrates with domain, persistence, and infrastructure.

## Guidelines
- Use minimal APIs 
- Domain models are defined within `MyHomeRamen.Domain` project under respective module folders e.g. `Orders`, `Menu`, etc.
- Each module has its own folder under `MyHomeRamen.Api` with subfolders for features and shared code.
- Modules should not reference each other directly (enforced by architecture tests).
- Cross-cutting concerns are handled by abstractions, filters and middlewares defined in `MyHomeRamen.Api.Common` project:
	- Global:	
		- Logging: implemented using `LoggingMiddleware`
		- Exception handling: implemented using `ExceptionHandlingMiddleware`
		- Performance monitoring: implemented using `PerformanceMiddleware`
	- Per feature:
		- Authorization requires `IAuthorizationPolicy` implementation for the feature
		- Validation requires `AbstractValidator<T>` implementation (FluentValidation library)
		- Caching requires `ICachePolicy` implementation for the feature feature handler should use proper repository implementation instead of DbContext abstraction and self cache implementation
- Feature folders should be organized according to `feature-structure.instructions.md` guidelines

## Structure
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
|	-- ExternalApis/ (integration with other module, expose features for other modules)