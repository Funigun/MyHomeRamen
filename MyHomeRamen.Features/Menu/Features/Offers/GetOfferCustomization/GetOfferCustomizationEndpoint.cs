using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Offers.GetOfferCustomization;

public sealed record GetOfferCustomizationResponse(
    Guid OfferId,
    Guid ProductId,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    IEnumerable<OfferIngredientDto> BaseIngredients,
    IEnumerable<OfferIngredientDto> AvailableExtras);

public sealed class GetOfferCustomizationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetOfferCustomizationResponse>(
                "api/menu/offers/{id:guid}",
                HandleAsync)
            .WithName("GetOfferCustomizationEndpoint")
            .WithTags("Offers")
            .WithDescription("Returns an active offer and its available priced extras.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] IRequestHandler<GetOfferCustomizationQuery, GetOfferCustomizationResponse?> handler,
        CancellationToken cancellationToken)
    {
        GetOfferCustomizationResponse? response = await handler.Handle(
            new GetOfferCustomizationQuery(id),
            cancellationToken);
        return response is null ? Results.NotFound() : Results.Ok(response);
    }
}
