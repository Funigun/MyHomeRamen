using FluentValidation;
using MyHomeRamen.Api.Common.Authorization;
using MyHomeRamen.Api.Common.Extentsions;
using MyHomeRamen.Api.Users.Features.Account.UpdateAddress.Models;
using MyHomeRamen.Common.Contracts.Account;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Persistance.Users.Extensions;

namespace MyHomeRamen.Api.Users.Features.Account.UpdateAddress.Policies;

public sealed class UpdateAddressValidationPolicy : AbstractValidator<UpdateAddressRequest>
{
    public UpdateAddressValidationPolicy(IUsersDbContext dbContext, IHttpContextAccessor httpContextAccessor, ICurrentUser currentUser)
    {
        RuleFor(x => x.Street)
            .ValidStreet();

        RuleFor(x => x.Building)
            .ValidBuilding();

        RuleFor(x => x.Apartment)
            .ValidApartment();

        RuleFor(x => x.City)
            .ValidCity();

        RuleFor(x => x.ZipCode)
            .ValidZipCode();

        RuleFor(x => x.Id)
            .MustAsync(async (id, cancellationToken) =>
            {
                Guid addressId = httpContextAccessor.GetGuidFromRouteParam("id");
                return await dbContext.Users.AddressExists(currentUser.UserId, addressId, cancellationToken);
            })
            .WithMessage("Address not found or does not belong to the current user.");
    }
}
