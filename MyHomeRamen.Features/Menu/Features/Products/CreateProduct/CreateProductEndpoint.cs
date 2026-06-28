using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Features.Menu.Features.Products.CreateProduct;

public sealed class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateProductResponse>("api/menu/products", HandleAsync)
                       .WithName("CreateProductEndpoint")
                       .WithTags("Products")
                       .WithDescription("Handles Create Product operations.")
                       .RequireAuthorization("RestaurantManager");
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateProductRequest request,
        [FromServices] ICommandHandler<CreateProductCommand, CreateProductResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateProductCommand command = new(request);
        CreateProductResponse response = await handler.Handle(command, cancellationToken);

        return Results.Created($"/api/menu/products/{response.Id}", response);
    }
}
