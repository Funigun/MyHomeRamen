using FluentValidation;
using MyHomeRamen.Blazor.Components.Models;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.CompanyDetails.Models;

public sealed class CompanyDetailsFormModelValidator : BaseValidator<CompanyDetailsFormModel>
{
    public CompanyDetailsFormModelValidator()
    {
        RuleFor(model => model.Description).NotEmpty();
        RuleFor(model => model.LegalName).NotEmpty();
        RuleFor(model => model.TaxId).NotEmpty();
    }
}
