using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Categories.DeleteCategory;

public sealed class DeleteCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedDelete<DeleteCategoryCommand>("api/menu/categories/{id}", HandleAsync)
                       .WithName("DeleteCategoryEndpoint")
                       .WithTags("Categories")
                       .WithDescription("Handles Delete Category operations.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(DeleteCategoryCommand id, [FromServices] ICommandHandler<DeleteCategoryCommand, IResult> handler, CancellationToken cancellationToken)
    {
        return await handler.Handle(id, cancellationToken);
    }
}
