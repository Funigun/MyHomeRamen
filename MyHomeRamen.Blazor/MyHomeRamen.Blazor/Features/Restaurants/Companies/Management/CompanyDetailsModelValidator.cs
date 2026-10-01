using FluentValidation;
using MyHomeRamen.Blazor.Components.Models;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Management;

public sealed class CompanyDetailsModelValidator : BaseValidator<CompanyDetailsFormModel>
{
    public CompanyDetailsModelValidator()
    {
        RuleFor(model => model.Description).NotEmpty();
        RuleFor(model => model.LegalName).NotEmpty();
        RuleFor(model => model.TaxId).NotEmpty();
    }
}
