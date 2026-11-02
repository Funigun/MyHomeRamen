using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCardOptions;

public sealed record GetMenuCardOptionsResponse(IEnumerable<MenuCardOptionDto> MenuCards);

public sealed class GetMenuCardOptionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetMenuCardOptionsResponse>(
                "api/menu/cards/options",
                HandleAsync)
            .WithName("GetMenuCardOptionsEndpoint")
            .WithTags("Menu Cards")
            .WithDescription("Returns active menu cards at active restaurants.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] Guid? restaurantId,
        [FromServices] IRequestHandler<GetMenuCardOptionsQuery, GetMenuCardOptionsResponse> handler,
        CancellationToken cancellationToken)
    {
        GetMenuCardOptionsResponse response = await handler.Handle(
            new GetMenuCardOptionsQuery(restaurantId),
            cancellationToken);
        return Results.Ok(response);
    }
}
