using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;

public sealed class GetCategoriesByTypeValidator : AbstractValidator<GetCategoriesByTypeQuery>
{
    public GetCategoriesByTypeValidator()
    {
        RuleFor(x => x.CategoryType)
            .Must(BeValidCategoryType).WithMessage("Please select a valid category type.");
    }

    private static bool BeValidCategoryType(int categoryType)
    {
        return Enum.IsDefined(typeof(CategoryType), (CategoryType)categoryType);
    }
}
