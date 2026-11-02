using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.RestaurantIngredientPrices.CreateRestaurantIngredientPrice;

public sealed record CreateRestaurantIngredientPriceRequest(Guid IngredientId, decimal Price);

public sealed record CreateRestaurantIngredientPriceResponse(Guid Id);

public sealed class CreateRestaurantIngredientPriceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateRestaurantIngredientPriceResponse>(
                "api/menu/ingredient-prices",
                HandleAsync)
            .WithName("CreateRestaurantIngredientPriceEndpoint")
            .WithTags("Ingredient Prices")
            .WithDescription("Sets an extra ingredient price for the current restaurant.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateRestaurantIngredientPriceRequest request,
        [FromServices] IRequestHandler<CreateRestaurantIngredientPriceCommand, CreateRestaurantIngredientPriceResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateRestaurantIngredientPriceResponse response = await handler.Handle(
            new CreateRestaurantIngredientPriceCommand(request.IngredientId, request.Price),
            cancellationToken);
        return Results.Created($"/api/menu/ingredient-prices/{request.IngredientId}", response);
    }
}
