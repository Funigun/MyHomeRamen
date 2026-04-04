using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory.Models;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory.Policies;

public sealed class DeleteCategoryValidator : AbstractValidator<DeleteCategoryRequest>
{
    public DeleteCategoryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Category ID must not be empty.");
    }
}
