using Bogus;
using MyHomeRamen.Domain.ShoppingCart.PaymentDetails;
using MyHomeRamen.Domain.ShoppingCart.ShippingDetails;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;

internal sealed class ShoppingCartDataSet : DataSet
{
    internal static readonly IEnumerable<Guid> OriginalProductIds = [ Guid.Parse("9942e0d8-c7ec-4171-8d54-deb0f5d7db90"), Guid.Parse("9942e0d8-c7ec-4171-8d54-deb0f5d7db91"), Guid.Parse("9942e0d8-c7ec-4171-8d54-deb0f5d7db92"), Guid.Parse("9942e0d8-c7ec-4171-8d54-deb0f5d7db93")];
    internal static readonly Dictionary<Guid, Guid> PaymentMethods = new()
    {
        { Guid.Parse("2042e0d8-c7ec-4171-8d54-deb0f5d7db90"), Guid.Parse("3042e0d8-c7ec-4171-8d54-deb0f5d7db94") },
    };

    private readonly string[] _productNames = ["Chintan Shoyu Ramen", "Paitan Miso Ramen", "Classic Tonkotsu Ramen", "Classic Shio Ramen"];

    private readonly Dictionary<string, string> productDescriptions = new()
    {
        {"Chintan Shoyu Ramen", "A classic soy sauce-based ramen with a rich and savory broth."},
        {"Paitan Miso Ramen", "A hearty miso-flavored ramen with a deep umami taste."},
        {"Classic Tonkotsu Ramen", "A creamy pork bone broth ramen with a rich flavor."},
        {"Classic Shio Ramen", "A light and delicate salt-based ramen."}
    };

    private readonly Dictionary<string, Guid> productOriginalIds = new()
    {
        {"Chintan Shoyu Ramen", Guid.Parse("9942e0d8-c7ec-4171-8d54-deb0f5d7db90")},
        {"Paitan Miso Ramen", Guid.Parse("9942e0d8-c7ec-4171-8d54-deb0f5d7db91")},
        {"Classic Tonkotsu Ramen", Guid.Parse("9942e0d8-c7ec-4171-8d54-deb0f5d7db92")},
        {"Classic Shio Ramen", Guid.Parse("9942e0d8-c7ec-4171-8d54-deb0f5d7db93")}
    };

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

    public Guid ProductOriginalId(string productName) => productOriginalIds[productName];

    public string IngredientName() => Random.ArrayElement(_ingredientNames);

    public string IngredientDescription(string ingredientName) => ingredientDescriptions[ingredientName];

    public ShippingDetails DeliveryShippingDetails() => ShippingDetails.CreateDelivery(new("Test street", "Test building", "Test apartment", "Test city", "12345"));

    public ShippingDetails PersonalPickupShippingDetails() => ShippingDetails.CreatePersonalPickup();

    public PaymentDetails CashPaymentDetails() => PaymentDetails.Create(PaymentMethods.Keys.First().ToString(), PaymentMethods.Values.First().ToString());
}
