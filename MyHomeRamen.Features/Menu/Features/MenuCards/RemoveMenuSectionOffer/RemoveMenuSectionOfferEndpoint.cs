using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.RemoveMenuSectionOffer;

public sealed class RemoveMenuSectionOfferEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete(
                "api/menu/cards/{menuCardId:guid}/sections/{sectionId:guid}/offers/{itemId:guid}",
                HandleAsync)
            .WithName("RemoveMenuSectionOfferEndpoint")
            .WithTags("Menu Sections")
            .WithDescription("Removes an offer from a menu section.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid menuCardId,
        [FromRoute] Guid sectionId,
        [FromRoute] Guid itemId,
        [FromServices] IRequestHandler<RemoveMenuSectionOfferCommand, Unit> handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(new RemoveMenuSectionOfferCommand(menuCardId, sectionId, itemId), cancellationToken);
        return Results.NoContent();
    }
}
