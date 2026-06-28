using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Common.Contracts.Menu.Products.Validators;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Menu.Features.Products.Common;

namespace MyHomeRamen.Features.Menu.Features.Products.CreateProduct;

public sealed class CreateProductValidator : AbstractValidator<CreateProductCommand>
{
    private readonly IMenuDbContext _dbContext;

    public CreateProductValidator(IMenuDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(x => x.CreateProductRequest.Name)
            .SetValidator(new ProductNameValidator());

        RuleFor(x => x.CreateProductRequest.Description)
            .SetValidator(new ProductDescriptionValidator()!);

        RuleFor(x => x.CreateProductRequest.Price)
            .SetValidator(new ProductPriceValidator());

        RuleFor(x => x.CreateProductRequest.Name)
            .MustAsync(BeUniqueNameAsync)
            .WithMessage("Product with same name already exists");

        RuleFor(x => x.CreateProductRequest.CategoryId)
            .NotEmpty()
            .MustAsync(async (id, cancellation) =>
                await _dbContext.Categories.AnyAsync(c => c.Id == new CategoryId(id), cancellation))
            .WithMessage("Category does not exist.");

        RuleFor(x => x.CreateProductRequest.IngredientIds)
            .NotEmpty();

        RuleFor(x => x.CreateProductRequest.CustomIngredientIds)
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
            .Must(x => !x.CreateProductRequest.IngredientIds.Intersect(x.CreateProductRequest.CustomIngredientIds).Any())
            .WithMessage("Ingredient IDs and custom ingredient IDs must be unique across both collections.");
    }

    private async Task<bool> BeUniqueNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Products.IsNameUniqueAsync(name, cancellationToken);
    }
}
