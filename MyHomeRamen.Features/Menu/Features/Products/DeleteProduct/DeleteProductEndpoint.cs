using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Domain.Menu.Products;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Products.DeleteProduct;

public sealed class DeleteProductEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardDelete("api/menu/products/{id}", HandleAsync)
            .WithName("DeleteProductEndpoint")
            .WithTags("Products")
            .WithDescription("Deletes a product by ID.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromRoute] Guid id,
        [FromServices] IRequestHandler<DeleteProductCommand, Unit> handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(new DeleteProductCommand((ProductId)id), cancellationToken);
        return TypedResults.NoContent();
    }
}
