using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.RemoveRestaurantIngredientPrice;

public sealed class RemoveRestaurantIngredientPriceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete(
                "api/menu/ingredient-prices/{ingredientId:guid}",
                HandleAsync)
            .WithName("RemoveRestaurantIngredientPriceEndpoint")
            .WithTags("Ingredient Prices")
            .WithDescription("Removes an extra ingredient price at the current restaurant.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid ingredientId,
        [FromServices] IRequestHandler<RemoveRestaurantIngredientPriceCommand, Unit> handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(new RemoveRestaurantIngredientPriceCommand(ingredientId), cancellationToken);
        return Results.NoContent();
    }
}
