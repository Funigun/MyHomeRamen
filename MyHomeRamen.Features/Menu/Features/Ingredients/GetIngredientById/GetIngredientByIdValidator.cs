using FluentValidation;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Repository;
    
namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientById;

public sealed class GetIngredientByIdValidator : AbstractValidator<GetIngredientByIdQuery>
{
    public GetIngredientByIdValidator(IMenuDbContext menuDbContext)
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Ingredient ID must not be empty.")
            .ChildRules(id =>
                id.RuleFor(id => id)
                    .MustAsync(async (id, ct) => await menuDbContext.Ingredients.Exists(i => i.Id == (IngredientId)id, ct))
                    .WithMessage("Ingredient with the specified ID does not exist."));
    }
}
