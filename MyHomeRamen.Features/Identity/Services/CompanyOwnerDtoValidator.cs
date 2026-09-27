using FluentValidation;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Identity.Features.Users.Common;

namespace MyHomeRamen.Features.Identity.Services;

public sealed class CompanyOwnerDtoValidator : AbstractValidator<CompanyOwnerDto>
{
    public CompanyOwnerDtoValidator()
    {
        RuleFor(x => x.UserName).ValidUserName();
        RuleFor(x => x.FirstName).ValidName();
        RuleFor(x => x.LastName).ValidName();
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email must not be empty.")
            .EmailAddress()
            .WithMessage("Email must be valid.");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone number must not be empty.");
        RuleFor(x => x.Password).ValidPassword();
        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");
        RuleFor(x => x.CompanyId)
            .NotEmpty()
            .WithMessage("Company ID must not be empty.");
        RuleFor(x => x.IdempotencyKey)
            .NotEmpty()
            .WithMessage("Idempotency-Key is required.");
    }
}
