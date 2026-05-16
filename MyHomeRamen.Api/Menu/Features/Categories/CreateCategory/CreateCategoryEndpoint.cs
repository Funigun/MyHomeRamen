using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Menu.Categories.Requests;
using MyHomeRamen.Common.Contracts.Menu.Categories.Responses;

namespace MyHomeRamen.Api.Menu.Features.Categories.CreateCategory;

public sealed class CreateCategoryEndpoint : IEndpoint
{

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateCategoryResponse>("api/menu/categories", HandleAsync)
                       .WithName("CreateCategoryEndpoint")
                       .WithTags("Categories")
                       .WithDescription("Handles Create Category operations.")
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateCategoryRequest request,
        [FromServices] ICommandHandler<CreateCategoryCommand, CreateCategoryResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateCategoryCommand command = new(request);
        CreateCategoryResponse response = await handler.Handle(command, cancellationToken);

        return Results.Created($"/api/menu/categories/{response.Id}", response);
    }
}
