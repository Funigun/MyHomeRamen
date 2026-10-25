using FluentValidation;
using Microsoft.Extensions.Localization;
using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.ConfigureCompanySetup.Models;

public sealed class RegisterCompanyOwnerFormModel
{
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;

    public RegisterCompanyOwnerRequest ToRequest() =>
        new(UserName, FirstName, LastName, Email, PhoneNumber, Password, ConfirmPassword);
}

public sealed class RegisterCompanyOwnerFormModelValidator : BaseValidator<RegisterCompanyOwnerFormModel>
{
    public RegisterCompanyOwnerFormModelValidator(IStringLocalizer<CompanySetupResources> text)
    {
        RuleFor(model => model.UserName).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.FirstName).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.LastName).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(text["Required"])
            .EmailAddress().WithMessage(text["EmailInvalid"]);
        RuleFor(model => model.PhoneNumber).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.Password).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.ConfirmPassword)
            .NotEmpty().WithMessage(text["Required"])
            .Equal(model => model.Password).WithMessage(text["PasswordsDoNotMatch"]);
    }
}
