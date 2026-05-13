using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsForManage;

public sealed record GetProductsForManageQuery(PageParameters PageParameters, GetProductsForManageRequest Request) : IRequest<GetProductsForManageResponse>;
