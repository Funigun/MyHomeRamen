using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models;
using MyHomeRamen.Common.Contracts.Menu.Products;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
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
    }

    private async Task<bool> BeUniqueNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Products.IsNameUniqueAsync(name, cancellationToken);
    }
}
