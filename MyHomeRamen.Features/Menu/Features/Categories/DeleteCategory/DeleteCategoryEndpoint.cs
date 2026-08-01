using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Endpoints.Command;

namespace MyHomeRamen.Features.Menu.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete("api/menu/categories/{id}", HandleAsync)
                       .WithName("DeleteCategoryEndpoint")
                       .WithTags("Categories")
                       .WithDescription("Handles Delete Category operations.")
                       .RequireAuthorization("RestaurantManager");
    }

    private static async Task<IResult> HandleAsync([FromRoute] Guid id, [FromServices] ICommandHandler<DeleteCategoryCommand> handler, CancellationToken cancellationToken)
    {
        DeleteCategoryCommand command = new(id);
        await handler.Handle(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
