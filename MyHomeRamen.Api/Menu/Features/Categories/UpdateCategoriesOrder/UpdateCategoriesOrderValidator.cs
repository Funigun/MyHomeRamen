using FluentValidation;
using MyHomeRamen.Common.Contracts.Menu.Categories.DTOs;
using MyHomeRamen.Common.Contracts.Menu.Categories.Validators;

namespace MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder;

public sealed class UpdateCategoriesOrderValidator : AbstractValidator<UpdateCategoriesOrderCommand>
{
    public UpdateCategoriesOrderValidator()
    {
        RuleFor(x => x.UpdateCategoriesOrderRequest.Items)
            .NotEmpty().WithMessage("Categories list must not be empty.")
            .Must(HaveUniqueIds).WithMessage("Category IDs must be unique within the request.");

        RuleForEach(x => x.UpdateCategoriesOrderRequest.Items)
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
