using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Domain.Restaurants.Users;

namespace MyHomeRamen.Features.Restaurants.Features.Abstractions;

internal static class CurrentUserExtensions
{
    extension(ICurrentUser currentUser)
    {
        public bool CanViewCompanyDetails() => currentUser.Permissions.Contains(PermissionConstants.CompanyView);
        public bool CanEditCompanyDetails() => currentUser.Permissions.Contains(PermissionConstants.CompanyEdit);

        public bool CanViewCompanySocialMedia() => currentUser.Permissions.Contains(PermissionConstants.CompanySocialMediaView);
        public bool CanEditCompanySocialMedia() => currentUser.Permissions.Contains(PermissionConstants.CompanySocialMediaEdit);

        public bool CanManageRestaurants() => currentUser.Permissions.Contains(PermissionConstants.RestaurantsManage);
        public bool CanAddRestaurant() => currentUser.Permissions.Contains(PermissionConstants.RestaurantsCreate);
        public bool CanManageRestaurant() => currentUser.Permissions.Contains(PermissionConstants.RestaurantManage);
        public bool CanEditRestaurantDetails() => currentUser.Permissions.Contains(PermissionConstants.RestaurantDetailsEdit);
        public bool CanEditRestaurantBankDetails() => currentUser.Permissions.Contains(PermissionConstants.RestaurantBankDetailsEdit);
        public bool CanEditRestaurantWorkingHours() => currentUser.Permissions.Contains(PermissionConstants.RestaurantWorkingHoursEdit);
        public bool CanEditRestaurantContactDetails() => currentUser.Permissions.Contains(PermissionConstants.RestaurantContactDetailsEdit);
        public bool CanEditRestaurantClosingPeriods() => currentUser.Permissions.Contains(PermissionConstants.RestaurantClosingPeriodsEdit);
    }
}
