using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Categories.Common;

namespace MyHomeRamen.Features.Menu.Features.Categories.DeleteCategory;

public record DeleteCategoryCommand(DeleteCategoryRequest Request) : ICommand;

public sealed class DeleteCategoryValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryValidator(IMenuDbContext dbContext)
    {
        RuleFor(x => x.Request.Id)
            .Cascade(CascadeMode.Stop)
            .MustBeValidCategoryId(dbContext)
            .MustNotBeUsed(dbContext);
    }
}

public sealed class DeleteCategoryHandler(IMenuDbContext dbContext) : ICommandHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        Category category = await dbContext.Category.Load().ById((CategoryId)command.Request.Id, cancellationToken);

        dbContext.Category.Delete(category);

        List<Category> remaining = (await dbContext.Category.Load().GetRemainingForResequencing(category.CategoryType, category.Id, cancellationToken)).ToList();

        for (int i = 0; i < remaining.Count; i++)
        {
            remaining[i].UpdateSortOrder(i + 1);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
