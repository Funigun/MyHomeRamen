---
name: code-quality
description: Comprehensive Code Quality standards for backend blazor, workers and tests in MyHomeRamen solutions.
---

# 1) Code Quality

Code quality standards which can not be automated e.g. REPR, mappings, EF Core, Blazor components, security policies, testing patterns etc.

# 2) What's covered by this skill:

|--|--|
|Area|Description|
|Backend|REPR, mappings, security policies|
|Blazor|Http Clients, mappings, security policies|
|Shared|Domain entities usage, business logic boundaries|
|Workers|Common config, Quartz|
|Tests|General guides, Unit tests, Integration tests, System tests, Blazor tests|

General code style and rules are enforced by `.editorconfig` file. 
Code is also verified by `SonarAnalyzer` and `StyleCop.Analyzers` with some custom rules disabled globally in `.editorconfig` and 
some specific rules disabled in code with `#pragma warning disable` where necessary.
	
## 2.1) Backend:

### a) REPR + CQRS

Backend follows REPR pattern (Request, Endpoint, Processor, Response) for all features which in our case means:
{Feature}Request -> {Feature}Endpoint -> {Feature}Handler -> {Feature}Response.

We still want to follow CQRS pattern so we need to make sure that each endpoint is either a command or a query.

**Query rules**
- is a GET endpoint
- use `.AsNoTracking()` for all queries
- maps to a {Feature}Response DTO
- does not call `SaveChangesAsync()`
- does not publish any events
- does not have any side effects

**Command rules**
- is a POST, PUT, PATCH or DELETE endpoint
- returns `204 No Content` for DELETE
- returns `201 Created` for POST with location header
- returns what the caller needs with `200 OK` for PUT and PATCH
- does not query database after calling `SaveChangesAsync()`
- always use `FluentValidation` validators paired with `IValidationPolicy` interface

**Common rules**
- {Feature}Handler must be public sealed class to be correctly registered in DI container and to prevent inheritance

### b) Mappings

We do not use AutoMapper or any other mapping library. Whole mapping must be done manually via extension methods.

``` csharp
// ❌ Used AutoMapper

CategoryDto category = mapper.Map<CategoryDto>(category);

// ✅ Explicit extension method for reusability in tests

public static CategoryDto ToDto(this Category category)
{
	return new CategoryDto
	{
		Id = category.Id,
		Name = category.Name,
		Description = category.Description
	};
}
```

### c) Security policies

Domain entities can not be used as direct API response or directly used/requested by Blazor components (e.g. via `MyHomeRamen.Common.Contracts`).

``` csharp
// ❌ No authorization
var group = app.MapGroup("/products")
			   .WithTags("Products");

// ✅ Required
var group = app.MapGroup("/products")
               .WithTags("Products")
               .RequireAuthorization();      // ← mandatory

// ✅ Explicit opt-out for public endpoints only
var group = group.MapGet("/products", ...)
				 .AllowAnonymous();          // ← explicit and visible
```

### d) Domain entities

Do not create public constructors for domain entities creation. Instead use static descriptive methods for that.
Always create private empty constructor for EF Core.
Create private constructor with ID and properties that represent relations.

``` csharp
// ❌ public constructor
public sealed class Product
{
	public Product(string name, string description)
	{
		Name = name;
		Description = description;
	}

	public ProductId Id { get; private set; }
	public string Name { get; private set; }
	public string Description { get; private set; }
}

// ❌ missing private constructors
public sealed class Product
{
	public Product(string name, string description)
	{
		Name = name;
		Description = description;
	}

	// private Product() {} // ← missing private empty constructor for EF Core

	// private Product(ProductId id, Category category) {} // ← missing private constructor for relations

	public ProductId Id { get; private set; }
	public string Name { get; private set; }
	public string Description { get; private set; }
	public Category Category { get; private set; }
}


// ✅ Static factory method
public sealed class Product
{
	private Product(ProductId id, Category category)
	{
		Id = id;
		Category = category;
	}

	public static Product Create(ProductId id, string name, string description, Category category)
	{
		return new Product(id, category)
		{
			Name = name,
			Description = description
		};
	}

	public ProductId Id { get; private set; }
	public string Name { get; private set; }
	public string Description { get; private set; }
	public Category Category { get; private set; }
}
```

### e) Domain validation

Domain entities must have separate static validator class which accepts Domain entity and run validation.
Validator must be called from static factory method before returning the entity.

``` csharp
Example part of validator:

internal static class ProductValidator
{
	// Must mutch domain constants
	public const int NameMaxLength = 100;

    internal static void ValidateProduct(Product product)
    {
        CheckName(product);
        CheckDescription(product);
        CheckPrice(product);
        CheckIngredients(product);
        CheckCategories(product);
    }

	...
}

// ❌ missing call to validate entity
public sealed class Category
{
	public static Category Create(CategoryId id, string name, string description)
	{
		// missing call to validate entity

		return new Category(id)
		{
			Name = name,
			Description = description
		};
	}

	private Category(CategoryId id)
	{
		Id = id;
	}

	public CategoryId Id { get; private set; }
	public string Name { get; private set; }
	public string Description { get; private set; }
}

// ✅ call to validate entity

public sealed class Category
{
	public static Category Create(CategoryId id, string name, string description)
	{
		var category = new Category(id)
		{
			Name = name,
			Description = description
		};

		CategoryValidator.ValidateCategory(category); // ← validate entity before returning
		
		return category;
	}
	private Category(CategoryId id)
	{
		Id = id;
	}
	public CategoryId Id { get; private set; }
	public string Name { get; private set; }
	public string Description { get; private set; }
}
```

