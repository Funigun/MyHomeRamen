using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Common.Contracts.Menu.Categories.Validators;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Categories.CreateCategory;

public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.CreateCategoryRequest.Name)
            .SetValidator(new CategoryNameValidator())
            .MustAsync((name, cancellationToken) => dbContext.Category.Query().IsCategoryNameUnique(name, cancellationToken)).WithMessage("Category with this name already exists.");

        RuleFor(x => x.CreateCategoryRequest.CategoryType)
            .Must(BeValidCategoryType).WithMessage("Please select a valid category type.");
    }

    private static bool BeValidCategoryType(int categoryType)
    {
        return Enum.IsDefined(typeof(CategoryType), (CategoryType)categoryType);
    }
}
