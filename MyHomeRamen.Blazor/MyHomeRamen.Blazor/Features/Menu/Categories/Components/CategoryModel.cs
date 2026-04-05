using MyHomeRamen.Blazor.Features.Menu.Categories.Requests;
using MyHomeRamen.Blazor.Features.Menu.Common.Models;

namespace MyHomeRamen.Blazor.Features.Menu.Categories.Components;

public sealed class CategoryModel
{
    public string Name { get; set; } = string.Empty;

    public CategoryType CategoryType { get; set; }

    public CreateCategoryRequest ToCreateRequest()
    {
        return new CreateCategoryRequest(Name, CategoryType);
    }
}
