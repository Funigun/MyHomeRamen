using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage;

public record struct GetProductByIdForManageQuery : IRequestId<GetProductByIdForManageQuery>, IRequest<GetProductByIdForManageResponse>
{
    public Guid Id { get; set; }
}
