using FluentValidation;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Products.Common;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.DeleteIngredient;

public sealed class DeleteIngredientValidator : AbstractValidator<DeleteIngredientCommand>
{
    public DeleteIngredientValidator(IMenuDbContext menuDbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ingredient ID must not be empty.")
            .ChildRules(id =>
                {
                    id.RuleFor(id => id)
                        .MustAsync(IngredientExists(menuDbContext)).WithMessage("Ingredient with the specified ID does not exist.");

                    id.RuleFor(id => id)
                        .MustAsync(IngredientIsNotUsedAsBaseIngredient(menuDbContext)).WithMessage("Ingredient is used as a base ingredient by one or more products and cannot be deleted.");

                    id.RuleFor(id => id)
                        .MustAsync(IngredientIsNotUsedAsCustomIngredient(menuDbContext)).WithMessage("Ingredient is used as an additional ingredient by one or more products and cannot be deleted.");
                }
            );
    }

    private static Func<Guid, CancellationToken, Task<bool>> IngredientExists(IMenuDbContext menuDbContext)
    {
        return async (id, cancellationToken) => await menuDbContext.Ingredients.Exists(i => i.Id == (IngredientId)id, cancellationToken);
    }

    private static Func<Guid, CancellationToken, Task<bool>> IngredientIsNotUsedAsBaseIngredient(IMenuDbContext menuDbContext)
    {
        return async (id, cancellationToken) => !await menuDbContext.Products.IsIngredientUsedAsBaseByProductAsync((IngredientId)id, cancellationToken);
    }

    private static Func<Guid, CancellationToken, Task<bool>> IngredientIsNotUsedAsCustomIngredient(IMenuDbContext menuDbContext)
    {
        return async (id, cancellationToken) => !await menuDbContext.Products.IsIngredientUsedAsCustomByProductAsync((IngredientId)id, cancellationToken);
    }
}
