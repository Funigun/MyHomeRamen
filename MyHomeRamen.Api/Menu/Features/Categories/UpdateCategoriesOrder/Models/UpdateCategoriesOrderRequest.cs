using MyHomeRamen.Api.Common.Endpoint.Models;

namespace MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder.Models;

public sealed record UpdateCategoriesOrderRequest(
    List<CategoryOrderItemDto> Items) : IRequest;
