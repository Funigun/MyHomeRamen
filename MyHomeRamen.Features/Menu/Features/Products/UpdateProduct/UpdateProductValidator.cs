using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Common.Contracts.Menu.Products.Validators;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Products.Common;

namespace MyHomeRamen.Features.Menu.Features.Products.UpdateProduct;

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.UpdateProductRequest.Name)
            .SetValidator(new ProductNameValidator());

        RuleFor(x => x.UpdateProductRequest.Description)
            .SetValidator(new ProductDescriptionValidator()!);

        RuleFor(x => x.UpdateProductRequest.Price)
            .SetValidator(new ProductPriceValidator());

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                return await dbContext.Products.Exists(p => p.Id == command.Id, ct);
            })
            .WithMessage("Product with the specified ID does not exist.");

        RuleFor(x => x)
            .MustAsync(async (command, ct) =>
            {
                return await dbContext.Products.IsProductNameUniqueExcludingAsync(command.UpdateProductRequest.Name, command.Id, ct);
            })
            .WithMessage("Product with this name already exists.");

        RuleFor(x => x.UpdateProductRequest.CategoryId)
            .NotEmpty()
            .MustAsync(async (id, cancellation) =>
                await dbContext.Categories.AnyAsync(c => c.Id == new CategoryId(id), cancellation))
            .WithMessage("Category does not exist.");

        RuleFor(x => x.UpdateProductRequest.IngredientIds)
            .NotEmpty();

        RuleFor(x => x.UpdateProductRequest.CustomIngredientIds)
            .MustAsync(async (ids, ct) =>
            {
                if (!ids.Any())
                {
                    return true;
                }

                IEnumerable<IngredientId> customIngredientIds = ids.Distinct().Select(id => (IngredientId)id);
                IEnumerable<Ingredient> found = await dbContext.Ingredients.GetByIds(customIngredientIds, ct);
                return found.Count() == ids.Distinct().Count();
            })
            .WithMessage("One or more custom ingredient IDs do not exist.");

        RuleFor(x => x)
            .Must(x => !x.UpdateProductRequest.IngredientIds.Intersect(x.UpdateProductRequest.CustomIngredientIds).Any())
            .WithMessage("Ingredient IDs and custom ingredient IDs must be unique across both collections.");
    }
}
