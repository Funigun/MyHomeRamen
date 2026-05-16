using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Categories.Requests;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;

namespace MyHomeRamen.Api.Menu.Features.Categories.CreateCategory;

public sealed record CreateCategoryCommand(CreateCategoryRequest CreateCategoryRequest) : ICommand<CreateCategoryResponse>;
