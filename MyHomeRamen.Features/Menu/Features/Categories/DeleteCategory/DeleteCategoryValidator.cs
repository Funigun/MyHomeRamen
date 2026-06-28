using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Ingredients.Common;
using MyHomeRamen.Features.Menu.Features.Products.Common;

namespace MyHomeRamen.Features.Menu.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidator(IMenuDbContext menuDbContext)
    {
        RuleFor(x => x.Id)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Category ID must not be empty.")
            .MustAsync(CategoryExists(menuDbContext)).WithMessage("Category with the specified ID does not exist.")
            .MustAsync(CategoryIsNotUsed(menuDbContext)).WithMessage("Category is still in use and cannot be deleted.");
    }

    private static Func<Guid, CancellationToken, Task<bool>> CategoryExists(IMenuDbContext menuDbContext)
    {
        return async (id, cancellationToken) => await menuDbContext.Categories.Exists(c => c.Id == (CategoryId)id, cancellationToken);
    }

    private static Func<Guid, CancellationToken, Task<bool>> CategoryIsNotUsed(IMenuDbContext menuDbContext)
    {
        return async (id, cancellationToken) =>
        {
            Category category = await menuDbContext.Categories.GetByIdQuery((CategoryId)id, cancellationToken);

            return category.CategoryType == CategoryType.Product
                ? !await menuDbContext.Products.IsCategoryUsedByProductAsync((CategoryId)id, cancellationToken)
                : !await menuDbContext.Ingredients.IsCategoryUsedByIngredientAsync((CategoryId)id, cancellationToken);
        };
    }
}
