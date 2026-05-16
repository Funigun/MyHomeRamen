using MyHomeRamen.Api.Common.Endpoint.Pipeline;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory;

public record struct DeleteCategoryCommand : ICommand<IResult>
{
    public Guid Id { get; set; }
}
