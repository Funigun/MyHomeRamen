using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory.Models;

public sealed class DeleteCategoryRequest : IRequestId<DeleteCategoryRequest>, IRequest<IResult>
{
    public Guid Id { get; set; }
}
