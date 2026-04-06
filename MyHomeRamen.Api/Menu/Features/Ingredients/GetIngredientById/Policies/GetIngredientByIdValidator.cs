using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Models;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Policies;

public sealed class GetIngredientByIdValidator : AbstractValidator<GetIngredientByIdRequest>
{
    public GetIngredientByIdValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ingredient ID must not be empty.")
            .MustAsync(async (id, ct) => await dbContext.Ingredients.ExistsByIdAsync((IngredientId)id, ct))
            .WithMessage("Ingredient with the specified ID does not exist.");
    }
}
