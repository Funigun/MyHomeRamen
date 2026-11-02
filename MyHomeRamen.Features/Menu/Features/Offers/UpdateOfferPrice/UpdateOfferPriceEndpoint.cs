using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Offers.UpdateOfferPrice;

public sealed record UpdateOfferPriceRequest(decimal Price);

public sealed record UpdateOfferPriceResponse(Guid Id);

public sealed class UpdateOfferPriceEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPut<UpdateOfferPriceResponse>("api/menu/offers/{id:guid}", HandleAsync)
            .WithName("UpdateOfferPriceEndpoint")
            .WithTags("Offers")
            .WithDescription("Updates a restaurant offer price.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateOfferPriceRequest request,
        [FromServices] IRequestHandler<UpdateOfferPriceCommand, UpdateOfferPriceResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateOfferPriceResponse response = await handler.Handle(
            new UpdateOfferPriceCommand(id, request.Price),
            cancellationToken);
        return Results.Ok(response);
    }
}
