using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.ActivateMenuCard;

public sealed record ActivateMenuCardResponse(Guid MenuCardId);

public sealed class ActivateMenuCardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardPut<ActivateMenuCardResponse>(
                "api/menu/cards/{id:guid}/activate",
                HandleAsync)
            .WithName("ActivateMenuCardEndpoint")
            .WithTags("Menu Cards")
            .WithDescription("Activates an inactive menu card for its restaurant.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] IRequestHandler<ActivateMenuCardCommand, ActivateMenuCardResponse> handler,
        CancellationToken cancellationToken)
    {
        ActivateMenuCardResponse response = await handler.Handle(
            new ActivateMenuCardCommand(id),
            cancellationToken);
        return Results.Ok(response);
    }
}
