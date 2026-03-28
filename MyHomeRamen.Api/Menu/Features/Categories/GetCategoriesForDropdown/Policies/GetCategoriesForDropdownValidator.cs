using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Models;
using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Policies;

public sealed class GetCategoriesForDropdownValidator : AbstractValidator<GetCategoriesForDropdownRequest>
{
    public GetCategoriesForDropdownValidator()
    {
        RuleFor(x => x.CategoryType)
            .Must(BeValidCategoryType).WithMessage("Please select a valid category type.");
    }

    private static bool BeValidCategoryType(int categoryType)
    {
        return Enum.IsDefined(typeof(CategoryType), (CategoryType)categoryType);
    }
}
