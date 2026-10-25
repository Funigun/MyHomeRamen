using FluentValidation;
using Microsoft.Extensions.Localization;
using MyHomeRamen.Blazor.Components.Models;
using MyHomeRamen.Blazor.Features.Restaurants.Companies.Shared;

namespace MyHomeRamen.Blazor.Features.Restaurants.Companies.ConfigureCompanySetup.Models;

public sealed class ConfigureCompanyFormModel
{
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string TaxId { get; set; } = string.Empty;

    public ConfigureCompanyRequest ToRequest() => new(Name, LegalName, TaxId);
}

public sealed class ConfigureCompanyFormModelValidator : BaseValidator<ConfigureCompanyFormModel>
{
    public ConfigureCompanyFormModelValidator(IStringLocalizer<CompanySetupResources> text)
    {
        RuleFor(model => model.Name).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.LegalName).NotEmpty().WithMessage(text["Required"]);
        RuleFor(model => model.TaxId).NotEmpty().WithMessage(text["Required"]);
    }
}
