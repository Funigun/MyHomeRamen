using FluentValidation;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Validators;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient;

public sealed class UpdateIngredientValidator : AbstractValidator<UpdateIngredientCommand>
{
    public UpdateIngredientValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.UpdateIngredientRequest.Name)
            .SetValidator(new IngredientNameValidator());

        RuleFor(x => x.UpdateIngredientRequest.Description)
            .SetValidator(new IngredientDescriptionValidator());

        RuleFor(x => x.UpdateIngredientRequest.Price)
            .SetValidator(new IngredientPriceValidator());

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                return await dbContext.Ingredients.Exists(i => i.Id == command.Id, ct);
            })
            .WithMessage("Ingredient with the specified ID does not exist.");

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                return await dbContext.Ingredients.IsIngredientNameUniqueExcludingAsync(command.UpdateIngredientRequest.Name, command.Id, ct);
            })
            .WithMessage("Ingredient with this name already exists.");

        RuleFor(x => x.UpdateIngredientRequest.CategoryIds)
            .NotEmpty()
            .WithMessage("At least one category must be selected.");
    }
}
