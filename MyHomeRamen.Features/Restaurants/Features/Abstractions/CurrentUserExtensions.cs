using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Permissions;

namespace MyHomeRamen.Features.Restaurants.Features.Abstractions;

internal static class CurrentUserExtensions
{
    extension(ICurrentUser currentUser)
    {
        public bool CanViewCompanyDetails() => currentUser.Permissions.Contains(RestaurantsPermissionConstants.CompanyView);
        public bool CanEditCompanyDetails() => currentUser.Permissions.Contains(RestaurantsPermissionConstants.CompanyEdit);

        public bool CanViewCompanySocialMedia() => currentUser.Permissions.Contains(RestaurantsPermissionConstants.CompanySocialMediaView);
        public bool CanEditCompanySocialMedia() => currentUser.Permissions.Contains(RestaurantsPermissionConstants.CompanySocialMediaEdit);

        public bool CanManageRestaurants() => currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantsManage);
        public bool CanAddRestaurant() => currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantsCreate);
        public bool CanManageRestaurant() => currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantManage);
        public bool CanEditRestaurantDetails() => currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantDetailsEdit);
        public bool CanEditRestaurantBankDetails() => currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantBankDetailsEdit);
        public bool CanEditRestaurantWorkingHours() => currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantWorkingHoursEdit);
        public bool CanEditRestaurantContactDetails() => currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantContactDetailsEdit);
        public bool CanEditRestaurantClosingPeriods() => currentUser.Permissions.Contains(RestaurantsPermissionConstants.RestaurantClosingPeriodsEdit);
    }
}
