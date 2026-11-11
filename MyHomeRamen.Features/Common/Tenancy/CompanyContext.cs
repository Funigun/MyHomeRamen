namespace MyHomeRamen.Features.Common.Tenancy;

public sealed class CompanyContext : ICompanyContext
{
    public Guid? CompanyId { get; private set; }

    public Guid? RestaurantId { get; private set; }

    public void SetCompany(Guid companyId)
    {
        if (companyId == Guid.Empty)
        {
            throw new ArgumentException("Company identifier must not be empty.", nameof(companyId));
        }

        if (CompanyId.HasValue && CompanyId != companyId)
        {
            throw new InvalidOperationException("Request company context cannot be changed after it is set.");
        }

        CompanyId = companyId;
    }

    public void SetRestaurant(Guid restaurantId, Guid companyId)
    {
        if (restaurantId == Guid.Empty || companyId == Guid.Empty)
        {
            throw new ArgumentException("Restaurant and company identifiers must not be empty.");
        }

        if (RestaurantId.HasValue && RestaurantId != restaurantId)
        {
            throw new InvalidOperationException("Request company context cannot be changed after it is set.");
        }

        SetCompany(companyId);
        RestaurantId = restaurantId;
    }
}
