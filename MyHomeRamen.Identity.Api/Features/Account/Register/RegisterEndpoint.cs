using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MyHomeRamen.Api.Common.Endpoint;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Identity.Api.Features.Account.Register.Models;
using MyHomeRamen.Infrastructure.Keycloak;
using MyHomeRamen.Infrastructure.Keycloak.Dto;
using MyHomeRamen.Domain.Users;
using MyHomeRamen.Api.Common.Configuration;

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

    private static async Task<Results<Ok, BadRequest>> Handler(
        RegisterRequest request,
        [FromServices] IKeycloakAdminService keycloakAdminService,
        [FromServices] IUsersDbContext usersDbContext,
        [FromServices] RestaurantConfigurationProvider restaurantConfigurationProvider,
        CancellationToken cancellationToken)
    {
        KeycloakUserDto keycloakUser = request.ToUserDto();

        string keycloakUserId = await keycloakAdminService.CreateUserAsync(keycloakUser, RoleConstants.Customer, cancellationToken);

        User user = User.Create(
            restaurantConfigurationProvider.RestaurantId,
            keycloakUserId,
            request.UserName,
            request.FirstName,
            request.LastName,
            request.Email,
            request.PhoneNumber,
            RoleConstants.Customer
            );

        usersDbContext.Users.Add(user);
        await usersDbContext.SaveChangesAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
