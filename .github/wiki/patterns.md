# Patterns

This file contains canonical examples of patterns that agents must follow when generating code.
It replaces runtime codebase scanning — agents load this file instead of searching for existing patterns.

---

## Domain Patterns

### Entity ID (value object)

```csharp
// MyHomeRamen.Domain\Menu\Products\ProductId.cs
using MyHomeRamen.Api.Common.Domain;

namespace MyHomeRamen.Domain.Menu.Products;

public readonly record struct ProductId(Guid Value) : IEntityId
{
	public static implicit operator Guid(ProductId id) => id.Value;
	public static implicit operator ProductId(Guid value) => new(value);

	public override string ToString() => Value.ToString();
}
```

### Entity class

```csharp
// MyHomeRamen.Domain\Menu\Ingredients\Ingredient.cs
public sealed class Ingredient : AuditableEntity, IEntity<IngredientId>
{
	private List<Category> _categories = [];

	public IngredientId Id { get; private set; }
	public string Name { get; private set; } = string.Empty;
	public string Description { get; private set; }
	public decimal Price { get; private set; }
	public IReadOnlyList<Category> Categories => _categories.ToList();

	private Ingredient() { }  // ← required private parameterless ctor for EF Core

	private Ingredient(IngredientId id, IEnumerable<Category> categories)
	{
		Id = id;
		_categories = categories.ToList();
	}

	// ← static factory method; calls validator before returning
	public static Ingredient Create(IngredientId id, string name, string description, decimal price, IEnumerable<Category> categories)
	{
		Ingredient ingredient = new(id, categories)
		{
			Name = name,
			Description = description,
			Price = price
		};
		IngredientValidator.Validate(ingredient);
		return ingredient;
	}

	public void Update(string name, string description, decimal price, IEnumerable<Category> categories)
	{
		Name = name;
		Description = description;
		Price = price;
		_categories = categories.ToList();
		IngredientValidator.Validate(this);
	}
}
```

---

## Persistence Patterns

### EF Core configuration

```csharp
// MyHomeRamen.Persistance\Menu\Configurations\ProductConfiguration.cs
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.HasKey(x => x.Id);
		builder.Property(x => x.RestaurantId).IsRequired();
		builder.Property(x => x.Name).IsRequired().HasMaxLength(ProductConstants.MaxNameLength);
		builder.Property(x => x.Price).IsRequired().HasPrecision(18, 2);

		builder.HasMany(x => x.BaseIngredients)
			   .WithMany()
			   .UsingEntity(j => j.ToTable("ProductBaseIngredients"));
	}
}
```

### DbContext extension methods

Extensions live in `MyHomeRamen.Persistance\{Module}\Extensions\{Entity}DbExtensions.cs` as a `partial` static `DbExtensions` class using C# 14 extension members syntax.

```csharp
// MyHomeRamen.Persistance\Menu\Extensions\ProductDbExtensions.cs
namespace MyHomeRamen.Persistance.Common;

public static partial class DbExtensions
{
	extension(IQueryable<Product> products)
	{
		public IQueryable<Product> WithAllIngredients()
			=> products
				.AsNoTracking()
				.Include(p => p.BaseIngredients)
				.Include(p => p.CustomIngredients);

		public async Task<bool> IsNameUniqueAsync(string name, CancellationToken ct = default)
			=> await products.Exists(p => p.Name.ToLower() != name.ToLower(), ct);
	}
}
```

Generic helpers (`Paged`, `Exists`, `GetList`, `GetById`, etc.) are in `MyHomeRamen.Persistance\Common\RepositoryDbExtensions.cs`.

---

## API Layer Patterns

### Folder structure per feature

```
MyHomeRamen.Api\{Module}\Features\{Entity}\{Operation}\
	{Operation}{Entity}Endpoint.cs
	{Operation}{Entity}Handler.cs
	Models\
		{Operation}{Entity}Request.cs
		{Operation}{Entity}Response.cs
		Mappings.cs
	Policies\
		{Operation}{Entity}Validator.cs
```

### POST endpoint

