using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductById;

public sealed class GetProductByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetProductByIdResponse>("api/menu/products/{id}", HandleAsync)
            .WithName("GetProductByIdEndpoint")
            .WithTags("Products")
            .WithDescription("Returns the full public-facing details of a single product including its base and custom ingredients.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] IQueryHandler<GetProductByIdQuery, GetProductByIdResponse> handler,
        CancellationToken cancellationToken)
    {
        GetProductByIdQuery query = new(id);
        GetProductByIdResponse response = await handler.Handle(query, cancellationToken);
        return Results.Ok(response);
    }
}
