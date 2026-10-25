namespace MyHomeRamen.Domain.Identity.Roles;

public static class RoleConstants
{
    public const string CompanyOwner = "Company Owner";
    public const string RestaurantAdmin = "Restaurant Admin";
    public const string Guest = "Guest";
    public const string Customer = "Customer";

    public static IEnumerable<string> AvailableRoles => [CompanyOwner, RestaurantAdmin, Guest, Customer];
}
