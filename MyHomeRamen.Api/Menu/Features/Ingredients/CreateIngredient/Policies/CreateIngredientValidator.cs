using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;
using MyHomeRamen.Common.Contracts.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Policies;

public sealed class CreateIngredientValidator : AbstractValidator<CreateIngredientRequest>
{
    public CreateIngredientValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Name)
            .SetValidator(new IngredientNameValidator());

        RuleFor(x => x.Description)
            .SetValidator(new IngredientDescriptionValidator());

        RuleFor(x => x.Price)
            .SetValidator(new IngredientPriceValidator());

        RuleFor(x => x.Name)
            .MustAsync(async (name, ct) => await dbContext.Ingredients.IsIngredientNameUniqueAsync(name, ct))
            .WithMessage("Ingredient with this name already exists.");

        RuleFor(x => x.CategoryIds)
            .NotEmpty()
            .WithMessage("At least one category must be selected.");
    }
}
