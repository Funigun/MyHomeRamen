using FluentValidation;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Validators;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.CreateIngredient;

public sealed class CreateIngredientValidator : AbstractValidator<CreateIngredientCommand>
{
    public CreateIngredientValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.CreateIngredientRequest.Name)
            .SetValidator(new IngredientNameValidator());

        RuleFor(x => x.CreateIngredientRequest.Description)
            .SetValidator(new IngredientDescriptionValidator());

        RuleFor(x => x.CreateIngredientRequest.Price)
            .SetValidator(new IngredientPriceValidator());

        RuleFor(x => x.CreateIngredientRequest.Name)
            .MustAsync(async (name, ct) => await dbContext.Ingredients.IsIngredientNameUniqueAsync(name, ct))
            .WithMessage("Ingredient with this name already exists.");

        RuleFor(x => x.CreateIngredientRequest.CategoryIds)
            .NotEmpty()
            .WithMessage("At least one category must be selected.");
    }
}
