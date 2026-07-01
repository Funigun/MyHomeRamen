using FluentValidation;
using MyHomeRamen.Blazor.Common.Models;

namespace MyHomeRamen.Blazor.Features.Account.AccountManagement.Components;

public sealed class AddressValidator : BaseValidator<AddressFormModel>
{
    public AddressValidator()
    {
        RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Building).NotEmpty().MaximumLength(20);
        RuleFor(x => x.Apartment).MaximumLength(20);
        RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(10);
    }
}
