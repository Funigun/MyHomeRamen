namespace MyHomeRamen.Api.Menu.Features.GetCategoriesOptions;

public interface ICategoryService
{
    Task<IReadOnlyCollection<CategoryOption>> GetCategoriesOptionsAsync(CancellationToken cancellationToken = default);
}