```csharp
// CreateProductEndpoint.cs
public sealed class CreateProductEndpoint : IEndpoint
{
	public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
	{
		endpointBuilder
			.MapStandardValidatedPost<CreateProductRequest, CreateProductResponse>("api/menu/products", HandleAsync)
			.WithName("CreateProductEndpoint")
			.WithTags("Menu")
			.WithDescription("Handles Create Product operations.")
			.RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
	}

	private static async Task<IResult> HandleAsync(
		[FromBody] CreateProductRequest request,
		[FromServices] IRequestHandler<CreateProductRequest, Guid> handler,
		CancellationToken cancellationToken)
	{
		Guid id = await handler.Handle(request, cancellationToken);
		CreateProductResponse response = new(id);
		return Results.Created($"/api/menu/products/{id}", response);
	}
}
```

### PUT endpoint (with route ID binding)

```csharp
// UpdateProductEndpoint.cs
public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
{
	endpointBuilder
		.MapStandardValidatedPutWithResponse<UpdateProductRequest, UpdateProductResponse>("products/{id}", HandleAsync)
		// ...
}

private static async Task<IResult> HandleAsync(
	[FromRoute] UpdateProductRequestId id,   // ← separate record for route param
	[FromBody] UpdateProductRequest request,
	[FromServices] IRequestHandler<UpdateProductRequest, UpdateProductResponse> handler,
	CancellationToken cancellationToken)
{
	UpdateProductResponse response = await handler.Handle(request with { Id = id.Id }, cancellationToken);
	return Results.Ok(response);
}
```

### DELETE endpoint

```csharp
// DeleteIngredientEndpoint.cs
public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
{
	endpointBuilder
		.MapStandardValidatedDelete<DeleteIngredientRequest>("ingredients/{id}", HandleAsync)
		// ...
}

private static async Task<IResult> HandleAsync(
	DeleteIngredientRequest id,
	[FromServices] IRequestHandler<DeleteIngredientRequest, IResult> handler,
	CancellationToken cancellationToken)
	=> await handler.Handle(id, cancellationToken);
```

### GET single

```csharp
// GetIngredientByIdEndpoint.cs
public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
{
	endpointBuilder
		.MapStandardValidatedGet<GetIngredientByIdRequest, GetIngredientByIdResponse>("api/menuingredients/{id}", HandleAsync)
		// ...
}

private static async Task<IResult> HandleAsync(
	GetIngredientByIdRequest id,
	[FromServices] IRequestHandler<GetIngredientByIdRequest, GetIngredientByIdResponse> handler,
	CancellationToken cancellationToken)
{
	GetIngredientByIdResponse response = await handler.Handle(id, cancellationToken);
	return Results.Ok(response);
}
```

### GET list with filter + pagination

```csharp
// GetIngredientsForManageEndpoint.cs
private static async Task<IResult> HandleAsync(
	[AsParameters] GetIngredientsForManageRequest request,  // ← [AsParameters] for query string binding
	[AsParameters] PageParameters pageParameters,
	[FromServices] IRequestHandler<GetIngredientsForManageRequest, GetIngredientsForManageResponse> handler,
	CancellationToken cancellationToken)
{
	request.PageParameters = pageParameters;
	GetIngredientsForManageResponse response = await handler.Handle(request, cancellationToken);
	return Results.Ok(response);
}
```

### Handler

```csharp
// CreateProductHandler.cs — uses primary constructor for DI
public sealed class CreateProductHandler(IMenuDbContext dbContext) : IRequestHandler<CreateProductRequest, Guid>
{
	public async Task<Guid> Handle(CreateProductRequest request, CancellationToken cancellationToken)
	{
		Category category = await dbContext.Categories
										   .FirstAsync(c => c.Id == (CategoryId)request.CategoryId, cancellationToken);
		IEnumerable<Ingredient> ingredients = await dbContext.Ingredients
															 .GetByIds(request.IngredientIds.Select(id => (IngredientId)id), cancellationToken);
		Product product = request.ToDomain(category, ingredients, []);
		dbContext.Products.Add(product);
		await dbContext.SaveChangesAsync(cancellationToken);
		return product.Id.Value;
	}
}
```

### Validator (with route ID extraction and async DB checks)

