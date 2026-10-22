using FluentValidation;
using Microsoft.Extensions.Localization;
using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.ConfigureCompanySetup.Models;

public sealed class CreateInitialRestaurantFormModel
{
    public string Name { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string BankName { get; set; } = string.Empty;
    public string RoutingNumber { get; set; } = string.Empty;

    public InitialRestaurantSetupRequest ToRequest() =>
        new(Name, Street, City, ZipCode, Latitude, Longitude, Phone, Email, AccountNumber, BankName, RoutingNumber);
}

public sealed class CreateInitialRestaurantFormModelValidator : BaseValidator<CreateInitialRestaurantFormModel>
{
    public CreateInitialRestaurantFormModelValidator(IStringLocalizer<CompanySetupResources> text)
    {
        RuleFor(model => model.Name).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.Street).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.City).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.ZipCode).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.Phone).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage(text["Required"])
            .EmailAddress().WithMessage(text["EmailInvalid"]);
        RuleFor(model => model.AccountNumber).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.BankName).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.RoutingNumber).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.Latitude).InclusiveBetween(-90, 90).WithMessage(text["LatitudeInvalid"]);
        RuleFor(model => model.Longitude).InclusiveBetween(-180, 180).WithMessage(text["LongitudeInvalid"]);
    }
}
