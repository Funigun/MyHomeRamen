using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete<DeleteCategoryRequest>("categories/{id}", HandleAsync)
                       .WithValidationFilter<DeleteCategoryRequest>()
                       .ProducesProblem(StatusCodes.Status400BadRequest)
                       .ProducesProblem(StatusCodes.Status409Conflict)
                       .WithName("DeleteCategoryEndpoint")
                       .WithDescription("Handles Delete Category operations.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        DeleteCategoryRequest request,
        [FromServices] IRequestHandler<DeleteCategoryRequest, IResult> handler,
        CancellationToken cancellationToken)
    {
        return await handler.Handle(request, cancellationToken);
    }
}
