using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.UpdateMenuCard;

public sealed record UpdateMenuCardRequest(string Name, bool IsActive);

public sealed record UpdateMenuCardResponse(Guid Id);

public sealed class UpdateMenuCardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPut<UpdateMenuCardResponse>("api/menu/cards/{id:guid}", HandleAsync)
            .WithName("UpdateMenuCardEndpoint")
            .WithTags("Menu Cards")
            .WithDescription("Updates a menu card.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateMenuCardRequest request,
        [FromServices] IRequestHandler<UpdateMenuCardCommand, UpdateMenuCardResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateMenuCardResponse response = await handler.Handle(
            new UpdateMenuCardCommand(id, request.Name, request.IsActive),
            cancellationToken);
        return Results.Ok(response);
    }
}
