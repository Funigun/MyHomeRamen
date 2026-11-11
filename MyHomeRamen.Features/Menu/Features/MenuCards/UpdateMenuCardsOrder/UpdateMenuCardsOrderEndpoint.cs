using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.MenuCards.UpdateMenuCardsOrder;

public sealed class UpdateMenuCardsOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardPut<UpdateMenuCardsOrderCommand>("api/menu/cards/order", HandleAsync)
            .WithName("UpdateMenuCardsOrderEndpoint")
            .WithTags("Menu Cards")
            .WithDescription("Updates the persisted sort order of menu cards for the selected restaurant.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] UpdateMenuCardsOrderRequest request,
        [FromServices] IRequestHandler<UpdateMenuCardsOrderCommand, Unit> handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(new UpdateMenuCardsOrderCommand(request), cancellationToken);
        return Results.NoContent();
    }
}
