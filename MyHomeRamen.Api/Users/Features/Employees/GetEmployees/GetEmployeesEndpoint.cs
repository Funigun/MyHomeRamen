using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Pipeline;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Users.Employees.Responses;

namespace MyHomeRamen.Api.Users.Features.Employees.GetEmployees;

public sealed class GetEmployeesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetEmployeesResponse>("api/admin/employee", Handler)
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy)
                       .WithName("GetEmployeesEndpoint")
                       .WithTags("admin")
                       .WithDescription("Handles GetEmployees operations.");
    }

    private static async Task<Results<Ok<GetEmployeesResponse>, NotFound>> Handler(
        [FromServices] IQueryHandler<GetEmployeesQuery, GetEmployeesResponse> handler,
        CancellationToken cancellationToken)
    {
        GetEmployeesQuery query = new();
        GetEmployeesResponse response = await handler.Handle(query, cancellationToken);

        return TypedResults.Ok(response);
    }
}
