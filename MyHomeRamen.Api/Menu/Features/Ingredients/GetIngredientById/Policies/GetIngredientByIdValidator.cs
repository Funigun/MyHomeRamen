using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Policies;

public sealed class GetIngredientByIdValidator : AbstractValidator<GetIngredientByIdRequest>
{
    public GetIngredientByIdValidator(IMenuDbContext menuDbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ingredient ID must not be empty.")
            .ChildRules(id =>
                id.RuleFor(id => id)
                    .MustAsync(async (id, ct) => await menuDbContext.Ingredients.ExistsByIdAsync((IngredientId)id, ct))
                    .WithMessage("Ingredient with the specified ID does not exist."));
    }
}
