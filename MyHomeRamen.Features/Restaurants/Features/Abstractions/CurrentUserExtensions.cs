using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Domain.Restaurants.Users;

namespace MyHomeRamen.Features.Restaurants.Features.Abstractions;

internal static class CurrentUserExtensions
{
    private const string ModuleName = "Restaurants";

    extension(ICurrentUser currentUser)
    {
        public bool CanViewCompanyDetails() => currentUser.HasPermission(ModuleName, PermissionConstants.CompanyView);
        public bool CanEditCompanyDetails() => currentUser.HasPermission(ModuleName, PermissionConstants.CompanyEdit);

        public bool CanViewCompanySocialMedia() => currentUser.HasPermission(ModuleName, PermissionConstants.CompanySocialMediaView);
        public bool CanEditCompanySocialMedia() => currentUser.HasPermission(ModuleName, PermissionConstants.CompanySocialMediaEdit);

        public bool CanManageRestaurants() => currentUser.HasPermission(ModuleName, PermissionConstants.RestaurantsManage);
        public bool CanManageAllRestaurants()
            => currentUser.CanManageRestaurants()
               || currentUser.HasPermission(ModuleName, PermissionConstants.RestaurantManage);

        public bool CanAddRestaurant() => currentUser.HasPermission(ModuleName, PermissionConstants.RestaurantsCreate);
        public bool CanManageAnyRestaurant()
            => currentUser.CanManageAllRestaurants()
               || currentUser.HasAnyPermission(ModuleName, PermissionConstants.RestaurantManage);

        public bool CanManageRestaurant(Guid restaurantId)
            => currentUser.CanManageAllRestaurants()
               || currentUser.HasPermission(ModuleName, PermissionConstants.RestaurantManage, restaurantId);

        public IReadOnlyCollection<Guid> ManagedRestaurantIds()
            => currentUser.RestaurantIdsFor(ModuleName, PermissionConstants.RestaurantManage);

        public bool CanEditRestaurantDetails(Guid restaurantId) => currentUser.HasPermission(ModuleName, PermissionConstants.RestaurantDetailsEdit, restaurantId);
        public bool CanEditRestaurantBankDetails(Guid restaurantId) => currentUser.HasPermission(ModuleName, PermissionConstants.RestaurantBankDetailsEdit, restaurantId);
        public bool CanEditRestaurantWorkingHours(Guid restaurantId) => currentUser.HasPermission(ModuleName, PermissionConstants.RestaurantWorkingHoursEdit, restaurantId);
        public bool CanEditRestaurantContactDetails(Guid restaurantId) => currentUser.HasPermission(ModuleName, PermissionConstants.RestaurantContactDetailsEdit, restaurantId);
        public bool CanEditRestaurantClosingPeriods(Guid restaurantId) => currentUser.HasPermission(ModuleName, PermissionConstants.RestaurantClosingPeriodsEdit, restaurantId);
    }
}
