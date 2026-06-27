using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Products.Requests;
using MyHomeRamen.Common.Contracts.Menu.Products.Responses;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Api.Menu.Features.Products.GetProductsByCategory;

public sealed class GetProductsByCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<IEnumerable<GetProductsByCategoryResponse>>("api/menu/products", HandleAsync)
            .WithName("GetProductsByCategoryEndpoint")
            .WithTags("Products")
            .WithDescription("Returns all products for a given category.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetProductsByCategoryRequest request,
        [FromServices] IQueryHandler<GetProductsByCategoryQuery, IEnumerable<GetProductsByCategoryResponse>> handler,
        CancellationToken cancellationToken)
    {
        GetProductsByCategoryQuery query = new(request);
        IEnumerable<GetProductsByCategoryResponse> response = await handler.Handle(query, cancellationToken);

        return Results.Ok(response);
    }
}
