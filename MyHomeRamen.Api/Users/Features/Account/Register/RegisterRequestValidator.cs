using FluentValidation;
using MyHomeRamen.Api.Users.Features.Account.Register.Models;
using MyHomeRamen.Common.Contracts.Account;

namespace MyHomeRamen.Api.Users.Features.Account.Register;

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
