using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductByIdForManage;

public record struct GetProductByIdForManageQuery : IQuery<GetProductByIdForManageResponse>
{
    public Guid Id { get; set; }
}
