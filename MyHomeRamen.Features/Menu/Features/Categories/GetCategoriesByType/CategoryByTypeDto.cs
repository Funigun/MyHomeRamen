using MyHomeRamen.Domain.Menu.Categories;

namespace MyHomeRamen.Features.Menu.Features.Categories.GetCategoriesByType;

public sealed record CategoryByTypeDto(CategoryId Id, string Name, int SortOrder);
