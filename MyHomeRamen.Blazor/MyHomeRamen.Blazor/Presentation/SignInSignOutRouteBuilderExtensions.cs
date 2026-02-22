using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MyHomeRamen.Blazor.Presentation;

internal static class SignInSignOutRouteBuilderExtensions
{
    internal static IEndpointConventionBuilder MapSignInSignOut(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("authentication");

        group.MapGet(pattern: "/login", OnLogin).AllowAnonymous();
        group.MapPost(pattern: "/logout", OnLogout);

        return group;
    }

    internal static ChallengeHttpResult OnLogin() =>
        TypedResults.Challenge(properties: new AuthenticationProperties
        {
            RedirectUri = "/"
        });

    internal static SignOutHttpResult OnLogout() =>
        TypedResults.SignOut(
            properties: new AuthenticationProperties { RedirectUri = "/" },
            [
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme
            ]);
}
