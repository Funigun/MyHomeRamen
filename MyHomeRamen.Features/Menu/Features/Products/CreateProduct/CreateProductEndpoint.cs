using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Products.CreateProduct;

public sealed record CreateProductRequest(
    string Name,
    string? Description,
    decimal Price,
    Guid CategoryId,
    IEnumerable<Guid> IngredientIds,
    IEnumerable<Guid> CustomIngredientIds);

public sealed record CreateProductResponse(Guid Id);

public sealed class CreateProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateProductResponse>("api/menu/products", HandleAsync)
                       .WithName("CreateProductEndpoint")
                       .WithTags("Products")
                       .WithDescription("Handles Create Product operations.")
                       .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateProductRequest request,
        [FromServices] IRequestHandler<CreateProductCommand, CreateProductResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateProductCommand command = new(request);
        CreateProductResponse response = await handler.Handle(command, cancellationToken);

        return Results.Created($"/api/menu/products/{response.Id}", response);
    }
}
