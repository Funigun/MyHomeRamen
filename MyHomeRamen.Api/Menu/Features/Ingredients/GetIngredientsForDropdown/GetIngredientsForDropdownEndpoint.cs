using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForDropdown.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForDropdown;

public sealed class GetIngredientsForDropdownEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<IEnumerable<GetIngredientsForDropdownResponse>>("ingredients/dropdown", HandleAsync)
            .WithName("GetIngredientsForDropdownEndpoint")
            .WithDescription("Returns an ordered list of ingredients for use in dropdown selectors.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IRequestHandler<GetIngredientsForDropdownRequest, IEnumerable<GetIngredientsForDropdownResponse>> handler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetIngredientsForDropdownResponse> response = await handler.Handle(new GetIngredientsForDropdownRequest(), cancellationToken);
        return Results.Ok(response);
    }
}
