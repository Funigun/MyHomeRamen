using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Features.Menu.Features.Products.UpdateProduct;

public sealed class UpdateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardPut<UpdateProductResponse>("api/menu/products/{id}", HandleAsync)
            .WithName("UpdateProductEndpoint")
            .WithTags("Products")
            .WithDescription("Updates the name, description, price, category, and ingredients of an existing product.")
            .RequireAuthorization("RestaurantManager");
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateProductRequest request,
        [FromServices] ICommandHandler<UpdateProductCommand, UpdateProductResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateProductCommand command = new(new ProductId(id), request);
        UpdateProductResponse response = await handler.Handle(command, cancellationToken);

        return Results.Ok(response);
    }
}
