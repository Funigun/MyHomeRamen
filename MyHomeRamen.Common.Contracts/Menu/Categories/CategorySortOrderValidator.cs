using FluentValidation;

namespace MyHomeRamen.Common.Contracts.Menu.Categories;

public sealed class CategorySortOrderValidator : AbstractValidator<int>
{
    public const int MinSortOrder = 0;

    public CategorySortOrderValidator()
    {
        RuleFor(x => x)
            .GreaterThanOrEqualTo(MinSortOrder).WithMessage($"Sort order must be greater than or equal to {MinSortOrder}.");
    }
}
