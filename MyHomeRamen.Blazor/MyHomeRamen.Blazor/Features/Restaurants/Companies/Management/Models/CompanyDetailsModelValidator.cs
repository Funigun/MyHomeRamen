using FluentValidation;
using MyHomeRamen.Blazor.Components.Models;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.Management.Models;

public sealed class CompanyDetailsModelValidator : BaseValidator<CompanyDetailsEditModel>
{
    public CompanyDetailsModelValidator()
    {
        RuleFor(model => model.Description).NotEmpty();
        RuleFor(model => model.LegalName).NotEmpty();
        RuleFor(model => model.TaxId).NotEmpty();
    }
}
