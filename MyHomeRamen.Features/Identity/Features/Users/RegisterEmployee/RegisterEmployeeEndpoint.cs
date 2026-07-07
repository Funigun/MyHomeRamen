using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Common.Contracts.Users.Employees.Requests;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Features.Identity.Features.Users.RegisterEmployee;

public sealed class RegisterEmployeeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<Created>("api/admin/employee-sign-up", Handler)
                       .RequireAuthorization(AuthorizationPolicies.RestaurantManagerPolicy)
                       .WithName("CreateEmployeeEndpoint")
                       .WithTags("admin")
                       .WithDescription("Creates an employee account in Keycloak. Requires admin role.");
    }

    private static async Task<Results<Created, BadRequest>> Handler(
        [FromBody] RegisterEmployeeRequest request,
        [FromServices] ICommandHandler<RegisterEmployeeCommand> handler,
        CancellationToken cancellationToken)
    {
        RegisterEmployeeCommand command = new(request);
        await handler.Handle(command, cancellationToken);

        return TypedResults.Created();
    }
}

