using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Menu.Categories.Requests;

namespace MyHomeRamen.Features.Menu.Features.Categories.UpdateCategoriesOrder;

public sealed record UpdateCategoriesOrderCommand(UpdateCategoriesOrderRequest UpdateCategoriesOrderRequest) : ICommand;
