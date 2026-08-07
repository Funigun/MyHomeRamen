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

    private readonly string[] _ingredientNames = ["Chashu Pork", "Ajitama Egg", "Menma Bamboo Shoots", "Nori Seaweed", "Green Onions", "Garlic Chips", "Sesame Seeds", "Chili Oil"];

    private readonly Dictionary<string, string> ingredientDescriptions = new()
    {
        {"Chashu Pork", "Tender slices of braised pork belly."},
        {"Ajitama Egg", "Soft-boiled egg marinated in soy sauce."},
        {"Menma Bamboo Shoots", "Fermented bamboo shoots with a savory flavor."},
        {"Nori Seaweed", "Dried seaweed sheets for garnish."},
        {"Green Onions", "Freshly chopped green onions for added flavor."},
        {"Garlic Chips", "Crispy fried garlic slices."},
        {"Sesame Seeds", "Toasted sesame seeds for a nutty taste."},
        {"Chili Oil", "Spicy chili-infused oil for heat."}
    };

    public string ProductName() => Random.ArrayElement(_productNames);

    public string ProductDescription(string productName) => productDescriptions[productName];

    public string ProductCategory() => Random.ArrayElement(_productCategories);

    public string IngredientCategory() => Random.ArrayElement(_ingredientCategories);

    public IEnumerable<string> IngredientCategoryNames() => _ingredientCategories;

    public string IngredientName() => Random.ArrayElement(_ingredientNames);

    public string IngredientDescription(string ingredientName) => ingredientDescriptions[ingredientName];
}
