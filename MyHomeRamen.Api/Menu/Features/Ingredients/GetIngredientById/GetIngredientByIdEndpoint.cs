using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientById;

public sealed class GetIngredientByIdEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetIngredientByIdResponse>("ingredients/{id}", HandleAsync)
            .WithName("GetIngredientByIdEndpoint")
            .WithDescription("Returns the details of a single ingredient by its ID.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        GetIngredientByIdRequest id,
        [FromServices] IRequestHandler<GetIngredientByIdRequest, GetIngredientByIdResponse> handler,
        CancellationToken cancellationToken)
    {
        GetIngredientByIdResponse response = await handler.Handle(id, cancellationToken);
        return Results.Ok(response);
    }
}
