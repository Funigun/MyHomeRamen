using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Query;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Features.Identity.Features.Users.GetEmployees;

public sealed record GetEmployeesResponse(IEnumerable<EmployeeDto> Employees);

public sealed record EmployeeDto(string UserName, string FirstName, string LastName, string Email);

public sealed class GetEmployeesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardGet<GetEmployeesResponse>("api/admin/employee", Handler)
                       .RequireAuthorization(AuthorizationPolicies.RestaurantManagerPolicy)
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

