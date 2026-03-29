using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown.Models;
using MyHomeRamen.Api.WebPresentation;

namespace MyHomeRamen.Api.Menu.Features.Categories.GetCategoriesForDropdown;

public sealed class GetCategoriesForDropdownEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Menu";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardValidatedGet<GetCategoriesForDropdownRequest, IEnumerable<GetCategoriesForDropdownResponse>>(
                "categories/dropdown", HandleAsync)
            .WithName("GetCategoriesForDropdownEndpoint")
            .WithDescription("Returns a filtered and ordered list of categories for use in dropdown selectors.")
            .RequireAuthorization(AuthorizationConfiguration.RestaurantManagerPolicy);
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] GetCategoriesForDropdownRequest request,
        [FromServices] IRequestHandler<GetCategoriesForDropdownRequest, IEnumerable<GetCategoriesForDropdownResponse>> handler,
        CancellationToken cancellationToken)
    {
        IEnumerable<GetCategoriesForDropdownResponse> response = await handler.Handle(request, cancellationToken);
        return Results.Ok(response);
    }
}
