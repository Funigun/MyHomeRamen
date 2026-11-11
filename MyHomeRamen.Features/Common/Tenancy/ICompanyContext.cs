namespace MyHomeRamen.Features.Common.Tenancy;

public interface ICompanyContext
{
    Guid? CompanyId { get; }

    Guid? RestaurantId { get; }

    void SetCompany(Guid companyId);

    void SetRestaurant(Guid restaurantId, Guid companyId);
}
