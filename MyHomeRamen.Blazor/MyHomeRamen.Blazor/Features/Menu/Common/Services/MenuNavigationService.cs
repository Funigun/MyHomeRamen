using Microsoft.AspNetCore.Components;

namespace MyHomeRamen.Blazor.Features.Menu.Common.Services;

public sealed class MenuNavigationService(NavigationManager navigation)
{
    public static class Routes
    {
        public const string List = "/menu/products";
        public const string Create = "/menu/products/create";

        public static string Detail(Guid id) => $"/menu/products/{id}";

        public static string Edit(Guid id) => $"/menu/products/{id}/edit";
    }

    public void ToList() => navigation.NavigateTo(Routes.List);

    public void ToCreate() => navigation.NavigateTo(Routes.Create);

    public void ToDetail(Guid id) => navigation.NavigateTo(Routes.Detail(id));

    public void ToEdit(Guid id) => navigation.NavigateTo(Routes.Edit(id));
}
