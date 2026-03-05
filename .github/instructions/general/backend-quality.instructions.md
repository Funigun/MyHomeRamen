---
description : Guidelines for backend code quality
applyTo: '*.cs'
---

# Backend Quality Instructions

## Overview
Backend quality ensures code maintainability, performance, and security across API, domain, persistence, etc.
This file covers general guidelines which will apply also to other projects like Persistance, Domain or even Blazor

## Guidelines
- Follow coding standards from .editorconfig, StyleCop, SonarAnalyzer.
- Use async/await for I/O operations.
- Review for code smells and refactor.
- Single file should not exceed 250 lines of code (excluding usings and namespaces).
- Never use `var` for any type
- Use latest C# syntax and features where appropriate.

## Tools
- .editorconfig
- StyleCop.Analyzers
- SonarAnalyzer.CSharp

## Nuget packages management
Project follows central package management approach:
- `Directory.Packages.props` defines all package versions centrally
- `Directory.Build.props` defines which packages are used in which projects 

## DTO guidelines
- Additional DTOs should be defined in `DTOs` folder under `Models` folder of the feature
- DTOs should be named with `{Entity}Dto` convention and should be used for mapping between domain models and request/response models
- DTOs should not be used directly in `IEndpoint` implementations and should be mapped to request/response models defined in `Models` folder of the feature
- DTOs should be designed for specific use cases
- DTOs should be created for complex data structure or to avoid exposing domain models directly in API contracts

## Request and response DTOs guidelines
- Request and Response dtos should be defined in `Models` folder of the feature
- Request model should implement `IRequest` or `IRequest<TResponse>` from `MyHomeRamen.Api.Common` project
- Response model should be defined as separete class in `Models` folder
- For features like Get{Entity}ById Request model should implement `IRequestId` interface from `MyHomeRamen.Api.Common` project which is required for to resolve Validation/Architecture policies correctly
- Request / Response models should be built with additional additional DTOs defined in `DTOs` when applicable

## Mapping guidelines
- Mapping between domain models and dtos should be defined in `Mappings.cs` file in `Models/DTOs` folder of the feature
- Mapping should be implemented manually using extension methods and should be defined as static methods in `Mappings.cs` file
- Never use external libraries for mapping like AutoMapper etc.

## IGroupEndpoint implementation guidelines
- Group endpoint should be defined for each group of features that are related to the same domain model
- Group endpoint should be implemented in features group folder once (as per project structure guidelines)
- It must define `GroupName` property which will be used by `IEndpoint` implementations
- It must define `.WithTags({TagName})` which will be used for endpoint grouping in api documentation
- It must define `.WithDescription({GroupDescription})` which will be used for group description in api documentation

## IEndpoint implementation guidelines
- Each `IEndpoint` must use define methods:
	- `GroupName` property which should match with `IGroupEnpoint` implementation that should exist in feature parent folder
	- .WithName({EndpointName})
    - .WithDescription({EndpointDescription})
	- .WithAuthorizationPolicy({PolicyName})
- Each endpoint must define async Task method for request handling with request, handler and cancellation token parameters
- Each endpoint must use extension method for endpoint configuration which can be found in `EndpointBuilderExtensions` class in `MyHomeRamen.Api.Common` project to configure endpoint with proper generic parameters, name and route.

Example endpoint implementation:
#### POST Endpoint
```csharp
public sealed class {FeatureName}Endpoint : IEndpoint
{
	public string GroupName { get; init; } = "{Module}";

	public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
	{
		app.MapStandardGet<{Request}, {Response}>(string.Empty, Handler)
		   .WithName("{FeatureName}Endpoint")
		   .WithDesciption("Handles {FeatureName} operations.")
		   .WithAuthorizationPolicy(PolicyConstants.Anonymous);
	}

	private async Task<IResult> HandleAsync([FromBody] Request request, {RequestHandler}, CancellationToken cancellationToken)
	{
		// Implementation here

		return Results.Created($"/api/{Module}/{id}", new Response());
	}
}
```

#### GET Endpoint (By Id)
```csharp
public sealed class {FeatureName}Endpoint : IEndpoint
{
	public string GroupName { get; init; } = "{Module}";

	public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
	{
		app.MapStandardGet<Request, Response>(string.Empty, Handler)
		   .WithName("{FeatureName}Endpoint")
		   .WithDesciption("Handles {FeatureName} operations.")
		   .WithAuthorizationPolicy(PolicyConstants.Admin);
	}

	private async Task<Results<Ok<Response>, NotFound>>  HandleAsync(Request request, {RequestHandler}, CancellationToken cancellationToken)
	{
		// Implementation here

		return TypedResults.Ok(new Response());
	}
}
```

#### GET Endpoint (collection)

