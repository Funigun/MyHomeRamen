using FluentValidation;
using MyHomeRamen.Domain.Common.CompanyDetails;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;
using MyHomeRamen.Features.Restaurants.Permissions;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.UpdateDetails;

public sealed record UpdateCompanyDetailsCommand(UpdateCompanyDetailsRequest Request) : ICommand;

public sealed class UpdateCompanyDetailsAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<UpdateCompanyDetailsCommand>
{
    public async Task<bool> Authorize(UpdateCompanyDetailsCommand request, CancellationToken cancellationToken)
        => await Task.FromResult(currentUser.CanViewCompanyDetails() && currentUser.CanEditCompanyDetails());
}

public sealed class UpdateCompanyDetailsValidator : AbstractValidator<UpdateCompanyDetailsCommand>
{
    public UpdateCompanyDetailsValidator(IRestaurantDbContext dbContext)
    {
        RuleFor(x => x.Request.Description)
            .MaximumLength(CompanyConstants.MaxDescriptionLength)
            .WithMessage($"Description cannot exceed {CompanyConstants.MaxDescriptionLength} characters.");

        RuleFor(x => x.Request.LogoUrl)
            .MaximumLength(CompanyConstants.MaxLogoUrlLength)
            .WithMessage($"Logo URL cannot exceed {CompanyConstants.MaxLogoUrlLength} characters.");

        RuleFor(x => x.Request.BusinessDetails)
            .NotNull()
            .WithMessage("Business details must not be empty.");

        When(x => x.Request.BusinessDetails is not null, () =>
        {
            RuleFor(x => x.Request.BusinessDetails.LegalName)
                .NotEmpty().WithMessage("Legal name must not be empty.")
                .MaximumLength(CompanyConstants.MaxLegalNameLength)
                .WithMessage($"Legal name cannot exceed {CompanyConstants.MaxLegalNameLength} characters.");

            RuleFor(x => x.Request.BusinessDetails.TaxId)
                .NotEmpty().WithMessage("Tax ID must not be empty.")
                .MaximumLength(CompanyConstants.MaxTaxIdLength)
                .WithMessage($"Tax ID cannot exceed {CompanyConstants.MaxTaxIdLength} characters.");
        });

        RuleFor(_ => _)
            .MustAsync(async (_, cancellationToken) => await dbContext.Company.Query().CurrentDetails(cancellationToken) is not null)
            .WithMessage("Current company context is unavailable.");
    }
}

public sealed class UpdateCompanyDetailsHandler(IRestaurantDbContext dbContext) : IRequestHandler<UpdateCompanyDetailsCommand, Unit>
{
    public async Task<Unit> Handle(UpdateCompanyDetailsCommand command, CancellationToken cancellationToken)
    {
        Company company = await dbContext.Company.Load().Current(cancellationToken)
                       ?? throw new InvalidOperationException("Current company context is unavailable.");

        company.UpdateDetails(
            command.Request.Description,
            command.Request.LogoUrl,
            command.Request.BusinessDetails.LegalName,
            command.Request.BusinessDetails.TaxId);

        await dbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
