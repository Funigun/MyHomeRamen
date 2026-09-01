using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.DeleteIngredient;

public sealed class DeleteIngredientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete("api/menu/ingredients/{id}", HandleAsync)
                       .WithName("DeleteIngredientEndpoint")
                       .WithTags("Ingredients")
                       .WithDescription("Deletes an ingredient by its ID. Validates that the ingredient exists and is not used by any product.")
                       .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
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

