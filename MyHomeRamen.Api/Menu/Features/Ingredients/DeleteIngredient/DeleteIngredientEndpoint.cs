using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient;

public sealed class DeleteIngredientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete("api/menu/ingredients/{id}", HandleAsync)
                       .WithName("DeleteIngredientEndpoint")
                       .WithTags("Ingredients")
                       .WithDescription("Deletes an ingredient by its ID. Validates that the ingredient exists and is not used by any product.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] ICommandHandler<DeleteIngredientCommand> handler,
        CancellationToken cancellationToken)
    {
        DeleteIngredientCommand command = new(id);
        await handler.Handle(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
