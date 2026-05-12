using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory.Models;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory;

public sealed class GetProductsByCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedGet<GetProductsByCategoryRequest, IEnumerable<GetProductsByCategoryResponse>>("api/menu/products", HandleAsync)
            .WithName("GetProductsByCategoryEndpoint")
            .WithTags("Products")
            .WithDescription("Returns all products for a given category.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetProductsByCategoryRequest request,
        [FromServices] IRequestHandler<GetProductsByCategoryRequest, IEnumerable<GetProductsByCategoryResponse>> handler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetProductsByCategoryResponse> response = await handler.Handle(request, cancellationToken);
        return Results.Ok(response);
    }
}
