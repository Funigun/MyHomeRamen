using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.DeleteMenuSection;

public sealed class DeleteMenuSectionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete(
                "api/menu/cards/{menuCardId:guid}/sections/{sectionId:guid}",
                HandleAsync)
            .WithName("DeleteMenuSectionEndpoint")
            .WithTags("Menu Sections")
            .WithDescription("Removes a section from a menu card.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid menuCardId,
        [FromRoute] Guid sectionId,
        [FromServices] IRequestHandler<DeleteMenuSectionCommand, Unit> handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(new DeleteMenuSectionCommand(menuCardId, sectionId), cancellationToken);
        return Results.NoContent();
    }
}
