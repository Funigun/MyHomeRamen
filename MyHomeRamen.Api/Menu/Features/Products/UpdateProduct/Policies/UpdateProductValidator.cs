using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Menu.Features.Products.UpdateProduct.Models;
using MyHomeRamen.Common.Contracts.Menu.Products;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.UpdateProduct.Policies;

public sealed class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator(IMenuDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        RuleFor(x => x.Name)
            .SetValidator(new ProductNameValidator());

        RuleFor(x => x.Description)
            .SetValidator(new ProductDescriptionValidator()!);

        RuleFor(x => x.Price)
            .SetValidator(new ProductPriceValidator());

        RuleFor(x => x)
            .MustAsync(async (_, ct) =>
            {
                Guid id = (Guid)httpContextAccessor.HttpContext!.GetRouteValue("id")!;
                return await dbContext.Products.ExistsByIdAsync((ProductId)id, ct);
            })
            .WithMessage("Product with the specified ID does not exist.");

        RuleFor(x => x.Name)
            .MustAsync(async (name, ct) =>
            {
                Guid id = (Guid)httpContextAccessor.HttpContext!.GetRouteValue("id")!;
                return await dbContext.Products.IsProductNameUniqueExcludingAsync(name, (ProductId)id, ct);
            })
            .WithMessage("Product with this name already exists.");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .MustAsync(async (id, cancellation) =>
                await dbContext.Categories.AnyAsync(c => c.Id == new CategoryId(id), cancellation))
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
                IEnumerable<Ingredient> found = await dbContext.Ingredients.GetByIds(customIngredientIds, ct);
                return found.Count() == ids.Distinct().Count();
            })
            .WithMessage("One or more custom ingredient IDs do not exist.");

        RuleFor(x => x)
            .Must(x => !x.IngredientIds.Intersect(x.CustomIngredientIds).Any())
            .WithMessage("Ingredient IDs and custom ingredient IDs must be unique across both collections.");
    }
}