```csharp
public sealed class {FeatureName}Endpoint : IEndpoint
{
	public string GroupName { get; init; } = "{Module}";

	public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
	{
		app.MapStandardGet<Response>(string.Empty, Handler)
		   .WithName("{FeatureName}Endpoint")
		   .WithDesciption("Handles {FeatureName} operations.")
		   .WithAuthorizationPolicy(PolicyConstants.Customer);
	}

	private async Task<Results<Ok<Response>, NotFound>>  HandleAsync({RequestHandler}, CancellationToken cancellationToken)
	{
		// Implementation here

		return TypedResults.Ok(new Response());
	}
}
```

#### PUT Endpoint
```csharp
public sealed class {FeatureName}Endpoint : IEndpoint
{
	public string GroupName { get; init; } = "{Module}";

	public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
	{
		app.MapStandardAuthenticatedPut<Request, Dto><{Request}, {Response}>(string.Empty, Handler)
		   .WithName("{FeatureName}Endpoint")
		   .WithDesciption("Handles {FeatureName} operations.")
		   .WithAuthorizationPolicy(PolicyConstants.Employee);
	}

	private async Task<IResult> HandleAsync(Request request, {RequestHandler}, CancellationToken cancellationToken)
	{
		// Implementation here

		return Results.Created($"/api/{Module}/{id}", new Response());
	}
}
```

#### Delete Endpoint

```csharp
public sealed class {FeatureName}Endpoint : IEndpoint
{
	public string GroupName { get; init; } = "{Module}";

	public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
	{
		app.MapStandardAuthenticatedDelete<AuthorizationPolicy, Request>("/{id}", Handler)
		   .WithName("{FeatureName}Endpoint")
		   .WithDesciption("Handles {FeatureName} operations.")
		   .WithAuthorizationPolicy(PolicyConstants.Admin);
	}

	private async Task<IResult> HandleAsync(Request id, {RequestHandler}, CancellationToken cancellationToken)
	{
		// Implementation here

		return Results.NoContent();
	}
}
```

## Policies guidelines

### Validation policies
- Validation policies should be defined in `Policies` folder of the feature
- Validation policy should be a public sealed class with name convention `{FeatureName}Validator`
- Validation policies should implement `AbstractValidator<TRequest>` interface from `FluentValidation` library
- Validators are divided into two categories:
	- basic: validations which do not require access to database or other services e.g. string length, required fields, date range, etc.
	- persistance: validations which require access to database or other services e.g. checking if user exists, if email is unique, etc.
- Basic validations should be implemented by creating `AbstractValidator<T>` implementation in `MyHomeRamen.Shared.Contracts` project to be reusable across modules, features and projects (API, Blazor)
- Simple persistance validators (e.g. item exists, is unique etc) should be implemented via extension methods defined in `Common/Validation` folder in `MyHomeRamen.Persistance` project to be reusable across modules and features
- Complex persistance validators should be implemented in `Policies` folder of the feature as separate `AbstractValidator<TRequest>` implementation and should use extension methods for simple validations when applicable

When to use validation policies:
- POST and PUT endpoints should always have validation policies
- GET endpoints should have validation for:
	- GetById: should validate if the provided id is valid and not empty and entity exists
	- Get collection: should validate query parameters if applicable e.g. date range, paging parameters, filters etc.
- DELETE endpoints should validate if provided ID is valid and entity exists

### Authorization policies
- Authorization policies should be defined in `Policies` folder of the feature
- Authorization policy should be a public sealed class with name convention `{FeatureName}AuthorizationPolicy`

When to use authorization policies:
Authorization policies should be applied to all endpoints that require additional verification (e.g. permissions) besides roles. 
This should be verified during feature planning step.

### Cache policies
- Cache policies should be defined in `Policies` folder of the feature
- Cache policy should be a public sealed class with name convention `{FeatureName}CachePolicy`
- Cache policies should implement `ICachePolicy<TRequest, TResponse>` interface from `MyHomeRamen.Api.Common` project

When to use cache policies:
Cache policies should be applied to GET endpoints which are expected to have high traffic and where data does not change frequently. 
This should be verified during feature planning step.

## Feature structure guidelines
Each feature should have its own folder and should be organized according to the following structure:

|..{FeatureName}
|	-- Models/
|		-- DTOs/
|			-- RequestDto.cs
|			-- ReponseDto.cs
|			-- Mappings.cs
|		-- {FeatureName}Request.cs
|		-- {FeatureName}Response.cs
|	-- Policies/
|		-- {FeatureName}ValidationPolicy.cs
|		-- {FeatureName}AuthorizationPolicy.cs
|		-- {FeatureName}CachePolicy.cs
|-- {FeatureName}Endpoint.cs
|-- {FeatureName}Handler.cs

Structure that lead to Feature folder will be defined by instructions for specific module/project e.g. `api-instructions.md` for API project, `identity-instructions.md` for Identity module etc.