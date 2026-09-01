using Bogus;
using Bogus.Premium;

namespace MyHomeRamen.MenuApi.IntegrationTests.Common.Data;

internal static class DataSeeder
{
    extension(Faker faker)
    {
        internal MenuDataSet RamenMenu() => ContextHelper.GetOrSet(faker, () => new MenuDataSet());
    }
}
