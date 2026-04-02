using Microsoft.AspNetCore.Components;

namespace MyHomeRamen.Blazor.Features.Home.Common.Services;

public sealed class HomeNavigationService(NavigationManager navigation)
{
    public static class Routes
    {
        public const string Home = "/home";
    }

    public void NavigateToHome() => navigation.NavigateTo(Routes.Home);
}
