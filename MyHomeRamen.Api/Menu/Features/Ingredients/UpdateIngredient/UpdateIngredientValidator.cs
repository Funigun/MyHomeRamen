using FluentValidation;
using MyHomeRamen.Api.Common.Extentsions;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Validators;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient;

public sealed class UpdateIngredientValidator : AbstractValidator<UpdateIngredientCommand>
{
    public UpdateIngredientValidator(IMenuDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        RuleFor(x => x.UpdateIngredientRequest.Name)
            .SetValidator(new IngredientNameValidator());

        RuleFor(x => x.UpdateIngredientRequest.Description)
            .SetValidator(new IngredientDescriptionValidator());

        RuleFor(x => x.UpdateIngredientRequest.Price)
            .SetValidator(new IngredientPriceValidator());

        RuleFor(x => x)
            .MustAsync(async (_, ct) =>
            {
                Guid id = httpContextAccessor.GetGuidFromRouteParam("id");
                return await dbContext.Ingredients.Exists(i => i.Id == (IngredientId)id, ct);
            })
            .WithMessage("Ingredient with the specified ID does not exist.");

        RuleFor(x => x.UpdateIngredientRequest.Name)
            .MustAsync(async (name, ct) =>
            {
                Guid id = httpContextAccessor.GetGuidFromRouteParam("id");
                return await dbContext.Ingredients.IsIngredientNameUniqueExcludingAsync(name, (IngredientId)id, ct);
            })
            .WithMessage("Ingredient with this name already exists.");

        RuleFor(x => x.UpdateIngredientRequest.CategoryIds)
            .NotEmpty()
            .WithMessage("At least one category must be selected.");
    }
}
