using FluentValidation;
using Microsoft.Extensions.Localization;
using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Restaurants.Restaurants;
using MyHomeRamen.Blazor.Features.Restaurants.Restaurants.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Restaurants.CreateRestaurant;

public sealed class CreateRestaurantFormModel
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? AccountNumber { get; set; }
    public string? BankName { get; set; }
    public string? RoutingNumber { get; set; }

    public CreateRestaurantRequest ToRequest()
        => new(Name, IsActive, Street, City, ZipCode, Latitude!.Value, Longitude!.Value,
            EmptyToNull(Phone), EmptyToNull(Email), EmptyToNull(AccountNumber), EmptyToNull(BankName), EmptyToNull(RoutingNumber));

    private static string? EmptyToNull(string? value) => string.IsNullOrWhiteSpace(value) ? null : value;
}

public sealed class CreateRestaurantFormModelValidator : BaseValidator<CreateRestaurantFormModel>
{
    public CreateRestaurantFormModelValidator(IStringLocalizer<RestaurantsResources> text)
    {
        RuleFor(model => model.Name)
            .NotEmpty().WithMessage(text["NameRequired"])
            .MaximumLength(200).WithMessage(text["NameTooLong"]);
        RuleFor(model => model.Street).NotEmpty().WithMessage(text["StreetRequired"]);
        RuleFor(model => model.City).NotEmpty().WithMessage(text["CityRequired"]);
        RuleFor(model => model.ZipCode).NotEmpty().WithMessage(text["ZipCodeRequired"]);
        RuleFor(model => model.Latitude)
            .NotNull().WithMessage(text["LatitudeRequired"])
            .Must(value => !value.HasValue || value.Value is >= -90 and <= 90).WithMessage(text["LatitudeRange"]);
        RuleFor(model => model.Longitude)
            .NotNull().WithMessage(text["LongitudeRequired"])
            .Must(value => !value.HasValue || value.Value is >= -180 and <= 180).WithMessage(text["LongitudeRange"]);

        RuleFor(model => model.Phone)
            .NotEmpty().WithMessage(text["ContactPairRequired"])
            .When(model => !string.IsNullOrWhiteSpace(model.Email));
        RuleFor(model => model.Email)
            .NotEmpty().WithMessage(text["ContactPairRequired"])
            .When(model => !string.IsNullOrWhiteSpace(model.Phone));
        RuleFor(model => model.Email)
            .EmailAddress().WithMessage(text["EmailInvalid"])
            .When(model => !string.IsNullOrWhiteSpace(model.Email));

        When(HasAnyBankDetails, () =>
        {
            RuleFor(model => model.AccountNumber).NotEmpty().WithMessage(text["BankDetailsPairRequired"]);
            RuleFor(model => model.BankName).NotEmpty().WithMessage(text["BankDetailsPairRequired"]);
            RuleFor(model => model.RoutingNumber).NotEmpty().WithMessage(text["BankDetailsPairRequired"]);
        });
    }

    private static bool HasAnyBankDetails(CreateRestaurantFormModel model)
        => !string.IsNullOrWhiteSpace(model.AccountNumber)
            || !string.IsNullOrWhiteSpace(model.BankName)
            || !string.IsNullOrWhiteSpace(model.RoutingNumber);
}
