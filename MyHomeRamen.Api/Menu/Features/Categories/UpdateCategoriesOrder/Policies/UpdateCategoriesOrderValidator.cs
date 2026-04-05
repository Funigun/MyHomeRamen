using FluentValidation;
using MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder.Models;
using MyHomeRamen.Common.Contracts.Menu.Categories;

namespace MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder.Policies;

public sealed class UpdateCategoriesOrderValidator : AbstractValidator<UpdateCategoriesOrderRequest>
{
    public UpdateCategoriesOrderValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Categories list must not be empty.")
            .Must(HaveUniqueIds).WithMessage("Category IDs must be unique within the request.");

        RuleForEach(x => x.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.Id)
                    .NotEmpty().WithMessage("Category ID must not be empty.");

                item.RuleFor(x => x.SortOrder)
                    .SetValidator(new CategorySortOrderValidator());
            });
    }

    private static bool HaveUniqueIds(IEnumerable<CategoryOrderItemDto> items)
    {
        return items.Select(i => i.Id).Distinct().Count() == items.Count();
    }
}
