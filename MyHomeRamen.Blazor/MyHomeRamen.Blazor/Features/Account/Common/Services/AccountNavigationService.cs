using Microsoft.AspNetCore.Components;

namespace MyHomeRamen.Blazor.Features.Account.Common.Services;

public sealed class AccountNavigationService(NavigationManager navigation)
{
    public static class Routes
    {
        public const string SignUp = "/account/signup";
    }

    public void ToSignUp() => navigation.NavigateTo(Routes.SignUp);
}
