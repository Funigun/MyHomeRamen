using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.AddMenuSectionOffer;

public sealed record AddMenuSectionOfferRequest(Guid OfferId, int SortOrder);

public sealed record AddMenuSectionOfferResponse(Guid Id);

public sealed class AddMenuSectionOfferEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<AddMenuSectionOfferResponse>(
                "api/menu/cards/{menuCardId:guid}/sections/{sectionId:guid}/offers",
                HandleAsync)
            .WithName("AddMenuSectionOfferEndpoint")
            .WithTags("Menu Sections")
            .WithDescription("Adds an offer to a menu section.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid menuCardId,
        [FromRoute] Guid sectionId,
        [FromBody] AddMenuSectionOfferRequest request,
        [FromServices] IRequestHandler<AddMenuSectionOfferCommand, AddMenuSectionOfferResponse> handler,
        CancellationToken cancellationToken)
    {
        AddMenuSectionOfferResponse response = await handler.Handle(
            new AddMenuSectionOfferCommand(menuCardId, sectionId, request.OfferId, request.SortOrder),
            cancellationToken);
        return Results.Created(
            $"/api/menu/cards/{menuCardId}/sections/{sectionId}/offers/{response.Id}",
            response);
    }
}
