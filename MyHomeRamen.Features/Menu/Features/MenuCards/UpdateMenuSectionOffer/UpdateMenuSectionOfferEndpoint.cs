using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.UpdateMenuSectionOffer;

public sealed record UpdateMenuSectionOfferRequest(int SortOrder);

public sealed record UpdateMenuSectionOfferResponse(Guid Id);

public sealed class UpdateMenuSectionOfferEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPut<UpdateMenuSectionOfferResponse>(
                "api/menu/cards/{menuCardId:guid}/sections/{sectionId:guid}/offers/{itemId:guid}",
                HandleAsync)
            .WithName("UpdateMenuSectionOfferEndpoint")
            .WithTags("Menu Sections")
            .WithDescription("Updates the order of an offer within a menu section.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid menuCardId,
        [FromRoute] Guid sectionId,
        [FromRoute] Guid itemId,
        [FromBody] UpdateMenuSectionOfferRequest request,
        [FromServices] IRequestHandler<UpdateMenuSectionOfferCommand, UpdateMenuSectionOfferResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateMenuSectionOfferResponse response = await handler.Handle(
            new UpdateMenuSectionOfferCommand(menuCardId, sectionId, itemId, request.SortOrder),
            cancellationToken);
        return Results.Ok(response);
    }
}
