using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Models;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesByType.Policies;

public sealed class GetCategoriesByTypeValidator : AbstractValidator<GetCategoriesByTypeRequest>
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
