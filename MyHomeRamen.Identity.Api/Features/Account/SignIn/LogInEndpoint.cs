using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Identity.Api.Application.Exceptions;
using MyHomeRamen.Identity.Api.Application.Services;
using MyHomeRamen.Identity.Api.Domain;
using MyHomeRamen.Identity.Api.Features.Account.SignIn.Models;
using MyHomeRamen.Identity.Api.Persistance;

namespace MyHomeRamen.Identity.Api.Features.Account.SignIn;

public class LogInEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Account";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<LogInRequest, LogInResponse>("/sign-in", Handler)
                       .WithName("LogInEndpoint")
                       .WithDescription("Handles LogIn operations.");
    }

    private static async Task<Results<Ok<LogInResponse>, BadRequest>> Handler(LogInRequest request, UserManager<User> userManager, AuthorizationService authorizationService, AppDbContext dbContext, IConfiguration configuration, CancellationToken cancellationToken)
    {
        User user = await userManager.Users.FirstAsync(userManager => userManager.UserName == request.UserName, cancellationToken);

        if (!await userManager.CheckPasswordAsync(user, request.Password))
        {
            throw IdentityValidationException.LogInFailed();
        }

        DateTime tokenExpirationTime = authorizationService.CalculateTokenExpirationTime(configuration, isRefreshToken: false);

        string token = await authorizationService.GenerateToken(user, userManager, configuration, isRefreshToken: false, tokenExpirationTime);

        await dbContext.SaveChangesAsync(cancellationToken);

        LogInResponse response = new(token);

        return TypedResults.Ok(response);
    }
}
