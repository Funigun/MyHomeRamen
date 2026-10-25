using FluentValidation;
using MyHomeRamen.Domain.Common.CompanyDetails;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.ConfigureCompany;

public sealed record ConfigureCompanyRequest(string Name, string LegalName, string TaxId);

public sealed record ConfigureCompanyResponse(Guid CompanyId);

public sealed record ConfigureCompanyCommand(ConfigureCompanyRequest Request)
    : ICommand<ConfigureCompanyResponse>;

public sealed class ConfigureCompanyCommandValidator : AbstractValidator<ConfigureCompanyCommand>
{
    public ConfigureCompanyCommandValidator(
        IRestaurantDbContext restaurantDbContext,
        IIdentityService identityService)
    {
        RuleFor(_ => _)
            .MustAsync(async (_, cancellationToken) =>
                await identityService.IsCompanyOwnerConfiguredAsync(cancellationToken))
            .WithMessage("A company owner must be registered before company configuration.");

        RuleFor(_ => _)
            .MustAsync(async (_, cancellationToken) =>
                !await restaurantDbContext.Company.Exists(_ => true, cancellationToken))
            .WithMessage("A company has already been configured for this deployment.");

        RuleFor(command => command.Request.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage("Company name must not be empty.")
            .Must(name =>
                !string.IsNullOrWhiteSpace(Company.NormalizeName(name))
                && name.Length <= CompanyConstants.MaxNameLength)
            .WithMessage("Company name is invalid or exceeds the maximum length.")
            .MustAsync(async (name, cancellationToken) =>
                await restaurantDbContext.Company.Query().IsNameUnique(Company.NormalizeName(name), cancellationToken))
            .WithMessage("Company name must be unique.");

        RuleFor(command => command.Request.LegalName)
            .NotEmpty()
            .WithMessage("Company legal name must not be empty.")
            .MaximumLength(CompanyConstants.MaxLegalNameLength)
            .WithMessage($"Company legal name cannot exceed {CompanyConstants.MaxLegalNameLength} characters.");

        RuleFor(command => command.Request.TaxId)
            .NotEmpty()
            .WithMessage("Company tax ID must not be empty.")
            .MaximumLength(CompanyConstants.MaxTaxIdLength)
            .WithMessage($"Company tax ID cannot exceed {CompanyConstants.MaxTaxIdLength} characters.");
    }
}

public sealed class ConfigureCompanyHandler(IRestaurantDbContext restaurantDbContext)
    : IRequestHandler<ConfigureCompanyCommand, ConfigureCompanyResponse>
{
    public async Task<ConfigureCompanyResponse> Handle(
        ConfigureCompanyCommand command,
        CancellationToken cancellationToken)
    {
        Company company = Company.Create(
            command.Request.Name,
            null,
            null,
            command.Request.LegalName,
            command.Request.TaxId);

        restaurantDbContext.Company.Add(company);
        await restaurantDbContext.SaveChangesAsync(cancellationToken);

        return new ConfigureCompanyResponse(company.Id.Value);
    }
}
