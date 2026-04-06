using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage.Models;

public sealed record GetIngredientsForManageRequest(string? Name, IEnumerable<Guid>? CategoryIds) : IRequest<GetIngredientsForManageResponse>
{
    public PageParameters PageParameters { get; set; }
};
