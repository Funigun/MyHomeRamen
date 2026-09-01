using FluentValidation;
using MyHomeRamen.Domain.Common.Product;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Products.Common;

internal static class ProductValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustMeetProductNameLengthRequirements<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                            .WithMessage("Product name cannot be empty.")

                          .MinimumLength(ProductConstants.MinNameLength)
                            .WithMessage($"Product name minimum length is {ProductConstants.MinNameLength}.")

                          .MaximumLength(ProductConstants.MaxNameLength)
                            .WithMessage($"Product name maximum length is {ProductConstants.MaxNameLength}.");
    }

    public static IRuleBuilderOptions<T, string> MustMeetProductDescriptionLengthRequirements<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                            .WithMessage("Product description cannot be empty.")

                          .MinimumLength(ProductConstants.MinDescriptionLength)
                            .WithMessage($"Product description minimum length is {ProductConstants.MinDescriptionLength}.")

                          .MaximumLength(ProductConstants.MaxDescriptionLength)
                            .WithMessage($"Product description maximum length is {ProductConstants.MaxDescriptionLength}.");
    }

    public static IRuleBuilderOptions<T, decimal> MustBeValidProductPrice<T>(this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder.GreaterThanOrEqualTo(ProductConstants.MinPrice)
                            .WithMessage($"Product price cannot be less than or equal to {ProductConstants.MinPrice}.")

                          .LessThanOrEqualTo(ProductConstants.MaxPrice)
                            .WithMessage($"Product price cannot be greater than or equal to {ProductConstants.MaxPrice}.");
    }

    public static IRuleBuilderOptions<T, string> MustNotExceedProductNameLength<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.MaximumLength(ProductConstants.MaxNameLength)
                            .WithMessage($"Name must not exceed {ProductConstants.MaxNameLength} characters.");
    }

    public static IRuleBuilderOptions<T, Guid> MustBeValidProductId<T>(this IRuleBuilder<T, Guid> ruleBuilder, IMenuDbContext dbContext)
    {
        return ruleBuilder.NotEmpty()
                            .WithMessage("Product ID must not be empty.")

                          .MustAsync(async (id, cancellationToken) => await dbContext.Product.Exists(p => p.Id == (ProductId)id, cancellationToken))
                            .WithMessage("Product with the specified ID does not exist.");
    }

    public static IRuleBuilderOptions<T, string> MustHaveUniqueProductName<T>(this IRuleBuilder<T, string> ruleBuilder, IMenuDbContext dbContext)
    {
        return ruleBuilder.MustAsync(async (name, cancellationToken) => await dbContext.Product.Query().IsProductNameUnique(name, cancellationToken))
                            .WithMessage("Product with same name already exists");
    }

    public static IRuleBuilderOptions<T, TCommand> MustHaveUniqueProductNameExcluding<T, TCommand>(this IRuleBuilder<T, TCommand> ruleBuilder, IMenuDbContext dbContext, Func<TCommand, string> nameSelector, Func<TCommand, ProductId> idSelector)
    {
        return ruleBuilder.MustAsync(async (command, cancellationToken) => await dbContext.Product.Query().IsProductNameUniqueExcluding(nameSelector(command), idSelector(command), cancellationToken))
                            .WithMessage("Product with this name already exists.");
    }

    public static IRuleBuilderOptions<T, Guid> MustBeExistingProductCategory<T>(this IRuleBuilder<T, Guid> ruleBuilder, IMenuDbContext dbContext)
    {
        return ruleBuilder.NotEmpty()
                            .WithMessage("Category ID must not be empty.")

                          .MustAsync(async (id, cancellationToken) => await dbContext.Category.Exists(c => c.Id == new CategoryId(id), cancellationToken))
                            .WithMessage("Category does not exist.");
    }

    public static IRuleBuilderOptions<T, Guid> MustBeProductCategoryType<T>(this IRuleBuilder<T, Guid> ruleBuilder, IMenuDbContext dbContext)
    {
        return ruleBuilder.MustAsync(async (id, cancellationToken) => await dbContext.Category.Query().IsProductCategoryType(new CategoryId(id), cancellationToken))
                            .WithMessage("Category must be a product category.");
    }

    public static IRuleBuilderOptions<T, IEnumerable<Guid>> MustContainIngredients<T>(this IRuleBuilder<T, IEnumerable<Guid>> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                            .WithMessage("At least one ingredient must be selected.");
    }

    public static IRuleBuilderOptions<T, IEnumerable<Guid>> MustContainExistingCustomIngredients<T>(this IRuleBuilder<T, IEnumerable<Guid>> ruleBuilder, IMenuDbContext dbContext)
    {
        return ruleBuilder.MustAsync(async (ids, cancellationToken) =>
                           {
                               if (!ids.Any())
                               {
                                   return true;
                               }

                               IEnumerable<IngredientId> customIngredientIds = ids.Distinct().Select(id => (IngredientId)id);
                               IEnumerable<Ingredient> found = await dbContext.Ingredient.Load().ByIds(customIngredientIds, cancellationToken);

                               return found.Count() == ids.Distinct().Count();
                           })
                          .WithMessage("One or more custom ingredient IDs do not exist.");
    }

    public static IRuleBuilderOptions<T, TCommand> MustHaveDistinctIngredientIds<T, TCommand>(this IRuleBuilder<T, TCommand> ruleBuilder, Func<TCommand, IEnumerable<Guid>> ingredientIdsSelector, Func<TCommand, IEnumerable<Guid>> customIngredientIdsSelector)
    {
        return ruleBuilder.Must(command => !ingredientIdsSelector(command).Intersect(customIngredientIdsSelector(command)).Any())
                            .WithMessage("Ingredient IDs and custom ingredient IDs must be unique across both collections.");
    }
}
