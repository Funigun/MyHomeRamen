using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Menu.Features.Products.UpdateProduct.Models;
using MyHomeRamen.Common.Contracts.Menu.Products;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
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
    }
}
