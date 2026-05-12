using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedDelete<DeleteCategoryRequest>("api/menu/categories/{id}", HandleAsync)
                       .WithName("DeleteCategoryEndpoint")
                       .WithTags("Categories")
                       .WithDescription("Handles Delete Category operations.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(DeleteCategoryRequest id, [FromServices] IRequestHandler<DeleteCategoryRequest, IResult> handler, CancellationToken cancellationToken)
    {
        return await handler.Handle(id, cancellationToken);
    }
}
