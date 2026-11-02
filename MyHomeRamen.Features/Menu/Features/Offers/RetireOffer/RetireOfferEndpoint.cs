using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Offers.RetireOffer;

public sealed class RetireOfferEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete("api/menu/offers/{id:guid}", HandleAsync)
            .WithName("RetireOfferEndpoint")
            .WithTags("Offers")
            .WithDescription("Retires a restaurant offer.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] IRequestHandler<RetireOfferCommand, Unit> handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(new RetireOfferCommand(id), cancellationToken);
        return Results.NoContent();
    }
}
