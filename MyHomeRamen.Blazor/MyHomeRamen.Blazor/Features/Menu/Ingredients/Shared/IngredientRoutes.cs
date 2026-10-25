namespace MyHomeRamen.Blazor.Features.Menu.Ingredients.Shared;

public static class IngredientRoutes
{
    public const string Index = "/admin/menu/ingredients-management";
    public const string Create = "/admin/menu/ingredients/create";
    public const string EditTemplate = "/admin/menu/ingredients/{Id:guid}/edit";

    public static string Edit(Guid id) => $"/admin/menu/ingredients/{id}/edit";
}
