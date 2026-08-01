using FluentValidation;
using MyHomeRamen.Features.Menu.Features.Categories.Common;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;

public sealed class GetCategoriesByTypeValidator : AbstractValidator<GetCategoriesByTypeQuery>
{
    public GetCategoriesByTypeValidator()
    {
        RuleFor(x => x.CategoryType)
            .MustBeValidCategoryType();
    }
}
