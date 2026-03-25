using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Categories.CreateCategory.Models;

public sealed record CreateCategoryRequest(
    string Name,
    int CategoryType) : IRequest<Guid>;
