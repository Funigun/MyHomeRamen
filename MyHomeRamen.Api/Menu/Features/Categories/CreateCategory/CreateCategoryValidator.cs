using FluentValidation;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Persistance.Common;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Common.Contracts.Menu.Categories.Validators;

namespace MyHomeRamen.Api.Menu.Features.Categories.CreateCategory;

public sealed class CreateCategoryValidator : AbstractValidator<CreateCategoryCommand>
{
    private readonly IMenuDbContext _dbContext;

    public CreateCategoryValidator(IMenuDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(x => x.CreateCategoryRequest.Name)
            .SetValidator(new CategoryNameValidator())
            .MustAsync(BeUniqueNameAsync).WithMessage("Category with this name already exists.");

        RuleFor(x => x.CreateCategoryRequest.CategoryType)
            .Must(BeValidCategoryType).WithMessage("Please select a valid category type.");
    }

    private async Task<bool> BeUniqueNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _dbContext.Categories.IsCategoryNameUniqueAsync(name, cancellationToken);
    }

    private static bool BeValidCategoryType(int categoryType)
    {
        return Enum.IsDefined(typeof(CategoryType), (CategoryType)categoryType);
    }
}
