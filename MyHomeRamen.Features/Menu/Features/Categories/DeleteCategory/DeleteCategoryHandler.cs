using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Domain.Menu.Categories;
using MyHomeRamen.Features.Menu.Features.Categories.Common;
using MyHomeRamen.Features.Menu.Features.Abstractions;

namespace MyHomeRamen.Features.Menu.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryHandler(ICategoryRepository categoryRepository, IMenuUnitOfWork unitOfWork) : ICommandHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand id, CancellationToken cancellationToken)
    {
        Category category = await categoryRepository.Specification().ById((CategoryId)id.Id, cancellationToken);

        categoryRepository.Delete(category);

        List<Category> remaining = await categoryRepository.Specification().GetRemainingForResequencing(category.CategoryType, category.Id, cancellationToken);

        for (int i = 0; i < remaining.Count; i++)
        {
            remaining[i].UpdateSortOrder(i + 1);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
