using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Identity.Api.Features.Admin.GetEmployees.Models;
using MyHomeRamen.Identity.Api.Persistance;

namespace MyHomeRamen.Identity.Api.Features.Admin.GetEmployees;

public sealed class GetEmployeesEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Admin";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardAuthenticatedGet<GetEmployeesResponse>(string.Empty, Handler)
                       .WithName("GetEmployeesEndpoint")
                       .WithDescription("Handles GetEmployees operations.");
    }

    private static async Task<Results<Ok<GetEmployeesResponse>, NotFound>> Handler(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        List<EmployeeDto>? employees = await dbContext.Users
            .Select(u => u.ToResponse())
            .ToListAsync(cancellationToken);

        GetEmployeesResponse response = new(employees);

        return TypedResults.Ok(response);
    }
}
