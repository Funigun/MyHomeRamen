using FluentValidation;
using MyHomeRamen.Common.Contracts.Account;
using MyHomeRamen.Identity.Api.Features.Account.Register.Models;

namespace MyHomeRamen.Identity.Api.Features.Account.Register;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.UserName)
            .ValidUserName();

        RuleFor(x => x.FirstName)
            .ValidName();

        RuleFor(x => x.LastName)
            .ValidName();

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.PhoneNumber)
            .NotEmpty();

        RuleFor(x => x.Password)
            .ValidPassword();

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match.");
    }
}