## 2.2) Blazor:

### a) Http Clients

Blazor components should not call HttpClient directly. Instead strongly typed Http Clients should be user for better maintainability and testing.

``` csharp
// ❌ Direct Http Client usage
@inject HttpClient Http

// ✅ Strongly typed Http Client
@inject IProductClient ProductClient
```

### b) Mappings

Blazor can not use AutoMapper or any other libraries as well. Same as backend, all mappings must be done manually via extension methods.
Additionally we must separate form models from API request/response models.

``` csharp
// ❌ AutoMapper used

CategoryDto category = CategoryClient.GetCategoryById(<GUID>, CancellationToken.None);
CategoryModel categoryModel = mapper.Map<CategoryModel>(category);

// ❌ API response used directly in Blazor form

CategoryDto category = CategoryClient.GetCategoryById(<GUID>, CancellationToken.None);
MudTextField @bind-Value="category.Name" />

// ✅ API response mapped to form model via explicit extension method

public static CategoryModel ToModel(this CategoryDto category)
{
	return new CategoryModel
	{
		Id = category.Id,
		Name = category.Name,
		Description = category.Description
	};
}
```

### c) Security policies

Every page that displays user or admin data must be protected with roles
``` csharp
@* ❌ Missing authorization *@
@page "/products"
@inject ProductsApiService ApiService

@* ✅ Authorized page *@
@page "/products"
@attribute [Authorize("Admin")]			// ← Explicit policy
@inject ProductsApiService ApiService
```

### d) Organize every `.razor` file in the following order:

1. `@page` directive (if the component is a routable page)
2. `@using` statements
3. `@inject` directives
4. HTML / Razor markup section
5. `@code { }` block

## 2.3) Shared:

### a) Domain entities usage
Domain entities must not be used directly in Blazor components or API responses. They should be used only in the backend for data access and business logic.
``` csharp
// ❌ Domain entity used in API response
public class ProductEndpoint : IEndpoint
{
	public void Map(IEndpointRouteBuilder app)
	{
		app.MapGet("/products/{id}", async (Guid id, I{Module}DbContext db) =>
		{
			var product = await db.Products.FindAsync(id);
			return product; // ← Domain entity returned directly
		});
	}
}

// ✅ Domain entity mapped to DTO for API response
public class ProductEndpoint : IEndpoint
{
	public void Map(IEndpointRouteBuilder app)
	{
		app.MapGet("/products/{id}", async (Guid id, I{Module}DbContext db) =>
		{
			var product = await db.Products.FindAsync(id);
			return product.ToDto(); // ← Domain entity mapped to DTO
		});
	}
}
```

### b) Domain & Business logic boundaries

Shared project is not allowed to implement business logic in any way. It should not use Domain project as well.

## 2.4) Workers:

### a) Common config

All workers must use common configuration project `MyHomeRamen.Worker.Common` to implement shared configuration etc.

### b) Scheduling

All workers that require scheduling must use Quartz library for that.
Worker without scheduling can skip Quartz library and use `HostedService` directly.

## 2.5) Tests:

### a) General rules

- Use xUnit as test framework
- Use AAA pattern: Arrange, Act, Assert.
- Use `Theory` and `InlineData` for parameterized tests that cover up to 5 scenarios per test method.
- Use `Theory` and `MemberData` for more complex parameterized tests that require multiple parameters or complex objects.
  Test data should be defined in separate class in dedicated folder for reusability and maintainability
- Use `AssemblyFixture` for setup shared across all tests
- Use `CollectionFixture` for setup shared across tests for specific module

### b) Unit tests rules

- Method Naming conventions: `{DomainModel}_Should{ExpectedBehavior}_When{StateUnderTest}`, `{MethodName}_Should{ExpectedBehavior}_When{StateUnderTest}`
- Test domain models static factory methods and validation logic e.g. `MyHomeRamen.UnitTests/MenuModule/Products/ProductValidationTests.cs`
- Test shared validator including tests of validator constants against domain constants e.g. `MyHomeRamen.UnitTests/MenuModule/Products/ProductValidatorTests.cs`

### c) Integration tests rules

- Method Naming convention: `{EndpointName}_Should{ExpectedBehavior}_When{StateUnderTest}`
- Test API endpoints using "Test" environment
- Tests order: Happy Path -> Unauthorized -> Forbidden -> Bad Request -> Not Found -> Edge cases

### d) System tests rules

- Method Naming convention: `{UserFlow}_Should{ExpectedBehavior}_When{StateUnderTest}`
- Test complex user flows using `Aspire.Hosting.Testing` library
- Complex flow can be recognized when multiple services and infrastructure components are involved e.g.:
	- user registration: Identity Api -> Keycloak -> RabbitMQ -> Messages Handler Worker
	
### e) Blazor tests rules

- Method Naming convention: `{Component}_Should{ExpectedBehavior}_When{StateUnderTest}`
- Do not test DOM structure that might change
- Test semantic content

``` csharp
// ❌ Test DOM structure
cut.Find(".mud-button");

// ✅ Test semantic content
cut.Find("button[type=submit]")
```