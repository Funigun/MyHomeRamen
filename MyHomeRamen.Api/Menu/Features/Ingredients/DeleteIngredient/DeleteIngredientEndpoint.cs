using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient;

public sealed class DeleteIngredientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete<DeleteIngredientCommand>("api/menu/ingredients/{id}", HandleAsync)
                       .WithName("DeleteIngredientEndpoint")
                       .WithTags("Ingredients")
                       .WithDescription("Deletes an ingredient by its ID. Validates that the ingredient exists and is not used by any product.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(DeleteIngredientCommand id, [FromServices] ICommandHandler<DeleteIngredientCommand, IResult> handler, CancellationToken cancellationToken)
    {
        return await handler.Handle(id, cancellationToken);
    }
}
