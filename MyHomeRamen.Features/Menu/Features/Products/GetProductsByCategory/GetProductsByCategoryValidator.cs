using FluentValidation;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Products.Common;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductsByCategory;

public sealed class GetProductsByCategoryValidator : AbstractValidator<GetProductsByCategoryQuery>
{
    public GetProductsByCategoryValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Request.CategoryId)
            .Cascade(CascadeMode.Stop)
            .MustBeExistingProductCategory(dbContext)
            .MustBeProductCategoryType(dbContext);
    }
}
