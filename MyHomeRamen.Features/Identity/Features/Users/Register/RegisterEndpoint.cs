using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Features.Common.Endpoints;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Identity.Features.Users.Register;

public sealed record RegisterRequest(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword);

public sealed class RegisterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<RegisterCommand>("api/account/sign-up", Handler)
                       .WithName("RegisterEndpoint")
                       .WithTags("account")
                       .WithDescription("Handles user registration")
                       .AllowAnonymous();
    }

    private static async Task<Results<Ok, BadRequest>> Handler(
        [FromBody] RegisterRequest request,
        [FromServices] IRequestHandler<RegisterCommand, Unit> handler,
        CancellationToken cancellationToken)
    {
        RegisterCommand command = new(request);
        await handler.Handle(command, cancellationToken);

        return TypedResults.Ok();
    }
}
