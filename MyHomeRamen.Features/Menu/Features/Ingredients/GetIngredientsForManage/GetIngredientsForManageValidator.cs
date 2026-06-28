using FluentValidation;
using MyHomeRamen.Domain.Common.Ingredient;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForManage;

public sealed class GetIngredientsForManageValidator : AbstractValidator<GetIngredientsForManageQuery>
{
    public GetIngredientsForManageValidator()
    {
        RuleFor(x => x.Request.Name)
            .MaximumLength(IngredientConstants.MaxNameLength)
            .WithMessage($"Name must not exceed {IngredientConstants.MaxNameLength} characters.");
    }
}
