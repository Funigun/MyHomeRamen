using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForManage;

public sealed record GetIngredientsForManageQuery(GetIngredientsForManageRequest Request) : IRequest<GetIngredientsForManageResponse>
{
    public PageParameters PageParameters { get; set; } = new();
}
