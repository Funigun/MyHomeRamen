using FluentValidation;
using MyHomeRamen.Domain.Identity.CompanyMemberships;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.Features.Users.Common;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.Features.Identity.Services.Dto;
using MyHomeRamen.Features.Restaurants.ExternalApi;

namespace MyHomeRamen.Features.Identity.Features.Companies.RegisterOwner;

public sealed record RegisterCompanyOwnerCommand(RegisterCompanyOwnerRequest Request, string IdempotencyKey) : ICommand<RegisterCompanyOwnerResponse>;

public sealed class RegisterCompanyOwnerCommandValidator : AbstractValidator<RegisterCompanyOwnerCommand>
{
    public RegisterCompanyOwnerCommandValidator(IRestaurantsService restaurantsExternalApi)
    {
        RuleFor(x => x).Cascade(CascadeMode.Stop)
            .NotNull()
            .WithMessage("Request cannot be null.")
            .ChildRules(c =>
            {
                c.RuleFor(x => x.IdempotencyKey).NotEmpty().WithMessage("Idempotency-Key is required.");
                c.RuleFor(x => x.Request.UserName).ValidUserName();
                c.RuleFor(x => x.Request.FirstName).ValidName();
                c.RuleFor(x => x.Request.LastName).ValidName();
                c.RuleFor(x => x.Request.Email).NotEmpty().EmailAddress();
                c.RuleFor(x => x.Request.PhoneNumber).NotEmpty();
                c.RuleFor(x => x.Request.Password).ValidPassword();
                c.RuleFor(x => x.Request.ConfirmPassword).NotEmpty().Equal(x => x.Request.Password).WithMessage("Passwords do not match.");

                c.RuleFor(x => x.Request.CompanyName)
                    .NotEmpty()
                    .Must((companyName) => restaurantsExternalApi.ValidateCompanyName(companyName) == null)
                        .WithMessage("Company name is invalid or exceeds the maximum length.")
                    .MustAsync(async (companyName, cancellationToken) => await restaurantsExternalApi.IsNameUnique(companyName, cancellationToken))
                        .WithMessage("Company name must be unique.")
                        .WithErrorCode("Conflict");
            });
    }
}

public sealed class RegisterCompanyOwnerHandler(IKeycloakAdminService keycloakAdminService, IIdentityDbContext identityDbContext, IRestaurantsService restaurantsExternalApi)
                  : IRequestHandler<RegisterCompanyOwnerCommand, RegisterCompanyOwnerResponse>
{
    public async Task<RegisterCompanyOwnerResponse> Handle(RegisterCompanyOwnerCommand command, CancellationToken cancellationToken)
    {
        CompanyMembership? existing = await identityDbContext.CompanyMembership.ByIdempotencyKey(command.IdempotencyKey, cancellationToken);
        if (existing is not null)
        {
            return new RegisterCompanyOwnerResponse(existing.CompanyId);
        }

        KeycloakUserDto keycloakUser = new()
        {
            Username = command.Request.UserName,
            Email = command.Request.Email,
            FirstName = command.Request.FirstName,
            LastName = command.Request.LastName,
            Enabled = true,
            Credentials = [new KeycloakCredentialDto { Type = "password", Value = command.Request.Password, Temporary = false }]
        };

        string keycloakUserId = await keycloakAdminService.CreateUserAsync(keycloakUser, cancellationToken);
        Role customerRole = await identityDbContext.Role.Load().ByName(RoleConstants.Customer, cancellationToken)
                            ?? throw new InvalidOperationException("Customer role was not found.");
        Role ownerRole = await identityDbContext.Role.Load().ByName(RoleConstants.CompanyOwner, cancellationToken)
                         ?? throw new InvalidOperationException("Company owner role was not found.");

        User user = User.Create(keycloakUserId, command.Request.UserName, command.Request.FirstName, command.Request.LastName, command.Request.Email, command.Request.PhoneNumber, customerRole);
        user.AddRole(ownerRole);
        CompanyRegistrationResult company = await restaurantsExternalApi.RegisterCompanyAsync(command.Request.CompanyName, cancellationToken);
        CompanyMembership membership = CompanyMembership.CreateOwner(user.Id, company.CompanyId, ownerRole.Id, command.IdempotencyKey);

        identityDbContext.User.Add(user);
        identityDbContext.CompanyMembership.Add(membership);
        await identityDbContext.SaveChangesAsync(cancellationToken);

        return new RegisterCompanyOwnerResponse(company.CompanyId);
    }
}
