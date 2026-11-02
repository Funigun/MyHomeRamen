using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.UpdateRestaurantIngredientPrice;

public sealed record UpdateRestaurantIngredientPriceRequest(decimal Price);

public sealed record UpdateRestaurantIngredientPriceResponse(Guid IngredientId);

public sealed class UpdateRestaurantIngredientPriceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPut<UpdateRestaurantIngredientPriceResponse>(
                "api/menu/ingredient-prices/{ingredientId:guid}",
                HandleAsync)
            .WithName("UpdateRestaurantIngredientPriceEndpoint")
            .WithTags("Ingredient Prices")
            .WithDescription("Updates an extra ingredient price for the current restaurant.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid ingredientId,
        [FromBody] UpdateRestaurantIngredientPriceRequest request,
        [FromServices] IRequestHandler<UpdateRestaurantIngredientPriceCommand, UpdateRestaurantIngredientPriceResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateRestaurantIngredientPriceResponse response = await handler.Handle(
            new UpdateRestaurantIngredientPriceCommand(ingredientId, request.Price),
            cancellationToken);
        return Results.Ok(response);
    }
}
