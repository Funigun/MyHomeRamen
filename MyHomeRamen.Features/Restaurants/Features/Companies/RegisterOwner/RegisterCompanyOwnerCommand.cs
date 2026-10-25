using FluentValidation;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Identity.ExternalApi;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.RegisterOwner;

public sealed record RegisterCompanyOwnerRequest(
    string UserName,
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    string Password,
    string ConfirmPassword);

public sealed record RegisterCompanyOwnerResponse(Guid UserId);

public sealed record RegisterCompanyOwnerCommand(RegisterCompanyOwnerRequest Request) : ICommand<RegisterCompanyOwnerResponse>;

public sealed class RegisterCompanyOwnerCommandValidator : AbstractValidator<RegisterCompanyOwnerCommand>
{
    public RegisterCompanyOwnerCommandValidator(IIdentityService identityService)
    {
        RuleFor(_ => _)
            .MustAsync(async (_, cancellationToken) =>
                !await identityService.IsCompanyOwnerConfiguredAsync(cancellationToken))
            .WithMessage("A company owner has already been registered for this deployment.");
    }
}

public sealed class RegisterCompanyOwnerHandler(IIdentityService identityService)
                  : IRequestHandler<RegisterCompanyOwnerCommand, RegisterCompanyOwnerResponse>
{
    public async Task<RegisterCompanyOwnerResponse> Handle(RegisterCompanyOwnerCommand command, CancellationToken cancellationToken)
    {
        CompanyOwnerRegistrationResult identityResult = await identityService.RegisterCompanyOwnerAsync(
            new CompanyOwnerDto(
                command.Request.UserName,
                command.Request.FirstName,
                command.Request.LastName,
                command.Request.Email,
                command.Request.PhoneNumber,
                command.Request.Password,
                command.Request.ConfirmPassword),
            cancellationToken);

        if (identityResult.Error is not null)
        {
            throw new ValidationException(identityResult.Error);
        }

        if (identityResult.UserId is not Guid userId)
        {
            throw new InvalidOperationException("Identity service returned an incomplete company owner registration result.");
        }

        return new RegisterCompanyOwnerResponse(userId);
    }
}
