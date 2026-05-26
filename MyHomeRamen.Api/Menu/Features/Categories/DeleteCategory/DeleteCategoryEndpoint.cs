using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete("api/menu/categories/{id}", HandleAsync)
                       .WithName("DeleteCategoryEndpoint")
                       .WithTags("Categories")
                       .WithDescription("Handles Delete Category operations.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<DeleteCategoryCommand> handler,
        CancellationToken cancellationToken)
    {
        DeleteCategoryCommand command = new(id);
        await handler.Handle(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
