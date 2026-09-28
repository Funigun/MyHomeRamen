using System.Security.Claims;

namespace MyHomeRamen.Blazor.Infrastructure.Authentication;

internal static class AuthenticationClaimConstants
{
    internal const string DomainId = "domain_id";
    internal const string FirstName = ClaimTypes.GivenName;
    internal const string AdminCanViewPanel = "my-home-ramen:admin:can-view-panel";
    internal const string OwnerCanViewPanel = "my-home-ramen:owner:can-view-panel";
}
