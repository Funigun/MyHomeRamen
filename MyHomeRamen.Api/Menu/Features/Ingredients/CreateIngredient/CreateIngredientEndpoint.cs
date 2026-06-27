using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Requests;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Features.Common.Endpoints;

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
