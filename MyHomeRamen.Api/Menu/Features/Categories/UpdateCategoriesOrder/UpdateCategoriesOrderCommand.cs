using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Categories.Requests;

namespace MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder;

public sealed record UpdateCategoriesOrderCommand(UpdateCategoriesOrderRequest UpdateCategoriesOrderRequest) : IRequest;
