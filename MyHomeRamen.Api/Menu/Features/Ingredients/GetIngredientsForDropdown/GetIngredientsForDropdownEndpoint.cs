using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;

namespace MyHomeRamen.Api.Menu.Features.Ingredients.GetIngredientsForDropdown;

public sealed class GetIngredientsForDropdownEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<IEnumerable<GetIngredientsForDropdownResponse>>("api/menu/ingredients/dropdown", HandleAsync)
            .WithName("GetIngredientsForDropdownEndpoint")
            .WithTags("Ingredients")
            .WithDescription("Returns an ordered list of ingredients for use in dropdown selectors.")
            .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IRequestHandler<GetIngredientsForDropdownQuery, IEnumerable<GetIngredientsForDropdownResponse>> handler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetIngredientsForDropdownResponse> response = await handler.Handle(new GetIngredientsForDropdownQuery(), cancellationToken);
        return Results.Ok(response);
    }
}
