using FluentValidation;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.Features.Users.Common;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.Features.Identity.Services.Dto;
using MyHomeRamen.Features.Common.Mediator;

namespace MyHomeRamen.Features.Identity.Features.Users.Register;

public sealed record RegisterCommand(RegisterRequest Request) : ICommand;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Request.UserName)
            .ValidUserName();

        RuleFor(x => x.Request.FirstName)
            .ValidName();

        RuleFor(x => x.Request.LastName)
            .ValidName();

        RuleFor(x => x.Request.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Request.PhoneNumber)
            .NotEmpty();

        RuleFor(x => x.Request.Password)
            .ValidPassword();

        RuleFor(x => x.Request.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Request.Password)
            .WithMessage("Passwords do not match.");
    }
}

public class RegisterHandler(IKeycloakAdminService keycloakAdminService, IIdentityDbContext usersDbContext) : IRequestHandler<RegisterCommand, Unit>
{
    public async Task<Unit> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        KeycloakUserDto keycloakUser = command.Request.ToKeycloakUserDto();

        string keycloakUserId = await keycloakAdminService.CreateUserAsync(keycloakUser, cancellationToken);

        Role role = await usersDbContext.Role.Load().ByName(RoleConstants.Customer, cancellationToken)
                    ?? throw new InvalidOperationException("Customer role was not found.");
        User user = command.Request.ToUserDto(keycloakUserId, role);

        usersDbContext.User.Add(user);
        await usersDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}

internal static class Mappings
{
    extension(RegisterRequest request)
    {
        internal KeycloakUserDto ToKeycloakUserDto()
        {
            return new KeycloakUserDto
            {
                Username = request.UserName,
                Email = request.Email,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Enabled = true,
                Credentials =
                [
                    new KeycloakCredentialDto
                    {
                        Type = "password",
                        Value = request.Password,
                        Temporary = false,
                    }
                ]
            };
        }
    }

    extension(RegisterRequest request)
    {
        internal User ToUserDto(string keycloakUserId, Role role)
        {
            return User.Create(
                keycloakUserId,
                request.UserName,
                request.FirstName,
                request.LastName,
                request.Email,
                request.PhoneNumber,
                role
            );
        }
    }
}
