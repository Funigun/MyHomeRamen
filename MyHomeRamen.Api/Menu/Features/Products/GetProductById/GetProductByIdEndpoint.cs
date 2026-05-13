using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductById;

public sealed class GetProductByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedGet<GetProductByIdQuery, GetProductByIdResponse>("api/menu/products/{id}", HandleAsync)
            .WithName("GetProductByIdEndpoint")
            .WithTags("Products")
            .WithDescription("Returns the full public-facing details of a single product including its base and custom ingredients.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        GetProductByIdQuery id,
        [FromServices] IRequestHandler<GetProductByIdQuery, GetProductByIdResponse> handler,
        CancellationToken cancellationToken)
    {
        GetProductByIdResponse response = await handler.Handle(id, cancellationToken);
        return Results.Ok(response);
    }
}