```csharp
// UpdateProductValidator.cs
public sealed class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
	public UpdateProductValidator(IMenuDbContext dbContext, IHttpContextAccessor httpContextAccessor)
	{
		RuleFor(x => x.Name).SetValidator(new ProductNameValidator());

		// Async DB existence check — extract ID from route
		RuleFor(x => x)
			.MustAsync(async (_, ct) =>
			{
				Guid id = httpContextAccessor.GetGuidFromRouteParam("id");
				return await dbContext.Products.Exists(p => p.Id == (ProductId)id, ct);
			})
			.WithMessage("Product with the specified ID does not exist.");

		RuleFor(x => x.Name)
			.MustAsync(async (name, ct) =>
			{
				Guid id = httpContextAccessor.GetGuidFromRouteParam("id");
				return await dbContext.Products.IsProductNameUniqueExcludingAsync(name, (ProductId)id, ct);
			})
			.WithMessage("Product with this name already exists.");
	}
}
```

---

## Test Patterns

### Unit test (domain logic)

```csharp
// MyHomeRamen.UnitTests\MenuModule\Products\ProductValidationTests.cs
public class ProductValidationTests
{
	[Fact]
	public void Create_Should_SetPropertiesCorrectly_When_InputIsValid()
	{
		// Arrange
		Collection<Ingredient> baseIngredients = [];
		Collection<Category> categories = [];

		// Act
		Product product = Product.Create(new(Guid.NewGuid()), "Delicious Ramen", "Long description here.", 50.0m, "http://img.jpg", baseIngredients, [], categories);

		// Assert
		Assert.Equal("Delicious Ramen", product.Name);
	}

	[Fact]
	public void Create_Should_ThrowDomainException_When_NameIsTooShort()
	{
		// Arrange
		string name = new('a', ProductConstants.MinNameLength - 1);

		// Act & Assert
		DomainException exception = Assert.Throws<DomainException>(() => CreateProduct(name: name));
		Assert.Equal(ProductErrors.NameTooShort().Message, exception.Message);
	}
}
```

### Integration test (API + DB via TestContainers)

```csharp
// MyHomeRamen.IntegrationTests\MenuModule\Products\CreateProductTests.cs
public sealed class CreateProductTests(WebApiFactory apiFactory)
{
	[Fact]
	public async Task CreateProduct_ShouldReturnLocationHeader_ForValidRequest()
	{
		// Arrange
		HttpStatusCode expectedStatusCode = HttpStatusCode.Created;
		CreateProductRequest request = DataGenerator.GenerateValidProduct().ToCreateProductRequest();

		using HttpRequestMessage httpRequest = HttpClientExtensions.CreatePostMessage("/api/menu/products")
																   .WithJsonContent(request)
																   .AddAuthorizationHeader(UserRoles.Admin);
		// Act
		HttpResponseMessage responseMessage = await apiFactory.HttpClient.SendAsync(httpRequest, TestContext.Current.CancellationToken);

		// Assert
		Assert.True(responseMessage.StatusCode == expectedStatusCode,
			$"Expected status code {expectedStatusCode} but got {responseMessage.StatusCode}.");
		Assert.True(responseMessage.Headers.Location != null, "Expected Location header to be present.");
	}

	[Theory]
	[InlineData(UserRoles.Employee)]
	[InlineData(UserRoles.Customer)]
	public async Task CreateProduct_ShouldReturnForbidden_ForNonAdminUser(UserRoles role)
	{
		// Arrange / Act / Assert pattern same as above, status = Forbidden
	}
}
```
---

## Common Mistakes

| Mistake | Correct approach |
|---|---|
| Using `var` | Always declare the explicit type — `var` is a build error in this project |
| Injecting via constructor field | Use primary constructors: `public sealed class Foo(IBar bar)` |
| Calling `SaveChangesAsync` in loops | Batch all changes, then call once |
| Using `FirstOrDefaultAsync` without null check | Use `FirstAsync` when existence is already validated, or guard the null |
| Registering validator manually | Validators are auto-discovered — do not add manual `services.AddValidator<>()` calls |
| Namespace mismatch | Namespace must exactly mirror the folder path under `MyHomeRamen.*` |
