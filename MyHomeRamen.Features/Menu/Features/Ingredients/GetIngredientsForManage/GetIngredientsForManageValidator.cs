using FluentValidation;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForManage;

public sealed class GetIngredientsForManageValidator : AbstractValidator<GetIngredientsForManageQuery>
{
    public GetIngredientsForManageValidator()
    {
        When(x => x.Request.Name is not null, () =>
        {
            RuleFor(x => x.Request.Name!)
                .MustNotExceedIngredientNameLength();
        });
    }
}
