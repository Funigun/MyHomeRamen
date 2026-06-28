using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Features.Common.Endpoints.Models;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForManage;

public sealed record GetIngredientsForManageQuery(GetIngredientsForManageRequest Request, PageParameters PageParameters)
                   : IQuery<GetIngredientsForManageResponse>;
