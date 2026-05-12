using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.CreateCategory.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Categories.CreateCategory;

public sealed class CreateCategoryEndpoint : IEndpoint
{

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedPost<CreateCategoryRequest, CreateCategoryResponse>("api/menu/categories", HandleAsync)
                       .WithName("CreateCategoryEndpoint")
                       .WithTags("Categories")
                       .WithDescription("Handles Create Category operations.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateCategoryRequest request, 
        [FromServices] IRequestHandler<CreateCategoryRequest, Guid> handler,
        CancellationToken cancellationToken)
    {
        Guid id = await handler.Handle(request, cancellationToken);
        CreateCategoryResponse response = new(id);

        return Results.Created($"/api/menu/categories/{id}", response);
    }
}
