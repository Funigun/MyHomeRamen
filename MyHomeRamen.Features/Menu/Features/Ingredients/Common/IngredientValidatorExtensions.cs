using FluentValidation;
using MyHomeRamen.Domain.Common.Ingredient;
using MyHomeRamen.Domain.Menu.Ingredients;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.Common;

internal static class IngredientValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> MustMeetNameLengthRequirements<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                            .WithMessage("Ingredient name cannot be empty.")

                          .MinimumLength(IngredientConstants.MinNameLength)
                            .WithMessage($"Ingredient name minimum length is {IngredientConstants.MinNameLength}.")

                          .MaximumLength(IngredientConstants.MaxNameLength)
                            .WithMessage($"Ingredient name maximum length is {IngredientConstants.MaxNameLength}.");
    }

    public static IRuleBuilderOptions<T, string> MustMeetDescriptionLengthRequirements<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.NotEmpty()
                            .WithMessage("Ingredient description cannot be empty.")

                          .MinimumLength(IngredientConstants.MinDescriptionLength)
                            .WithMessage($"Ingredient description minimum length is {IngredientConstants.MinDescriptionLength}.")

                          .MaximumLength(IngredientConstants.MaxDescriptionLength)
                            .WithMessage($"Ingredient description maximum length is {IngredientConstants.MaxDescriptionLength}.");
    }

    public static IRuleBuilderOptions<T, decimal> MustBeValidIngredientPrice<T>(this IRuleBuilder<T, decimal> ruleBuilder)
    {
        return ruleBuilder.GreaterThanOrEqualTo(IngredientConstants.MinPrice)
                            .WithMessage($"Ingredient price cannot be less than or equal to {IngredientConstants.MinPrice}.")

                          .LessThanOrEqualTo(IngredientConstants.MaxPrice)
                            .WithMessage($"Ingredient price cannot be greater than or equal to {IngredientConstants.MaxPrice}.");
    }

    public static IRuleBuilderOptions<T, string> MustNotExceedIngredientNameLength<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder.MaximumLength(IngredientConstants.MaxNameLength)
                            .WithMessage($"Name must not exceed {IngredientConstants.MaxNameLength} characters.");
    }

    public static IRuleBuilderOptions<T, Guid> MustBeValidIngredientId<T>(this IRuleBuilder<T, Guid> ruleBuilder, IMenuDbContext dbContext)
    {
        return ruleBuilder.NotEmpty()
                            .WithMessage("Ingredient ID must not be empty.")

                          .MustAsync(async (id, cancellationToken) => await dbContext.Ingredient.Exists(i => i.Id == (IngredientId)id, cancellationToken))
                            .WithMessage("Ingredient with the specified ID does not exist.");
    }

    public static IRuleBuilderOptions<T, string> MustHaveUniqueIngredientName<T>(this IRuleBuilder<T, string> ruleBuilder, IMenuDbContext dbContext)
    {
        return ruleBuilder.MustAsync(async (name, cancellationToken) => await dbContext.Ingredient.Query().IsIngredientNameUnique(name, cancellationToken))
                            .WithMessage("Ingredient with this name already exists.");
    }

    public static IRuleBuilderOptions<T, TCommand> MustHaveUniqueIngredientNameExcluding<T, TCommand>(this IRuleBuilder<T, TCommand> ruleBuilder, IMenuDbContext dbContext, Func<TCommand, string> nameSelector, Func<TCommand, IngredientId> idSelector)
    {
        return ruleBuilder.MustAsync(async (command, cancellationToken) => await dbContext.Ingredient.Query().IsIngredientNameUniqueExcluding(nameSelector(command), idSelector(command), cancellationToken))
                            .WithMessage("Ingredient with this name already exists.");
    }

    public static IRuleBuilderOptions<T, Guid> MustNotBeUsedAsBaseIngredient<T>(this IRuleBuilder<T, Guid> ruleBuilder, IMenuDbContext dbContext)
    {
        return ruleBuilder.MustAsync(async (id, cancellationToken) => !await dbContext.Product.Query().IsIngredientUsedAsBaseByProduct((IngredientId)id, cancellationToken))
                            .WithMessage("Ingredient is used as a base ingredient by one or more products and cannot be deleted.");
    }

    public static IRuleBuilderOptions<T, Guid> MustNotBeUsedAsCustomIngredient<T>(this IRuleBuilder<T, Guid> ruleBuilder, IMenuDbContext dbContext)
    {
        return ruleBuilder.MustAsync(async (id, cancellationToken) => !await dbContext.Product.Query().IsIngredientUsedAsCustomByProduct((IngredientId)id, cancellationToken))
                            .WithMessage("Ingredient is used as an additional ingredient by one or more products and cannot be deleted.");
    }
}
