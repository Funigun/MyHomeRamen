using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient;

public sealed class CreateIngredientEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateIngredientResponse>("api/menu/ingredients", HandleAsync)
                       .WithName("CreateIngredientEndpoint")
                       .WithTags("Ingredients")
                       .WithDescription("Handles Create Ingredient operations.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateIngredientRequest request,
        [FromServices] ICommandHandler<CreateIngredientCommand, CreateIngredientResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateIngredientCommand command = new(request);
        CreateIngredientResponse response = await handler.Handle(command, cancellationToken);

        return Results.Created($"/api/menu/ingredients/{response.Id}", response);
    }
}
