using MyHomeRamen.Features.Common.Endpoints.Command;

namespace MyHomeRamen.Features.Menu.Features.Categories.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : ICommand;
