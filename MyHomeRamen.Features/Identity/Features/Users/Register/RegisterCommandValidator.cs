using FluentValidation;
using MyHomeRamen.Common.Contracts.Users.Account.Validators;

namespace MyHomeRamen.Features.Identity.Features.Users.Register;

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

