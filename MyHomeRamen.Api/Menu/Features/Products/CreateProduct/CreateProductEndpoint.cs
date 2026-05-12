using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Products.CreateProduct;

public sealed class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedPost<CreateProductRequest, CreateProductResponse>("api/menu/products", HandleAsync)
                       .WithName("CreateProductEndpoint")
                       .WithTags("Products")
                       .WithDescription("Handles Create Product operations.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateProductRequest request, 
        [FromServices] IRequestHandler<CreateProductRequest, Guid> handler,
        CancellationToken cancellationToken)
    {
        Guid id = await handler.Handle(request, cancellationToken);
        CreateProductResponse response = new(id);

        return Results.Created($"/api/menu/products/{id}", response);
    }
}
