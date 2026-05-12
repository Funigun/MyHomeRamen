using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder;

public sealed class UpdateCategoriesOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedPut<UpdateCategoriesOrderRequest>("api/menu/categories/order", HandleAsync)
                       .WithName("UpdateCategoriesOrderEndpoint")
                       .WithTags("Categories")
                       .WithDescription("Updates the sort order of multiple categories in a single batch operation.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] UpdateCategoriesOrderRequest request,
        [FromServices] IRequestHandler<UpdateCategoriesOrderRequest> handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(request, cancellationToken);
        return Results.NoContent();
    }
}
