using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Common.Contracts.Menu.Ingredients.Responses;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForDropdown;

public sealed class GetIngredientsForDropdownEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<IEnumerable<GetIngredientsForDropdownResponse>>("api/menu/ingredients/dropdown", HandleAsync)
            .WithName("GetIngredientsForDropdownEndpoint")
            .WithTags("Ingredients")
            .WithDescription("Returns an ordered list of ingredients for use in dropdown selectors.")
            .RequireAuthorization("RestaurantManager");
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IQueryHandler<GetIngredientsForDropdownQuery, IEnumerable<GetIngredientsForDropdownResponse>> handler,
        CancellationToken cancellationToken)
    {
        GetIngredientsForDropdownQuery query = new();
        IEnumerable<GetIngredientsForDropdownResponse> response = await handler.Handle(query, cancellationToken);

        return Results.Ok(response);
    }
}
