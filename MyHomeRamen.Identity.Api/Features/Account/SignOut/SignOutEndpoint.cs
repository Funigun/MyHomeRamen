using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Domain.Users;

namespace MyHomeRamen.Identity.Api.Features.Account.SignOut;

public class SignOutEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Account";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapPost("/sign-out", Handler)
                       .WithName("SignOutEndpoint")
                       .WithDescription("Handles SignOut operations.");
    }

    private static async Task<IResult> Handler([FromServices] SignInManager<User> signInManager, CancellationToken cancellationToken)
    {
        await signInManager.SignOutAsync();

        return Results.Ok();
    }
}
