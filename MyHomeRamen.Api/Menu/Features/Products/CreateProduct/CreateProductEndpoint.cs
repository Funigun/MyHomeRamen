using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.CreateProduct.Models;
using MyHomeRamen.Domain.Menu.Users;

namespace MyHomeRamen.Api.Menu.Features.Products.CreateProduct;

public sealed class CreateProductEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu.Products";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardAuthenticatedPost<CreateProductRequest, CreateProductResponse>(string.Empty, HandleAsync)
                       .WithName("CreateProductEndpoint")
                       .WithDescription("Handles Create Product operations.")
                       .RequireAuthorization(RoleConstants.Admin);
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
