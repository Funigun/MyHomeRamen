using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Categories.Common;

namespace MyHomeRamen.Features.Menu.Features.Categories.UpdateCategoriesOrder;

public sealed record UpdateCategoriesOrderCommand(UpdateCategoriesOrderRequest Request) : ICommand;

public sealed class UpdateCategoriesOrderValidator : AbstractValidator<UpdateCategoriesOrderCommand>
{
    public UpdateCategoriesOrderValidator(IMenuDbContext menuDbContext)
    {
        RuleFor(x => x.Request.Items)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Items collection must not be empty.")
            .MustHaveValidUniqueIds(menuDbContext, x => x.Select(item => item.Id));

        RuleForEach(x => x.Request.Items)
            .ChildRules(item =>
            {
                item.RuleFor(x => x.SortOrder)
                    .MustBeValidSortOrder();
            });
    }
}

public sealed class UpdateCategoriesOrderHandler(IMenuDbContext dbContext) : ICommandHandler<UpdateCategoriesOrderCommand>
{
    public async Task Handle(UpdateCategoriesOrderCommand command, CancellationToken cancellationToken)
    {
        UpdateCategoriesOrderRequest request = command.Request;
        IEnumerable<CategoryId> ids = request.Items.Select(i => (CategoryId)i.Id);

        IEnumerable<Category> categories = await dbContext.Category.Specification().ByIds(ids, cancellationToken);

        await ReorderCategories(categories, request, cancellationToken);
    }

    private async Task ReorderCategories(IEnumerable<Category> categories, UpdateCategoriesOrderRequest request, CancellationToken cancellationToken)
    {
        Dictionary<CategoryId, Category> categoryMap = categories.ToDictionary(c => c.Id);

        foreach (CategoryOrderItemDto item in request.Items)
        {
            if (categoryMap.TryGetValue(item.Id, out Category? category))
            {
                category.UpdateSortOrder(item.SortOrder);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
