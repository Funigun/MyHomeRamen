using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.GetMenuCard;

public sealed record MenuCardResponse(
    Guid MenuCardId,
    Guid RestaurantId,
    string Name,
    IEnumerable<MenuSectionDto> Sections);

public sealed class GetMenuCardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<MenuCardResponse>("api/menu/cards/{id:guid}", HandleAsync)
            .WithName("GetMenuCardEndpoint")
            .WithTags("Menu Cards")
            .WithDescription("Returns an active menu card with its ordered sections and offers.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] IRequestHandler<GetMenuCardQuery, MenuCardResponse?> handler,
        CancellationToken cancellationToken)
    {
        MenuCardResponse? response = await handler.Handle(new GetMenuCardQuery(id), cancellationToken);
        return response is null ? Results.NotFound() : Results.Ok(response);
    }
}
