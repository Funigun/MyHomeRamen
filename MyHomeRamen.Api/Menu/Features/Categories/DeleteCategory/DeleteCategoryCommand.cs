using MyHomeRamen.Api.Common.Endpoint.Pipeline;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : ICommand<IResult>;
