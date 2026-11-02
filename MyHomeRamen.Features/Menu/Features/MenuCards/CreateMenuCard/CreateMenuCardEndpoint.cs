using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.CreateMenuCard;

public sealed record CreateMenuCardRequest(string Name);

public sealed record CreateMenuCardResponse(Guid Id);

public sealed class CreateMenuCardEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateMenuCardResponse>("api/menu/cards", HandleAsync)
            .WithName("CreateMenuCardEndpoint")
            .WithTags("Menu Cards")
            .WithDescription("Creates a menu card for the current restaurant.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateMenuCardRequest request,
        [FromServices] IRequestHandler<CreateMenuCardCommand, CreateMenuCardResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateMenuCardResponse response = await handler.Handle(
            new CreateMenuCardCommand(request.Name),
            cancellationToken);
        return Results.Created($"/api/menu/cards/{response.Id}", response);
    }
}
