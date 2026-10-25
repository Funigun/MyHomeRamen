using FluentValidation;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Common.Endpoints.Policies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.GetDetails;

public sealed record CompanyDetailsDto(Guid Id, string Name, string? Description, string? LogoUrl, string LegalName, string TaxId);

public sealed record GetCompanyDetailsQuery : IQuery<GetCompanyDetailsResponse>;

public sealed class GetCompanyDetailsAuthorizationPolicy(ICurrentUser currentUser) : IAuthorizationPolicy<GetCompanyDetailsQuery>
{
    public async Task<bool> Authorize(GetCompanyDetailsQuery request, CancellationToken cancellationToken)
        => await Task.FromResult(currentUser.CanViewCompanyDetails());
}

public sealed class GetCompanyDetailsValidator : AbstractValidator<GetCompanyDetailsQuery>
{
    public GetCompanyDetailsValidator(IRestaurantDbContext dbContext)
    {
        RuleFor(_ => _)
            .MustAsync(async (_, cancellationToken) => await dbContext.Company.Query().CurrentDetails(cancellationToken) is not null)
            .WithMessage("Current company context is unavailable.");
    }
}

public sealed class GetCompanyDetailsHandler(IRestaurantDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<GetCompanyDetailsQuery, GetCompanyDetailsResponse>
{
    public async Task<GetCompanyDetailsResponse> Handle(GetCompanyDetailsQuery query, CancellationToken cancellationToken)
    {
        CompanyDetailsDto? company = await dbContext.Company.Query().CurrentDetails(cancellationToken);

        return company is null
            ? throw new InvalidOperationException("Current company context is unavailable.")
            : company.ToResponse(currentUser.CanEditCompanyDetails());
    }
}

internal static class Mappings
{
    internal static GetCompanyDetailsResponse ToResponse(this CompanyDetailsDto company, bool canEditCompanyDetails)
        => new(
            company.Id,
            company.Name,
            company.Description,
            company.LogoUrl,
            new BusinessDetailsDto(company.LegalName, company.TaxId),
            new AllowedActionsDto(canEditCompanyDetails));
}
