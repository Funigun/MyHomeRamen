using FluentValidation;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientById;

public sealed class GetIngredientByIdValidator : AbstractValidator<GetIngredientByIdQuery>
{
    public GetIngredientByIdValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ingredient ID must not be empty.")
            .ChildRules(id =>
                id.RuleFor(id => id)
                    .MustAsync(async (id, ct) => await dbContext.Ingredient.Exists(i => i.Id == (IngredientId)id, ct))
                    .WithMessage("Ingredient with the specified ID does not exist."));
    }
}
