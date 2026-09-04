using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Menu.Features.Categories.UpdateCategoriesOrder;

public sealed record CategoryOrderItemDto(Guid Id, int SortOrder);

public sealed record UpdateCategoriesOrderRequest(IEnumerable<CategoryOrderItemDto> Items);

public sealed class UpdateCategoriesOrderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPut<UpdateCategoriesOrderCommand>("api/menu/categories/order", HandleAsync)
                       .WithName("UpdateCategoriesOrderEndpoint")
                       .WithTags("Categories")
                       .WithDescription("Updates the sort order of multiple categories in a single batch operation.")
                       .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync([FromBody] UpdateCategoriesOrderRequest request, [FromServices] IRequestHandler<UpdateCategoriesOrderCommand, Unit> handler,  CancellationToken cancellationToken)
    {
        UpdateCategoriesOrderCommand command = new(request);
        await handler.Handle(command, cancellationToken);
        return Results.NoContent();
    }
}
