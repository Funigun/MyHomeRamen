using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.CreateMenuSection;

public sealed record CreateMenuSectionRequest(string Name, int SortOrder);

public sealed record CreateMenuSectionResponse(Guid Id);

public sealed class CreateMenuSectionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateMenuSectionResponse>(
                "api/menu/cards/{menuCardId:guid}/sections",
                HandleAsync)
            .WithName("CreateMenuSectionEndpoint")
            .WithTags("Menu Sections")
            .WithDescription("Creates a section in a menu card.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid menuCardId,
        [FromBody] CreateMenuSectionRequest request,
        [FromServices] IRequestHandler<CreateMenuSectionCommand, CreateMenuSectionResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateMenuSectionResponse response = await handler.Handle(
            new CreateMenuSectionCommand(menuCardId, request.Name, request.SortOrder),
            cancellationToken);
        return Results.Created($"/api/menu/cards/{menuCardId}/sections/{response.Id}", response);
    }
}
