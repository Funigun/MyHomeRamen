namespace MyHomeRamen.Domain.Identity.CompanyMemberships;

internal static class CompanyMembershipValidator
{
    internal static void ValidateCompanyMembership(CompanyMembership membership)
    {
        if (membership.UserId.Value == Guid.Empty)
        {
            throw CompanyMembershipErrors.UserRequired();
        }

        if (membership.CompanyId == Guid.Empty)
        {
            throw CompanyMembershipErrors.CompanyRequired();
        }

        if (membership.RestaurantId == Guid.Empty)
        {
            throw CompanyMembershipErrors.RestaurantIdCannotBeEmpty();
        }
    }
}
