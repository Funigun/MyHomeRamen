using FluentValidation;
using MyHomeRamen.Domain.Common.Address;

namespace MyHomeRamen.Features.Identity.Features.Users.Common;

internal static class AccountValidatorExtensions
{
    private const int MaxUserNameLength = 50;
    private const int MaxNameLength = 50;
    private const int MinPasswordLength = 8;

    public static IRuleBuilderOptions<T, string> ValidUserName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("User name must not be empty.")
            .MaximumLength(MaxUserNameLength).WithMessage($"User name maximum length is {MaxUserNameLength}.");
    }

    public static IRuleBuilderOptions<T, string> ValidName<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Name must not be empty.")
            .MaximumLength(MaxNameLength).WithMessage($"Name maximum length is {MaxNameLength}.");
    }

    public static IRuleBuilderOptions<T, string> ValidPassword<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Password must not be empty.")
            .MinimumLength(MinPasswordLength).WithMessage($"Password minimum length is {MinPasswordLength}.");
    }

    public static IRuleBuilderOptions<T, string> ValidStreet<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Street must not be empty.")
            .MaximumLength(AddressConstants.MaxStreetLength).WithMessage($"Street maximum length is {AddressConstants.MaxStreetLength}.");
    }

    public static IRuleBuilderOptions<T, string> ValidBuilding<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Building must not be empty.")
            .MaximumLength(AddressConstants.MaxBuildingLength).WithMessage($"Building maximum length is {AddressConstants.MaxBuildingLength}.");
    }

    public static IRuleBuilderOptions<T, string?> ValidApartment<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder
            .MaximumLength(AddressConstants.MaxApartmentLength).WithMessage($"Apartment maximum length is {AddressConstants.MaxApartmentLength}.")
            .When(x => x is not null);
    }

    public static IRuleBuilderOptions<T, string> ValidCity<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("City must not be empty.")
            .MaximumLength(AddressConstants.MaxCityLength).WithMessage($"City maximum length is {AddressConstants.MaxCityLength}.");
    }

    public static IRuleBuilderOptions<T, string> ValidZipCode<T>(this IRuleBuilder<T, string> ruleBuilder)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("Zip code must not be empty.")
            .MaximumLength(AddressConstants.MaxZipCodeLength).WithMessage($"Zip code maximum length is {AddressConstants.MaxZipCodeLength}.");
    }
}
