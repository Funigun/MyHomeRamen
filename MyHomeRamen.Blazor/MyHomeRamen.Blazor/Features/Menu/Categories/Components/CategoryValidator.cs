using FluentValidation;
using MyHomeRamen.Blazor.Common.Models;
using MyHomeRamen.Common.Contracts.Menu.Categories;

namespace MyHomeRamen.Blazor.Features.Menu.Categories.Components;

public sealed class CategoryValidator : BaseValidator<CategoryModel>
{
    public CategoryValidator()
    {
        RuleFor(x => x.Name)
            .SetValidator(new CategoryNameValidator());

        RuleFor(x => x.CategoryType)
            .IsInEnum()
            .WithMessage("Please select a valid category type.");
    }
}
