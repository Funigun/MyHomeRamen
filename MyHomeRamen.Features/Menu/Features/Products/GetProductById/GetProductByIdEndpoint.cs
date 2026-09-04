using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Products.GetProductById;

public sealed record IngredientDto(Guid Id, string Name, string Description, decimal Price);

public sealed record ProductByIdDto(
    Guid Id,
    string Name,
    string Description,
    List<IngredientDto> BaseIngredients,
    List<IngredientDto> CustomIngredients);

public sealed record GetProductByIdResponse(
    Guid Id,
    string Name,
    string Description,
    List<IngredientDto> BaseIngredients,
    List<IngredientDto> CustomIngredients);

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
        [FromServices] IRequestHandler<GetProductByIdQuery, GetProductByIdResponse> handler,
        CancellationToken cancellationToken)
    {
        GetProductByIdQuery query = new(id);
        GetProductByIdResponse response = await handler.Handle(query, cancellationToken);
        return Results.Ok(response);
    }
}
