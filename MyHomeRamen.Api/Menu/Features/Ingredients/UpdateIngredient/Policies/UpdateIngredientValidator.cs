using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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

        RuleFor(x => x.Name)
            .MustAsync(async (name, ct) =>
            {
                object? routeId = httpContextAccessor.HttpContext!.GetRouteValue("id");
                if (routeId is null || !Guid.TryParse(routeId.ToString(), out Guid id))
                {
                    return true;
                }

                return await dbContext.Ingredients.IsIngredientNameUniqueExcludingAsync(name, (IngredientId)id, ct);
            })
            .WithMessage("Ingredient with this name already exists.");

        RuleFor(x => x.CategoryIds)
            .NotEmpty()
            .WithMessage("At least one category must be selected.");

        RuleFor(x => x)
            .MustAsync(async (_, ct) =>
            {
                object? routeId = httpContextAccessor.HttpContext!.GetRouteValue("id");
                if (routeId is null || !Guid.TryParse(routeId.ToString(), out Guid id))
                {
                    return false;
                }

                return await dbContext.Ingredients.ExistsByIdAsync((IngredientId)id, ct);
            })
            .WithMessage("Ingredient with the specified ID does not exist.");
    }
}
