using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Authorization;

namespace MyHomeRamen.Features.Menu.Features.Ingredients.GetIngredientsForDropdown;

public sealed record IngredientForDropdownDto(Guid Id, string Name);

public sealed record GetIngredientsForDropdownResponse(IEnumerable<IngredientForDropdownDto> Ingredients);

public sealed class GetIngredientsForDropdownEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetIngredientsForDropdownResponse>("api/menu/ingredients/dropdown", HandleAsync)
            .WithName("GetIngredientsForDropdownEndpoint")
            .WithTags("Ingredients")
            .WithDescription("Returns an ordered list of ingredients for use in dropdown selectors.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] IQueryHandler<GetIngredientsForDropdownQuery, GetIngredientsForDropdownResponse> handler,
        CancellationToken cancellationToken)
    {
        GetIngredientsForDropdownQuery query = new();
        GetIngredientsForDropdownResponse response = await handler.Handle(query, cancellationToken);

        return Results.Ok(response);
    }
}

