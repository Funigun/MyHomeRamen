using MyHomeRamen.Blazor.Common.Models;
using MyHomeRamen.Common.Contracts.Menu.Categories.Validators;

namespace MyHomeRamen.Blazor.Features.Menu.Categories.Components;

public sealed class CategoryValidator : BaseValidator<CategoryModel>
{
    public CategoryValidator()
    {
        RuleFor(x => x.Name)
            .SetValidator(new CategoryNameValidator());
    }
}
