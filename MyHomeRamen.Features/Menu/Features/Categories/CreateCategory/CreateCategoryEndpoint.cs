using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Endpoints.Command;

namespace MyHomeRamen.Features.Menu.Features.Categories.CreateCategory;

public sealed record CreateCategoryRequest(string Name, int CategoryType);

public sealed record CreateCategoryResponse(Guid Id);

public sealed class CreateCategoryEndpoint : IEndpoint
{

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<CreateCategoryResponse>("api/menu/categories", HandleAsync)
                       .WithName("CreateCategoryEndpoint")
                       .WithTags("Categories")
                       .WithDescription("Handles Create Category operations.")
                       .RequireAuthorization("RestaurantManager");
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] CreateCategoryRequest request,
        [FromServices] ICommandHandler<CreateCategoryCommand, CreateCategoryResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateCategoryCommand command = new(request);
        CreateCategoryResponse response = await handler.Handle(command, cancellationToken);

        return TypedResults.Created($"/api/menu/categories/{response.Id}", response);
    }
}
