using MyHomeRamen.Common.Contracts.Menu.Categories.Requests;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;
using MyHomeRamen.Features.Common.Endpoints.Command;

namespace MyHomeRamen.Api.Menu.Features.Categories.CreateCategory;

public sealed record CreateCategoryCommand(CreateCategoryRequest CreateCategoryRequest) : ICommand<CreateCategoryResponse>;
