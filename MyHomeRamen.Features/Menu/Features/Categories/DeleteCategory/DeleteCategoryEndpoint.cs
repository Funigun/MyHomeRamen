using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Categories.DeleteCategory;

public sealed record DeleteCategoryRequest(Guid Id);

public sealed class DeleteCategoryEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardDelete("api/menu/categories/{id}", HandleAsync)
                       .WithName("DeleteCategoryEndpoint")
                       .WithTags("Categories")
                       .WithDescription("Handles Delete Category operations.")
                       .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync([AsParameters] DeleteCategoryRequest request, [FromServices] IRequestHandler<DeleteCategoryCommand, Unit> handler, CancellationToken cancellationToken)
    {
        DeleteCategoryCommand command = new(request);
        await handler.Handle(command, cancellationToken);

        return TypedResults.NoContent();
    }
}
