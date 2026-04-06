using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient.Policies;

public sealed class DeleteIngredientValidator : AbstractValidator<DeleteIngredientRequest>
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
                        .MustAsync(IngredientIsNotUsed(menuDbContext)).WithMessage("Ingredient is still in use and cannot be deleted.");
                }
            );
    }

    private static Func<Guid, CancellationToken, Task<bool>> IngredientExists(IMenuDbContext menuDbContext)
    {
        return async (id, cancellationToken) => await menuDbContext.Ingredients.ExistsByIdAsync((IngredientId)id, cancellationToken);
    }

    private static Func<Guid, CancellationToken, Task<bool>> IngredientIsNotUsed(IMenuDbContext menuDbContext)
    {
        return async (id, cancellationToken) =>
            !await menuDbContext.Products.IsIngredientUsedByProductAsync((IngredientId)id, cancellationToken);
    }
}
