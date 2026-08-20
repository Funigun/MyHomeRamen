using FluentValidation;
using MyHomeRamen.Domain.Common.Category;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Categories.Common;

internal static class CategoryValidatorExtensions
{
    public static IRuleBuilderOptions<T, Guid> MustBeValidCategoryId<T>(this IRuleBuilder<T, Guid> ruleBuilder, IMenuDbContext dbContext)
    {
        return ruleBuilder.NotEmpty().WithMessage("Category ID must not be empty.")
                          .MustAsync(async (categoryId, cancellationToken) =>
                           {
                               return await dbContext.Category.Exists(category => category.Id == new CategoryId(categoryId), cancellationToken);
                           })
                            .WithMessage("Category does not exist.");
    }

    public static IRuleBuilderOptions<T, Guid> MustNotBeUsed<T>(this IRuleBuilder<T, Guid> ruleBuilder, IMenuDbContext dbContext)
    {
        return ruleBuilder.MustAsync(async (categoryId, cancellationToken) =>
                           {
                               Category category = await dbContext.Category.Load().ById(new CategoryId(categoryId), cancellationToken);

                               return category.CategoryType == CategoryType.Product
                                   ? !await dbContext.Category.Query().IsUsedByProducts(new CategoryId(categoryId), cancellationToken)
                                   : !await dbContext.Category.Query().IsUsedByIngredients(new CategoryId(categoryId), cancellationToken);
                           })
                          .WithMessage("Category is still in use and cannot be deleted.");
    }

    public static IRuleBuilderOptions<T, int> MustBeValidCategoryType<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        static bool BeValidCategoryType(int categoryType)  => Enum.IsDefined(typeof(CategoryType), (CategoryType)categoryType);
        
        return ruleBuilder.Must(BeValidCategoryType)
                          .WithMessage("Please select a valid category type.");
    }

    public static IRuleBuilderOptions<T, string> MustMeetLengthRequirements<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                            .WithMessage("Category name cannot be empty.")

                          .MinimumLength(CategoryConstants.MinNameLength)
                            .WithMessage($"Category name minimum length is {CategoryConstants.MinNameLength}.")

                          .MaximumLength(CategoryConstants.MaxNameLength)
                            .WithMessage($"Category name maximum length is {CategoryConstants.MaxNameLength}.");
    }

    public static IRuleBuilderOptions<T, string> MustHaveUniqueName<T>(this IRuleBuilder<T, string> ruleBuilder, IMenuDbContext dbContext)
    {
        return ruleBuilder.MustAsync(async (name, cancellationToken) =>
                           {
                               return await dbContext.Category.Query().IsCategoryNameUnique(name, cancellationToken);
                           })
                          .WithMessage("Category with this name already exists.");
    }

    public static IRuleBuilderOptions<T, int> MustBeValidSortOrder<T>(this IRuleBuilder<T, int> ruleBuilder)
    {
        return ruleBuilder.GreaterThanOrEqualTo(CategoryConstants.MinSortOrder)
                          .WithMessage($"Sort order must be greater than or equal to {CategoryConstants.MinSortOrder}.");
    }

    public static IRuleBuilderOptions<T, IEnumerable<TItem>> MustHaveValidUniqueIds<T, TItem>(this IRuleBuilder<T, IEnumerable<TItem>> ruleBuilder, IMenuDbContext menuDbContext, Func<IEnumerable<TItem>, IEnumerable<Guid>> idSelector)
    {
        static async Task<bool> BeExistingIds(IEnumerable<Guid> ids, IMenuDbContext menuDbContext, CancellationToken cancellationToken)
        {
            IEnumerable<Category> existingIds = await menuDbContext.Category.Query().GetByIds(ids.Select(id => new CategoryId(id)), cancellationToken);

            return existingIds.Count() == ids.Count();
        }

        return ruleBuilder.Must(items => idSelector(items).Distinct().Count() == items.Count())
                            .WithMessage("IDs must be unique within the request.")
                          .MustAsync(async (items, cancellationToken) => await BeExistingIds(idSelector(items), menuDbContext, cancellationToken))
                            .WithMessage("Some IDs do not exist.");
    }
}
