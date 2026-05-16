using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;

namespace MyHomeRamen.Api.Users.Features.Account.Register;

public sealed class RegisterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedPost<RegisterCommand, RegisterCommand>("api/account/sign-up", Handler)
                       .WithName("RegisterEndpoint")
                       .WithTags("account")
                       .WithDescription("Handles user registration")
                       .AllowAnonymous();
    }

    private static async Task<Results<Ok, BadRequest>> Handler(
        [FromBody] RegisterRequest request,
        [FromServices] IRequestHandler<RegisterCommand> handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(new RegisterCommand(request), cancellationToken);

        return TypedResults.Ok();
    }
}
