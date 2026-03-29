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
            .SetValidator(new IngredientNameValidator());

        RuleFor(x => x.Description)
            .SetValidator(new IngredientDescriptionValidator());

        RuleFor(x => x.Price)
            .SetValidator(new IngredientPriceValidator());

        RuleFor(x => x.Name)
            .MustAsync(BeUniqueNameAsync)
            .WithMessage("Ingredient with same name already exists.");

        RuleFor(x => x.CategoryIds)
            .NotEmpty()
            .WithMessage("At least one category is required.")
            .MustAsync(AllCategoriesExistAndAreIngredientTypeAsync)
            .WithMessage("All categories must exist and be of type Ingredient.");
    }

    private async Task<bool> BeUniqueNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Ingredients.IsIngredientNameUniqueAsync(name, cancellationToken);
    }

    private async Task<bool> AllCategoriesExistAndAreIngredientTypeAsync(IEnumerable<Guid> categoryIds, CancellationToken cancellationToken)
    {
        List<Guid> ids = categoryIds.ToList();

        List<Category> categories = await _dbContext.Categories
            .Where(c => ids.Contains(c.Id.Value) && c.CategoryType == CategoryType.Ingredient)
            .ToListAsync(cancellationToken);

        return categories.Count == ids.Count;
    }
}
