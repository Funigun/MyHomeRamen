using MyHomeRamen.Blazor.Features.Menu.Common.Models;

namespace MyHomeRamen.Blazor.Features.Menu.Categories.Requests;

public sealed record CreateCategoryRequest(string Name, CategoryType CategoryType);
