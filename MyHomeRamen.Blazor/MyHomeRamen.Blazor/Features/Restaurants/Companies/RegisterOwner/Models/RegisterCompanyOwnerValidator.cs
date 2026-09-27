using FluentValidation;
using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Account.Components.Validators;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.RegisterOwner.Models;

public sealed class RegisterCompanyOwnerValidator : BaseValidator<RegisterCompanyOwnerModel>
{
    public RegisterCompanyOwnerValidator()
    {
        RuleFor(model => model.UserName).ValidUserName();
        RuleFor(model => model.FirstName).ValidName();
        RuleFor(model => model.LastName).ValidName();
        RuleFor(model => model.Email).NotEmpty().EmailAddress();
        RuleFor(model => model.PhoneNumber).NotEmpty();
        RuleFor(model => model.Password).ValidPassword();
        RuleFor(model => model.ConfirmPassword)
            .NotEmpty()
            .Equal(model => model.Password)
            .WithMessage("Passwords do not match.");
        RuleFor(model => model.CompanyName)
            .Must(value => !string.IsNullOrWhiteSpace(value) && value.Any(char.IsLetterOrDigit))
            .WithMessage("Company name is required.");
    }
}
