using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage;

public record GetProductByIdForManageQuery(Guid Id) : IQuery<GetProductByIdForManageResponse>;
