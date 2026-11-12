using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.AddMenuSectionOffers;

public sealed record AddMenuSectionOffersRequest(IReadOnlyList<Guid> OfferIds);

public sealed record MenuSectionOfferAssignmentDto(Guid OfferId, Guid ListingId, int SortOrder);

public sealed record AddMenuSectionOffersResponse(IReadOnlyList<MenuSectionOfferAssignmentDto> Offers);

public sealed class AddMenuSectionOffersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<AddMenuSectionOffersResponse>(
                "api/menu/cards/{menuCardId:guid}/sections/{sectionId:guid}/offers/batch",
                HandleAsync)
            .WithName("AddMenuSectionOffersEndpoint")
            .WithTags("Menu Sections")
            .WithDescription("Adds multiple offers to a menu section atomically.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid menuCardId,
        [FromRoute] Guid sectionId,
        [FromBody] AddMenuSectionOffersRequest request,
        [FromServices] IRequestHandler<AddMenuSectionOffersCommand, AddMenuSectionOffersResponse> handler,
        CancellationToken cancellationToken)
    {
        AddMenuSectionOffersResponse response = await handler.Handle(
            new AddMenuSectionOffersCommand(menuCardId, sectionId, request.OfferIds),
            cancellationToken);
        return Results.Created(
            $"/api/menu/cards/{menuCardId}/sections/{sectionId}/offers/batch",
            response);
    }
}
