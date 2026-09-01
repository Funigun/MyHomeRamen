using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductsByCategory;

public sealed record GetProductsByCategoryRequest(Guid CategoryId);

public sealed record GetProductsByCategoryResponse(IEnumerable<ProductByCategoryDto> Products);

public sealed record ProductByCategoryDto(
    Guid Id,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    IEnumerable<ProductIngredientDto> Ingredients);

public sealed record ProductIngredientDto(Guid Id, string Name);

public sealed class GetProductsByCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetProductsByCategoryResponse>("api/menu/products", HandleAsync)
            .WithName("GetProductsByCategoryEndpoint")
            .WithTags("Products")
            .WithDescription("Returns all products for a given category.")
            .AllowAnonymous();
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetProductsByCategoryRequest request,
        [FromServices] IRequestHandler<GetProductsByCategoryQuery, GetProductsByCategoryResponse> handler,
        CancellationToken cancellationToken)
    {
        GetProductsByCategoryQuery query = new(request);
        GetProductsByCategoryResponse response = await handler.Handle(query, cancellationToken);

        return Results.Ok(response);
    }
}
