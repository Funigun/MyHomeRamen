using FluentValidation;
using MyHomeRamen.Domain.Common.CompanyDetails;
using MyHomeRamen.Domain.Restaurants.Companies;
using MyHomeRamen.Features.Common.Mediator;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Restaurants.Features.Abstractions;

namespace MyHomeRamen.Features.Restaurants.Features.Companies.RegisterOwner;

public sealed record RegisterCompanyOwnerCommand(RegisterCompanyOwnerRequest Request, string IdempotencyKey) : ICommand<RegisterCompanyOwnerResponse>;

public sealed class RegisterCompanyOwnerCommandValidator : AbstractValidator<RegisterCompanyOwnerCommand>
{
    public RegisterCompanyOwnerCommandValidator(IRestaurantDbContext restaurantDbContext)
    {
        RuleFor(x => x.IdempotencyKey)
            .NotEmpty()
                .WithMessage("Idempotency-Key is required.");

        RuleFor(x => x.Request.CompanyName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("Company name must not be empty.")
            .Must(companyName =>
                !string.IsNullOrWhiteSpace(Company.NormalizeName(companyName))
                && companyName.Length <= CompanyConstants.MaxNameLength)
                .WithMessage("Company name is invalid or exceeds the maximum length.")
            .MustAsync(async (command, companyName, cancellationToken) =>
                {
                    string normalizedName = Company.NormalizeName(companyName);
                    return await restaurantDbContext.Company.Query().IsNameUnique(normalizedName, cancellationToken);
                })
                .WithMessage("Company name must be unique.");
    }
}

public sealed class RegisterCompanyOwnerHandler(IIdentityService identityService, IRestaurantDbContext restaurantDbContext)
                  : IRequestHandler<RegisterCompanyOwnerCommand, RegisterCompanyOwnerResponse>
{
    public async Task<RegisterCompanyOwnerResponse> Handle(RegisterCompanyOwnerCommand command, CancellationToken cancellationToken)
    {
        Guid proposedCompanyId = Guid.CreateVersion7();

        CompanyOwnerRegistrationResult identityResult = await identityService.RegisterCompanyOwnerAsync(
            new CompanyOwnerDto(
                command.Request.UserName,
                command.Request.FirstName,
                command.Request.LastName,
                command.Request.Email,
                command.Request.PhoneNumber,
                command.Request.Password,
                command.Request.ConfirmPassword,
                proposedCompanyId,
                command.IdempotencyKey),
            cancellationToken);

        if (identityResult.Error is not null)
        {
            throw new ValidationException(identityResult.Error);
        }

        if (identityResult.UserId is null || identityResult.CompanyId is not Guid companyId)
        {
            throw new InvalidOperationException("Identity service returned an incomplete company owner registration result.");
        }

        CompanyId restaurantCompanyId = new(companyId);
        bool companyAlreadyExists = await restaurantDbContext.Company.Exists(company => company.Id == restaurantCompanyId, cancellationToken);
        
        if (!companyAlreadyExists)
        {
            Company company = Company.Create(restaurantCompanyId, command.Request.CompanyName, null, null);
            company.UpdateBusinessDetails(command.Request.CompanyName, "pending");
            restaurantDbContext.Company.Add(company);
            await restaurantDbContext.SaveChangesAsync(cancellationToken);
        }

        return new RegisterCompanyOwnerResponse(companyId);
    }
}
