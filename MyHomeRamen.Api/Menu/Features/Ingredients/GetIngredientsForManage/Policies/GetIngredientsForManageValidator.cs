using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage.Models;
using MyHomeRamen.Domain.Common.Ingredient;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage.Policies;

public sealed class GetIngredientsForManageValidator : AbstractValidator<GetIngredientsForManageRequest>
{
    public GetIngredientsForManageValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(IngredientConstants.MaxNameLength)
            .WithMessage($"Name must not exceed {IngredientConstants.MaxNameLength} characters.");
    }
}
