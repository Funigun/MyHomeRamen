using System.Collections.ObjectModel;
using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Products.Common;

namespace MyHomeRamen.Features.Menu.Features.Products.CreateProduct;

public sealed record CreateProductCommand(CreateProductRequest CreateProductRequest) : ICommand<CreateProductResponse>;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.CreateProductRequest.Name)
            .MustMeetProductNameLengthRequirements()
            .MustHaveUniqueProductName(dbContext);

        RuleFor(x => x.CreateProductRequest.Description!)
                .Cascade(CascadeMode.Stop)
                .MustMeetProductDescriptionLengthRequirements();

        RuleFor(x => x.CreateProductRequest.Price)
            .MustBeValidProductPrice();

        RuleFor(x => x.CreateProductRequest.CategoryId)
            .MustBeExistingProductCategory(dbContext);

        RuleFor(x => x.CreateProductRequest.IngredientIds)
            .MustContainIngredients();

        RuleFor(x => x.CreateProductRequest.CustomIngredientIds)
            .MustContainExistingCustomIngredients(dbContext);

        RuleFor(x => x)
            .MustHaveDistinctIngredientIds(
                c => c.CreateProductRequest.IngredientIds,
                c => c.CreateProductRequest.CustomIngredientIds)
            .WithMessage("Ingredient IDs and custom ingredient IDs must be unique across both collections.");
    }
}

public sealed class CreateProductHandler(IMenuDbContext dbContext) : ICommandHandler<CreateProductCommand, CreateProductResponse>
{
    public async Task<CreateProductResponse> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        Category category = await dbContext.Category.Load().ById((CategoryId)command.CreateProductRequest.CategoryId, cancellationToken);

        IEnumerable<Ingredient> ingredients = await dbContext.Ingredient.Load().ByIds(command.CreateProductRequest.IngredientIds.Select(id => (IngredientId)id), cancellationToken);

        IEnumerable<Ingredient> customIngredients = await dbContext.Ingredient.Load().ByIds(command.CreateProductRequest.CustomIngredientIds.Select(id => (IngredientId)id), cancellationToken);

        Product product = command.CreateProductRequest.ToDomain(category, ingredients, customIngredients);

        dbContext.Product.Add(product);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CreateProductResponse(product.Id.Value);
    }
}

internal static class Mappings
{
    public static Product ToDomain(this CreateProductRequest request, Category category, IEnumerable<Ingredient> ingredients, IEnumerable<Ingredient> customIngredients)
    {
        return Product.Create(
            Guid.NewGuid(),
            request.Name,
            request.Description ?? string.Empty,
            request.Price,
            string.Empty,
            new Collection<Ingredient>(ingredients.ToList()),
            new Collection<Ingredient>(customIngredients.ToList()),
            [category]);
    }
}
