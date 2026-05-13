using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Products;

namespace MyHomeRamen.Api.Menu.Features.Products.UpdateProduct;

public sealed class UpdateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedPutWithResponse<UpdateProductCommand, UpdateProductResponse>(
                "api/menu/products/{id}", HandleAsync)
            .WithName("UpdateProductEndpoint")
            .WithTags("Products")
            .WithDescription("Updates the name, description, price, category, and ingredients of an existing product.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateProductRequest request,
        [FromServices] IRequestHandler<UpdateProductCommand, UpdateProductResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateProductCommand command = new(new ProductId(id), request);

        UpdateProductResponse response = await handler.Handle(command, cancellationToken);

        return Results.Ok(response);
    }
}
