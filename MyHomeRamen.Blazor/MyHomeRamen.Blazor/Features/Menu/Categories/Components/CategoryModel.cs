using MyHomeRamen.Blazor.Features.Menu.Common.Models;
using MyHomeRamen.Blazor.Features.Menu.Common.Services.Contracts.Categories.Requests;

namespace MyHomeRamen.Blazor.Features.Menu.Categories.Components;

public sealed class CategoryModel
{
    public string Name { get; set; } = string.Empty;

    public CategoryType CategoryType { get; set; }

    public CreateCategoryRequest ToCreateRequest()
    {
        return new CreateCategoryRequest(Name, (int)CategoryType);
    }
}
