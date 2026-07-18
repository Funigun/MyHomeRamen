using Bogus;

namespace MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

public class MenuDataSet : DataSet
{
    private readonly string[] _productNames = ["Chintan Shoyu Ramen", "Paitan Miso Ramen", "Classic Tonkotsu Ramen", "Classic Shio Ramen"];

    private readonly Dictionary<string, string> productDescriptions = new()
    {
        {"Chintan Shoyu Ramen", "A classic soy sauce-based ramen with a rich and savory broth."},
        {"Paitan Miso Ramen", "A hearty miso-flavored ramen with a deep umami taste."},
        {"Classic Tonkotsu Ramen", "A creamy pork bone broth ramen with a rich flavor."},
        {"Classic Shio Ramen", "A light and delicate salt-based ramen."}
    };

    private readonly string[] _productCategories = ["Ramen", "Mazesoba", "Tsukemen"];
    private readonly string[] _ingredientCategories = ["Noodles", "Broth", "Toppings", "Seasonings", "Aroma Oils"];

    public string ProductName() => Random.ArrayElement(_productNames);

    public string ProductDescription(string productName) => productDescriptions[productName];

    public string ProductCategory() => Random.ArrayElement(_productCategories);

    public string IngredientCategory() => Random.ArrayElement(_ingredientCategories);
}
