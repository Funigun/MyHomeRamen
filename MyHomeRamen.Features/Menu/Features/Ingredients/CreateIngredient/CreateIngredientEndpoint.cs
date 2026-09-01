using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.CreateIngredient;

public sealed record CreateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);

public sealed record CreateIngredientResponse(Guid Id);

public sealed class CreateIngredientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateIngredientResponse>("api/menu/ingredients", HandleAsync)
                       .WithName("CreateIngredientEndpoint")
                       .WithTags("Ingredients")
                       .WithDescription("Handles Create Ingredient operations.")
                       .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateIngredientRequest request,
        [FromServices] IRequestHandler<CreateIngredientCommand, CreateIngredientResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateIngredientCommand command = new(request);
        CreateIngredientResponse response = await handler.Handle(command, cancellationToken);

        return Results.Created($"/api/menu/ingredients/{response.Id}", response);
    }
}
