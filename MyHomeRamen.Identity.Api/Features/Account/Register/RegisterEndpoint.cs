using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyHomeRamen.Api.Common.Configuration;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Domain.Common.Authorization;
using MyHomeRamen.Identity.Api.Application.Exceptions;
using MyHomeRamen.Identity.Api.Domain;
using MyHomeRamen.Identity.Api.Features.Account.Register.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.Register;

public sealed class RegisterEndpoint : IEndpoint
{
    public string GroupName { get; init; } = "Account";

    public void MapEndpoint(IEndpointRouteBuilder endpointBuilder)
    {
        endpointBuilder.MapStandardPost<RegisterRequest, RegisterRequest>("/sign-up", Handler)
               .WithName("RegisterEndpoint")
               .WithDescription("Handles user registration");
    }

    private static async Task<Results<Ok, BadRequest>> Handler(RegisterRequest request, [FromServices] UserManager<User> userManager, [FromServices] RestaurantConfigurationProvider configurationProvider, CancellationToken cancellationToken)
    {
        User user = request.ToUser(configurationProvider.RestaurantId);

        if (await userManager.Users.AnyAsync(usr => usr.UserName!.ToUpper() == user.UserName!.ToUpper() || usr.Email.ToUpper() == user.Email.ToUpper(), cancellationToken))
        {
            throw IdentityValidationException.UserNameAlreadyInUse();
        }

        IdentityResult result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            throw IdentityValidationException.RegistrationFailed(result.Errors.Select(error => error.Description));
        }

        return TypedResults.Ok();
    }
}
