using FluentValidation;

namespace MyHomeRamen.Blazor.Features.Account.AccountManagement.Components;

public sealed class AddressValidator : AbstractValidator<AddressFormModel>
{
    public AddressValidator()
    {
        RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Building).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Apartment).MaximumLength(20);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(10);
    }

    public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        FluentValidation.Results.ValidationResult result = await ValidateAsync(ValidationContext<AddressFormModel>.CreateWithOptions((AddressFormModel)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
        {
            return Array.Empty<string>();
        }
        return result.Errors.Select(e => e.ErrorMessage);
    };
}
