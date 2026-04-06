using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Models;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Policies;

public sealed class GetIngredientByIdValidator : AbstractValidator<GetIngredientByIdRequest>
{
    public GetIngredientByIdValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Ingredient ID must not be empty.");
    }
}
