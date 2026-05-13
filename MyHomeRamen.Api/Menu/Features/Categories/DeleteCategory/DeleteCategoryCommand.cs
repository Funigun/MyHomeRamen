using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory;

public record struct DeleteCategoryCommand : IRequestId<DeleteCategoryCommand>, IRequest<IResult>
{
    public Guid Id { get; set; }
}
