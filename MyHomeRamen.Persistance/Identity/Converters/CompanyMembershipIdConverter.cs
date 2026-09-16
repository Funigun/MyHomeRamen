using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyHomeRamen.Domain.Identity.CompanyMemberships;

namespace MyHomeRamen.Persistance.Identity.Converters;

public sealed class CompanyMembershipIdConverter() : ValueConverter<CompanyMembershipId, Guid>(id => id.Value, value => new CompanyMembershipId(value));
