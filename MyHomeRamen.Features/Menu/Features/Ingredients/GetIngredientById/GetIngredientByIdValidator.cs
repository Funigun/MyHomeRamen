using FluentValidation;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientById;

public sealed class GetIngredientByIdValidator : AbstractValidator<GetIngredientByIdQuery>
{
    public GetIngredientByIdValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .MustBeValidIngredientId(dbContext);
    }
}
