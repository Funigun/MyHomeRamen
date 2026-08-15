using FluentValidation;
using MyHomeRamen.Blazor.Common.Models;
using MyHomeRamen.Blazor.Features.Account.Components.Validators;

namespace MyHomeRamen.Blazor.Features.Account.Components;

public sealed class SignUpValidator : BaseValidator<SignUpModel>
{
    public SignUpValidator()
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
