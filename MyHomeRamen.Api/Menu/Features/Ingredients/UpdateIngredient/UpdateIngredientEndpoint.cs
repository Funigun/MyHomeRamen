using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.UpdateIngredient;

public sealed class UpdateIngredientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardPut<UpdateIngredientResponse>("api/menu/ingredients/{id}", HandleAsync)
            .WithName("UpdateIngredientEndpoint")
            .WithTags("Ingredients")
            .WithDescription("Updates the name, description, price, and categories of an existing ingredient.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateIngredientRequest request,
        [FromServices] ICommandHandler<UpdateIngredientCommand, UpdateIngredientResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateIngredientCommand command = new(new(id), request);
        UpdateIngredientResponse response = await handler.Handle(command, cancellationToken);

        return Results.Ok(response);
    }
}
