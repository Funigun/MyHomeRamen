using FluentValidation;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.DeleteIngredient;

public sealed class DeleteIngredientValidator : AbstractValidator<DeleteIngredientCommand>
{
    public DeleteIngredientValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .MustBeValidIngredientId(dbContext)
            .MustNotBeUsedAsBaseIngredient(dbContext)
            .MustNotBeUsedAsCustomIngredient(dbContext);
    }
}
