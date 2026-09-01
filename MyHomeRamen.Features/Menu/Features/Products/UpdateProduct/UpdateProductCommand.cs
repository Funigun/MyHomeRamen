using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Products.Common;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Products.UpdateProduct;

public sealed record UpdateProductCommand(ProductId Id, UpdateProductRequest UpdateProductRequest) : ICommand<UpdateProductResponse>;

public sealed class UpdateProductAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<UpdateProductCommand>
{
    public async Task<bool> Authorize(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        return currentUser.CanManageProducts() && currentUser.CanEditProduct();
    }
}

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Id.Value)
            .MustBeValidProductId(dbContext);

        RuleFor(x => x.UpdateProductRequest.Name)
            .MustMeetProductNameLengthRequirements();

        When(x => !string.IsNullOrEmpty(x.UpdateProductRequest.Description), () =>
        {
            RuleFor(x => x.UpdateProductRequest.Description!)
                .MustMeetProductDescriptionLengthRequirements();
        });

        RuleFor(x => x.UpdateProductRequest.Price)
            .MustBeValidProductPrice();

        RuleFor(x => x)
            .MustHaveUniqueProductNameExcluding(dbContext, c => c.UpdateProductRequest.Name, c => c.Id)
            .OverridePropertyName(nameof(UpdateProductCommand.UpdateProductRequest) + "." + nameof(UpdateProductCommand.UpdateProductRequest.Name));

        RuleFor(x => x.UpdateProductRequest.CategoryId)
            .MustBeExistingProductCategory(dbContext);

        RuleFor(x => x.UpdateProductRequest.IngredientIds)
            .MustContainIngredients();

        RuleFor(x => x.UpdateProductRequest.CustomIngredientIds)
            .MustContainExistingCustomIngredients(dbContext);

        RuleFor(x => x)
            .MustHaveDistinctIngredientIds(
                c => c.UpdateProductRequest.IngredientIds,
                c => c.UpdateProductRequest.CustomIngredientIds)
            .WithMessage("Ingredient IDs and custom ingredient IDs must be unique across both collections.");
    }
}

public sealed class UpdateProductHandler(IMenuDbContext dbContext) : IRequestHandler<UpdateProductCommand, UpdateProductResponse>
{
    public async Task<UpdateProductResponse> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        Product product = await dbContext.Product.Load().ById(command.Id, cancellationToken);

        Category category = await dbContext.Category.Load().ById(command.UpdateProductRequest.CategoryId, cancellationToken);

        IEnumerable<IngredientId> ingredientIds = command.UpdateProductRequest.IngredientIds.Select(id => (IngredientId)id);
        IEnumerable<Ingredient> ingredients = await dbContext.Ingredient.Load().ByIds(ingredientIds, cancellationToken);

        IEnumerable<IngredientId> customIngredientIds = command.UpdateProductRequest.CustomIngredientIds.Select(id => (IngredientId)id);
        IEnumerable<Ingredient> customIngredients = await dbContext.Ingredient.Load().ByIds(customIngredientIds, cancellationToken);

        product.Update(command.UpdateProductRequest.Name, command.UpdateProductRequest.Description ?? string.Empty, command.UpdateProductRequest.Price, category, ingredients, customIngredients);

        await dbContext.SaveChangesAsync(cancellationToken);

        return product.ToResponse();
    }
}

internal static class Mappings
{
    internal static UpdateProductResponse ToResponse(this Product product)
        => new(product.Id.Value);
}
