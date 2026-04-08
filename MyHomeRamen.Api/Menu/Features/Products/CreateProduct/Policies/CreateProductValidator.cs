using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models;
using MyHomeRamen.Common.Contracts.Menu.Products;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Policies;

public sealed class CreateProductValidator : AbstractValidator<CreateProductRequest>
{
    private readonly IMenuDbContext _dbContext;

    public CreateProductValidator(IMenuDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(x => x.Name)
            .SetValidator(new ProductNameValidator());

        RuleFor(x => x.Description)
            .SetValidator(new ProductDescriptionValidator()!);

        RuleFor(x => x.Price)
            .SetValidator(new ProductPriceValidator());

        RuleFor(x => x.Name)
            .MustAsync(BeUniqueNameAsync)
            .WithMessage("Product with same name already exists");

        // We can add simple validation for categories and ingredients
        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .MustAsync(async (id, cancellation) =>
                await _dbContext.Categories.AnyAsync(c => c.Id == new CategoryId(id), cancellation))
            .WithMessage("Category does not exist.");

        RuleFor(x => x.IngredientIds)
            .NotEmpty();

        RuleFor(x => x.CustomIngredientIds)
            .MustAsync(async (ids, ct) =>
            {
                if (!ids.Any())
                {
                    return true;
                }
                IEnumerable<IngredientId> customIngredientIds = ids.Distinct().Select(id => (IngredientId)id);
                IEnumerable<Ingredient> found = await _dbContext.Ingredients.GetByIds(customIngredientIds, ct);
                return found.Count() == ids.Distinct().Count();
            })
            .WithMessage("One or more custom ingredient IDs do not exist.");

        RuleFor(x => x)
            .Must(x => !x.IngredientIds.Intersect(x.CustomIngredientIds).Any())
            .WithMessage("Ingredient IDs and custom ingredient IDs must be unique across both collections.");
    }

    private async Task<bool> BeUniqueNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Products.IsNameUniqueAsync(name, cancellationToken);
    }
}
