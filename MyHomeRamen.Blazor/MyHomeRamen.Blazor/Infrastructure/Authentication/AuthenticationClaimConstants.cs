using System.Security.Claims;

namespace MyHomeRamen.Blazor.Infrastructure.Authentication;

internal static class AuthenticationClaimConstants
{
    internal const string DomainId = "domain_id";
    internal const string FirstName = ClaimTypes.GivenName;
    internal const string AdminCanViewPanel = "my-home-ramen:admin:can-view-panel";
    internal const string AdminSection = "my-home-ramen:admin:section";
}
