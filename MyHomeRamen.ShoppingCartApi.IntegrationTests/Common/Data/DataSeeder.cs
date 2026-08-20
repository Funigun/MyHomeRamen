using Bogus;
using Bogus.Premium;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;

internal static class DataSeeder
{
    extension(Faker faker)
    {
        internal ShoppingCartDataSet ShoppingCart() => ContextHelper.GetOrSet(faker, () => new ShoppingCartDataSet());
    }
}
