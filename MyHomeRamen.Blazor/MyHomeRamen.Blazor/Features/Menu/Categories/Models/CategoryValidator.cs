using FluentValidation;
using Microsoft.Extensions.Localization;
using MyHomeRamen.Blazor.Features.Menu.Categories;
using MyHomeRamen.Blazor.Components.Models;

namespace MyHomeRamen.Blazor.Features.Menu.Categories.Models;

public sealed class CategoryValidator : BaseValidator<CategoryModel>
{
    public CategoryValidator(IStringLocalizer<CategoryManagementResources> text)
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(text["NameRequired"])
            .MinimumLength(3).WithMessage(text["NameMinLength"])
            .MaximumLength(50).WithMessage(text["NameMaxLength"]);
    }
}
