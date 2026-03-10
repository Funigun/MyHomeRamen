using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.Identity.Api.Features.Account.Register.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.Register;

public sealed class RegisterEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Account";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardValidatedPost<RegisterRequest, RegisterRequest>("/sign-up", Handler)
                       .WithName("RegisterEndpoint")
                       .WithDescription("Handles user registration");
    }

    private static async Task<Results<Ok, BadRequest>> Handler(RegisterRequest request, [FromServices] IRequestHandler<RegisterRequest> handler, CancellationToken cancellationToken)
    {
        await handler.Handle(request, cancellationToken);

        return TypedResults.Ok();
    }
}
