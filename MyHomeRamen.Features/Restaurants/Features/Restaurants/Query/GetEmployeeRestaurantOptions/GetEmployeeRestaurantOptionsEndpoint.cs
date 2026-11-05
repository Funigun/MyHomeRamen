using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Restaurants.Features.Restaurants.Query.GetEmployeeRestaurantOptions;

public sealed record GetEmployeeRestaurantOptionsResponse(IEnumerable<EmployeeRestaurantOptionDto> Restaurants);

public sealed class GetEmployeeRestaurantOptionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder
            .MapStandardGet<GetEmployeeRestaurantOptionsResponse>("api/restaurants/employee-options", HandleAsync)
            .WithName("GetEmployeeRestaurantOptionsEndpoint")
            .WithTags("Restaurants")
            .WithDescription("Get active restaurants assigned to the authenticated employee.")
            .RequireAuthorization(AuthorizationPolicies.AuthenticatedUserPolicy);
    }

    private static async Task<Ok<GetEmployeeRestaurantOptionsResponse>> HandleAsync(
        [FromServices] IRequestHandler<GetEmployeeRestaurantOptionsQuery, GetEmployeeRestaurantOptionsResponse> handler,
        CancellationToken cancellationToken)
    {
        GetEmployeeRestaurantOptionsQuery query = new();
        GetEmployeeRestaurantOptionsResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
