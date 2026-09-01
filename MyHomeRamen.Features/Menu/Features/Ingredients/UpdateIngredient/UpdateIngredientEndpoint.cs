using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.UpdateIngredient;

public sealed record UpdateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);

public sealed record UpdateIngredientResponse(Guid Id);

public sealed class UpdateIngredientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardPut<UpdateIngredientResponse>("api/menu/ingredients/{id}", HandleAsync)
            .WithName("UpdateIngredientEndpoint")
            .WithTags("Ingredients")
            .WithDescription("Updates the name, description, price, and categories of an existing ingredient.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateIngredientRequest request,
        [FromServices] IRequestHandler<UpdateIngredientCommand, UpdateIngredientResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateIngredientCommand command = new(new(id), request);
        UpdateIngredientResponse response = await handler.Handle(command, cancellationToken);

        return Results.Ok(response);
    }
}
