using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.UpdateProduct.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Products.UpdateProduct;

public sealed class UpdateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedPutWithResponse<UpdateProductRequest, UpdateProductResponse>(
                "api/menu/products/{id}", HandleAsync)
            .WithName("UpdateProductEndpoint")
            .WithTags("Products")
            .WithDescription("Updates the name, description, price, category, and ingredients of an existing product.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] UpdateProductRequestId id,
        [FromBody] UpdateProductRequest request,
        [FromServices] IRequestHandler<UpdateProductRequest, UpdateProductResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateProductResponse response = await handler.Handle(request with { Id = id.Id }, cancellationToken);

        return Results.Ok(response);
    }
}
