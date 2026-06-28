using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Features.Common.Endpoints.Models;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductsForManage;

public sealed record GetProductsForManageQuery(PageParameters PageParameters, GetProductsForManageRequest Request) : IQuery<GetProductsForManageResponse>;
