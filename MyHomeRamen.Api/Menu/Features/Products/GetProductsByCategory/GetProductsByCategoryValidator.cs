using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory;

public sealed class GetProductsByCategoryValidator : AbstractValidator<GetProductsByCategoryQuery>
{
    private readonly IMenuDbContext _dbContext;

    public GetProductsByCategoryValidator(IMenuDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(x => x.Request.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Request.CategoryId)
            .MustAsync(CategoryExistsAsync)
            .WithMessage("Category does not exist.")
            .When(x => x.Request.CategoryId != Guid.Empty);

        RuleFor(x => x.Request.CategoryId)
            .MustAsync(IsProductCategoryTypeAsync)
            .WithMessage("Category must be a product category.")
            .When(x => x.Request.CategoryId != Guid.Empty);
    }

    private async Task<bool> CategoryExistsAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _dbContext.Categories.CategoryExistsAsync(new CategoryId(categoryId), cancellationToken);
    }

    private async Task<bool> IsProductCategoryTypeAsync(Guid categoryId, CancellationToken cancellationToken)
    {
        return await _dbContext.Categories.IsProductCategoryTypeAsync(new CategoryId(categoryId), cancellationToken);
    }
}
