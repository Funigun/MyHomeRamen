using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductsByCategory;

public sealed class GetProductsByCategoryValidator : AbstractValidator<GetProductsByCategoryQuery>
{
    public GetProductsByCategoryValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Request.CategoryId)
            .NotEmpty();

        RuleFor(x => x.Request.CategoryId)
            .MustAsync((categoryId, cancellationToken) => dbContext.Category.Exists(category => category.Id == new CategoryId(categoryId), cancellationToken))
            .WithMessage("Category does not exist.")
            .When(x => x.Request.CategoryId != Guid.Empty);

        RuleFor(x => x.Request.CategoryId)
            .MustAsync((categoryId, cancellationToken) => dbContext.Category.Query().IsProductCategoryType(new CategoryId(categoryId), cancellationToken))
            .WithMessage("Category must be a product category.")
            .When(x => x.Request.CategoryId != Guid.Empty);
    }
}
