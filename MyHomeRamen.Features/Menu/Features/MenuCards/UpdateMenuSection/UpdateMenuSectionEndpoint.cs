using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.UpdateMenuSection;

public sealed record UpdateMenuSectionRequest(string Name, int SortOrder);

public sealed record UpdateMenuSectionResponse(Guid Id);

public sealed class UpdateMenuSectionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPut<UpdateMenuSectionResponse>(
                "api/menu/cards/{menuCardId:guid}/sections/{sectionId:guid}",
                HandleAsync)
            .WithName("UpdateMenuSectionEndpoint")
            .WithTags("Menu Sections")
            .WithDescription("Updates a menu section.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid menuCardId,
        [FromRoute] Guid sectionId,
        [FromBody] UpdateMenuSectionRequest request,
        [FromServices] IRequestHandler<UpdateMenuSectionCommand, UpdateMenuSectionResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateMenuSectionResponse response = await handler.Handle(
            new UpdateMenuSectionCommand(menuCardId, sectionId, request.Name, request.SortOrder),
            cancellationToken);
        return Results.Ok(response);
    }
}
