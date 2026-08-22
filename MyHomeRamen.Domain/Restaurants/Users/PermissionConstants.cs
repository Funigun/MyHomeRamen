namespace MyHomeRamen.Domain.Restaurants.Users;

public static class PermissionConstants
{
    public const string CompanyDetailsEdit = "CompanyDetails.Edit";
    public const string CompanySocialMediaEdit = "CompanyDetails.SocialMedia.Edit";
    public const string CompanyBusinessDetailsEdit = "CompanyDetails.BusinessDetails.Edit";

    public const string RestaurantDetailsEdit = "RestaurantDetails.Edit";
    public const string RestaurantBankDetailsEdit = "RestaurantDetails.BankDetails.Edit";
    public const string RestaurantWorkingHoursEdit = "RestaurantDetails.WorkingHours.Edit";
    public const string RestaurantContactDetailsEdit = "RestaurantDetails.ContactDetails.Edit";
    public const string RestaurantClosingPeriodsEdit = "RestaurantDetails.ClosingPeriods.Edit";

    public static IEnumerable<string> AvailablePermissions =>
    [
        CompanyDetailsEdit,
        CompanySocialMediaEdit,
        CompanyBusinessDetailsEdit,
        RestaurantDetailsEdit,
        RestaurantBankDetailsEdit,
        RestaurantWorkingHoursEdit,
        RestaurantContactDetailsEdit,
        RestaurantClosingPeriodsEdit
    ];
}
