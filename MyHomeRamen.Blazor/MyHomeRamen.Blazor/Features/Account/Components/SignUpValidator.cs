using FluentValidation;
using MyHomeRamen.Common.Contracts.Account;

namespace MyHomeRamen.Blazor.Features.Account.Components;

public sealed class SignUpValidator : AbstractValidator<SignUpModel>
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

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        FluentValidation.Results.ValidationResult? result = await ValidateAsync(
            ValidationContext<SignUpModel>.CreateWithOptions(
                (SignUpModel)model,
                x => x.IncludeProperties(propertyName)));

        return result.IsValid ? [] : result.Errors.Select(e => e.ErrorMessage);
    };
}
