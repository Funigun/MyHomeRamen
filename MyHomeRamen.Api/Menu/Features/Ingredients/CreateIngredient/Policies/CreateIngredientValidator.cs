using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;
using MyHomeRamen.Common.Contracts.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Policies;

public sealed class CreateIngredientValidator : AbstractValidator<CreateIngredientRequest>
{
    private readonly IMenuDbContext _dbContext;

    public CreateIngredientValidator(IMenuDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(x => x.Name)
            .SetValidator(new IngredientNameValidator())
            .MustAsync(BeUniqueNameAsync)
            .WithMessage("Ingredient with this name already exists.");

        RuleFor(x => x.Description)
            .SetValidator(new IngredientDescriptionValidator());

        RuleFor(x => x.Price)
            .SetValidator(new IngredientPriceValidator());

        RuleFor(x => x.CategoryIds)
            .NotEmpty()
            .WithMessage("At least one category is required.")
            .MustAsync(AllCategoriesExistAsync)
            .WithMessage("One or more categories do not exist.")
            .Must(ids => ids.All(id => id != Guid.Empty))
            .WithMessage("Invalid category ID.");
    }

    private async Task<bool> BeUniqueNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Ingredients.IsIngredientNameUniqueAsync(name, cancellationToken);
    }

    private async Task<bool> AllCategoriesExistAsync(List<Guid> categoryIds, CancellationToken cancellationToken)
    {
        int count = await _dbContext.Categories
            .CountAsync(c => categoryIds.Contains(c.Id.Value) && c.CategoryType == CategoryType.Ingredient, cancellationToken);

        return count == categoryIds.Count;
    }
}
