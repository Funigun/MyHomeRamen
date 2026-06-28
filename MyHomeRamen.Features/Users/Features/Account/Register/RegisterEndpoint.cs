using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using MyHomeRamen.Features.Common.Endpoints.Command;
using MyHomeRamen.Common.Contracts.Users.Account.Requests;
using MyHomeRamen.Features.Common.Endpoints;

namespace MyHomeRamen.Features.Users.Features.Account.Register;

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
        [FromServices] ICommandHandler<RegisterCommand> handler,
        CancellationToken cancellationToken)
    {
        RegisterCommand command = new(request);
        await handler.Handle(command, cancellationToken);

        return TypedResults.Ok();
    }
}

