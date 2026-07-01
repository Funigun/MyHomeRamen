using FluentValidation;

namespace MyHomeRamen.Common.Contracts.Users.Account.Validators;

public static class AddressValidationExtensions
{
    public const int MaxStreetLength = 200;

    public const int MaxBuildingLength = 20;

    public const int MaxApartmentLength = 20;

    public const int MaxCityLength = 100;

    public const int MaxZipCodeLength = 20;

    public static IRuleBuilderOptions<T, string> ValidStreet<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Street must not be empty.")
            .MaximumLength(MaxStreetLength).WithMessage($"Street maximum length is {MaxStreetLength}.");
    }

    public static IRuleBuilderOptions<T, string> ValidBuilding<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Building must not be empty.")
            .MaximumLength(MaxBuildingLength).WithMessage($"Building maximum length is {MaxBuildingLength}.");
    }

    public static IRuleBuilderOptions<T, string?> ValidApartment<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(MaxApartmentLength).WithMessage($"Apartment maximum length is {MaxApartmentLength}.")
            .When(x => x != null);
    }

    public static IRuleBuilderOptions<T, string> ValidCity<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("City must not be empty.")
            .MaximumLength(MaxCityLength).WithMessage($"City maximum length is {MaxCityLength}.");
    }

    public static IRuleBuilderOptions<T, string> ValidZipCode<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Zip code must not be empty.")
            .MaximumLength(MaxZipCodeLength).WithMessage($"Zip code maximum length is {MaxZipCodeLength}.");
    }
}
