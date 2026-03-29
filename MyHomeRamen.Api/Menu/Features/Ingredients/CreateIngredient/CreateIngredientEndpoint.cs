using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.CreateIngredient;

public sealed class CreateIngredientEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedPost<CreateIngredientRequest, CreateIngredientResponse>("ingredients", HandleAsync)
                       .WithName("CreateIngredientEndpoint")
                       .WithDescription("Handles Create Ingredient operations.")
                       .RequireAuthorization(AuthorizationConfiguration.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateIngredientRequest request,
        [FromServices] IRequestHandler<CreateIngredientRequest, Guid> handler,
        CancellationToken cancellationToken)
    {
        Guid id = await handler.Handle(request, cancellationToken);
        CreateIngredientResponse response = new(id);

        return Results.Created($"/api/menu/ingredients/{id}", response);
    }
}
