using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient.Models;
using MyHomeRamen.Common.Contracts.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient.Policies;

public sealed class UpdateIngredientValidator : AbstractValidator<UpdateIngredientRequest>
{
    public UpdateIngredientValidator(IMenuDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        RuleFor(x => x.Name)
            .SetValidator(new IngredientNameValidator());

        RuleFor(x => x.Description)
            .SetValidator(new IngredientDescriptionValidator());

        RuleFor(x => x.Price)
            .SetValidator(new IngredientPriceValidator());

        RuleFor(x => x)
            .MustAsync(async (_, ct) =>
            {
                Guid id = (Guid)httpContextAccessor.HttpContext!.GetRouteValue("id")!;
                return await dbContext.Ingredients.ExistsByIdAsync((IngredientId)id, ct);
            })
            .WithMessage("Ingredient with the specified ID does not exist.");

        RuleFor(x => x.Name)
            .MustAsync(async (name, ct) =>
            {
                Guid id = (Guid)httpContextAccessor.HttpContext!.GetRouteValue("id")!;
                return await dbContext.Ingredients.IsIngredientNameUniqueExcludingAsync(name, (IngredientId)id, ct);
            })
            .WithMessage("Ingredient with this name already exists.");

        RuleFor(x => x.CategoryIds)
            .NotEmpty()
            .WithMessage("At least one category must be selected.");
    }
}
