using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Products.UpdateProduct;

public sealed record UpdateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);

public sealed record UpdateProductResponse(Guid Id);

public sealed class UpdateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardPut<UpdateProductResponse>("api/menu/products/{id}", HandleAsync)
            .WithName("UpdateProductEndpoint")
            .WithTags("Products")
            .WithDescription("Updates the name, description, price, category, and ingredients of an existing product.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateProductRequest request,
        [FromServices] IRequestHandler<UpdateProductCommand, UpdateProductResponse> handler,
        CancellationToken cancellationToken)
    {
        UpdateProductCommand command = new(new ProductId(id), request);
        UpdateProductResponse response = await handler.Handle(command, cancellationToken);

        return Results.Ok(response);
    }
}
