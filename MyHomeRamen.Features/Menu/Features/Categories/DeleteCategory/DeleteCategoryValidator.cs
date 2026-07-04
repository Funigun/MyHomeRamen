using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Category ID must not be empty.")
            .MustAsync(CategoryExists(dbContext)).WithMessage("Category with the specified ID does not exist.")
            .MustAsync(CategoryIsNotUsed(dbContext)).WithMessage("Category is still in use and cannot be deleted.");
    }

    private static Func<Guid, CancellationToken, Task<bool>> CategoryExists(IMenuDbContext dbContext)
    {
        return async (id, cancellationToken) => await dbContext.Category.Exists(c => c.Id == (CategoryId)id, cancellationToken);
    }

    private static Func<Guid, CancellationToken, Task<bool>> CategoryIsNotUsed(IMenuDbContext dbContext)
    {
        return async (id, cancellationToken) =>
        {
            Category category = await dbContext.Category.Specification().ById((CategoryId)id, cancellationToken);

            return category.CategoryType == CategoryType.Product
                ? !await dbContext.Category.Query().IsUsedByProducts((CategoryId)id, cancellationToken)
                : !await dbContext.Category.Query().IsUsedByIngredients((CategoryId)id, cancellationToken);
        };
    }
}
