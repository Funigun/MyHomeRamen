using FluentValidation;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.DeleteIngredient;

public sealed class DeleteIngredientValidator : AbstractValidator<DeleteIngredientCommand>
{
    public DeleteIngredientValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ingredient ID must not be empty.")
            .ChildRules(id =>
                {
                    id.RuleFor(id => id)
                        .MustAsync(IngredientExists(dbContext)).WithMessage("Ingredient with the specified ID does not exist.");

                    id.RuleFor(id => id)
                        .MustAsync(IngredientIsNotUsedAsBaseIngredient(dbContext)).WithMessage("Ingredient is used as a base ingredient by one or more products and cannot be deleted.");

                    id.RuleFor(id => id)
                        .MustAsync(IngredientIsNotUsedAsCustomIngredient(dbContext)).WithMessage("Ingredient is used as an additional ingredient by one or more products and cannot be deleted.");
                }
            );
    }

    private static Func<Guid, CancellationToken, Task<bool>> IngredientExists(IMenuDbContext dbContext)
    {
        return async (id, cancellationToken) => await dbContext.Ingredient.Exists(i => i.Id == (IngredientId)id, cancellationToken);
    }

    private static Func<Guid, CancellationToken, Task<bool>> IngredientIsNotUsedAsBaseIngredient(IMenuDbContext dbContext)
    {
        return async (id, cancellationToken) => !await dbContext.Product.Query().IsIngredientUsedAsBaseByProduct((IngredientId)id, cancellationToken);
    }

    private static Func<Guid, CancellationToken, Task<bool>> IngredientIsNotUsedAsCustomIngredient(IMenuDbContext dbContext)
    {
        return async (id, cancellationToken) => !await dbContext.Product.Query().IsIngredientUsedAsCustomByProduct((IngredientId)id, cancellationToken);
    }
}
