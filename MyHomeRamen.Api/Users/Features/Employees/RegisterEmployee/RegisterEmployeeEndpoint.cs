using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.WebPresentation;
using MyHomeRamen.Common.Contracts.Users.Employees.Requests;

namespace MyHomeRamen.Api.Users.Features.Employees.RegisterEmployee;

public sealed class RegisterEmployeeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<RegisterEmployeeCommand, Created>("api/admin/employee-sign-up", Handler)
                       .RequireAuthorization(AuthorizationDependencyInjection.RestaurantManagerPolicy)
                       .WithName("CreateEmployeeEndpoint")
                       .WithTags("admin")
                       .WithDescription("Creates an employee account in Keycloak. Requires admin role.");
    }

    private static async Task<Results<Created, BadRequest>> Handler(
        [FromBody] RegisterEmployeeRequest request,
        [FromServices] IRequestHandler<RegisterEmployeeCommand> handler,
        CancellationToken cancellationToken)
    {
        RegisterEmployeeCommand command = new(request);

        await handler.Handle(command, cancellationToken);

        return TypedResults.Created();
    }
}
