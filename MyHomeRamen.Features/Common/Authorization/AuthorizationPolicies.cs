namespace MyHomeRamen.Features.Common.Authorization;

public static class AuthorizationPolicies
{
    public const string RestaurantCustomerPolicy = "RestaurantCustomer";
    public const string RestaurantEmployeePolicy = "RestaurantEmployee";
    public const string RestaurantManagerPolicy = "RestaurantManager";
    public const string AnyAuthenticatedPolicy = "AnyAuthenticated";
    public const string AuthenticatedUserPolicy = "AuthenticatedUser";
}
