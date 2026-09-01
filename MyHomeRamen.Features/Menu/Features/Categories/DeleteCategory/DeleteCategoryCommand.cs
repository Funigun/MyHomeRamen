using FluentValidation;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Categories.DeleteCategory;

public record DeleteCategoryCommand(DeleteCategoryRequest Request) : ICommand;

public sealed class DeleteCategoryAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<DeleteCategoryCommand>
{
    public async Task<bool> Authorize(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        return currentUser.CanDeleteCategory();
    }
}

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

public sealed class DeleteCategoryHandler(IMenuDbContext dbContext) : IRequestHandler<DeleteCategoryCommand, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommand command, CancellationToken cancellationToken)
    {
        Category category = await dbContext.Category.Load().ById((CategoryId)command.Request.Id, cancellationToken);

        dbContext.Category.Delete(category);

        List<Category> remaining = (await dbContext.Category.Load().GetRemainingForResequencing(category.CategoryType, category.Id, cancellationToken)).ToList();

        for (int i = 0; i < remaining.Count; i++)
        {
            remaining[i].UpdateSortOrder(i + 1);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
