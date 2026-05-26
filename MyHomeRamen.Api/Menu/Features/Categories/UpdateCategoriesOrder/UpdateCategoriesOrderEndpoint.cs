using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Menu.Categories.Requests;

namespace MyHomeRamen.Api.Menu.Features.Categories.UpdateCategoriesOrder;

public sealed class UpdateCategoriesOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPut<UpdateCategoriesOrderCommand>("api/menu/categories/order", HandleAsync)
                       .WithName("UpdateCategoriesOrderEndpoint")
                       .WithTags("Categories")
                       .WithDescription("Updates the sort order of multiple categories in a single batch operation.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] UpdateCategoriesOrderRequest request,
        [FromServices] ICommandHandler<UpdateCategoriesOrderCommand> handler,
        CancellationToken cancellationToken)
    {
        UpdateCategoriesOrderCommand command = new(request);
        await handler.Handle(command, cancellationToken);
        return Results.NoContent();
    }
}
