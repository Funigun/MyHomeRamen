namespace MyHomeRamen.Blazor.Features.Menu.Products.Shared;

public static class ProductsRoutes
{
    public const string Index = "/admin/menu/products-management";
    private const string EditRoot = "/admin/menu/products";
    public const string Create = EditRoot + "/create";
    public const string EditTemplate = EditRoot + "/{Id:guid}/edit";

    public static string Edit(Guid id) => $"{EditRoot}/{id}/edit";
}
