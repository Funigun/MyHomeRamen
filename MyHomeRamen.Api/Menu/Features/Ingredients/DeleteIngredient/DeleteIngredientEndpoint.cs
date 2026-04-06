using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.DeleteIngredient;

public sealed class DeleteIngredientEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedDelete<DeleteIngredientRequest>("ingredients/{id}", HandleAsync)
                       .WithName("DeleteIngredientEndpoint")
                       .WithDescription("Deletes an ingredient by its ID. Validates that the ingredient exists and is not used by any product.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(DeleteIngredientRequest id, [FromServices] IRequestHandler<DeleteIngredientRequest, IResult> handler, CancellationToken cancellationToken)
    {
        return await handler.Handle(id, cancellationToken);
    }
}
