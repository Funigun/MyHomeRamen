using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage.Models;

public record struct GetProductByIdForManageRequest : IRequestId<GetProductByIdForManageRequest>, IRequest<GetProductByIdForManageResponse>
{
    public Guid Id { get; set; }
}
