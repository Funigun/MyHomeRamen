namespace MyHomeRamen.Features.Common.Tenancy;

public interface ICompanyContext
{
    Guid? CompanyId { get; }

    Guid? RestaurantId { get; }

    void SetRestaurant(Guid restaurantId, Guid companyId);
}
