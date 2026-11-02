using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Offers.CreateOffer;

public sealed record CreateOfferRequest(Guid ProductId, decimal Price);

public sealed record CreateOfferResponse(Guid Id);

public sealed class CreateOfferEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateOfferResponse>("api/menu/offers", HandleAsync)
            .WithName("CreateOfferEndpoint")
            .WithTags("Offers")
            .WithDescription("Creates a restaurant offer.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateOfferRequest request,
        [FromServices] IRequestHandler<CreateOfferCommand, CreateOfferResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateOfferResponse response = await handler.Handle(
            new CreateOfferCommand(request.ProductId, request.Price),
            cancellationToken);
        return Results.Created($"/api/menu/offers/{response.Id}", response);
    }
}
