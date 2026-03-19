using Microsoft.AspNetCore.Components;

namespace MyHomeRamen.Blazor.Features.Admin.Common.Services;

public sealed class AdminNavigationService(NavigationManager navigation)
{
    public static class Routes
    {
        public const string EmployeeList = "/admin/employees";
        public const string EmployeeCreate = "/admin/employees/create";

        public static string EmployeeDetail(string id) => $"/admin/employees/{id}";

        public static string EmployeeEdit(string id) => $"/admin/employees/{id}/edit";
    }

    public void ToEmployeeList() => navigation.NavigateTo(Routes.EmployeeList);

    public void ToEmployeeCreate() => navigation.NavigateTo(Routes.EmployeeCreate);

    public void ToEmployeeDetail(string id) => navigation.NavigateTo(Routes.EmployeeDetail(id));

    public void ToEmployeeEdit(string id) => navigation.NavigateTo(Routes.EmployeeEdit(id));
}
