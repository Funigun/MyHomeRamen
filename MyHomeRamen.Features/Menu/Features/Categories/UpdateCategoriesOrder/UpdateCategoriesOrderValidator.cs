using FluentValidation;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Categories.Common;

namespace MyHomeRamen.Features.Menu.Features.Categories.UpdateCategoriesOrder;

public sealed class UpdateCategoriesOrderValidator : AbstractValidator<UpdateCategoriesOrderCommand>
{
    public UpdateCategoriesOrderValidator(IMenuDbContext menuDbContext)
    {
        RuleFor(x => x.UpdateCategoriesOrderRequest.Items)
            .MustHaveValidUniqueIds(menuDbContext, x => x.Select(item => item.Id));

        RuleForEach(x => x.UpdateCategoriesOrderRequest.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.SortOrder)
                    .MustBeValidSortOrder();
            });
    }
}
