using MyHomeRamen.Features.Common.Endpoints.Command;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : ICommand;
