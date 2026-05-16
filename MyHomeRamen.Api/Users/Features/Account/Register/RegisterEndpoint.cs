using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Api.Users.Features.Account.Register.Models;

namespace MyHomeRamen.Api.Users.Features.Account.Register;

public sealed class RegisterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedPost<RegisterRequest, RegisterRequest>("api/account/sign-up", Handler)
                       .WithName("RegisterEndpoint")
                       .WithTags("account")
                       .WithDescription("Handles user registration")
                       .AllowAnonymous();
    }

    private static async Task<Results<Ok, BadRequest>> Handler(RegisterRequest request, [FromServices] IRequestHandler<RegisterRequest> handler, CancellationToken cancellationToken)
    {
        await handler.Handle(request, cancellationToken);

        return TypedResults.Ok();
    }
}
