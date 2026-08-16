using Bogus;
using Bogus.Premium;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;

internal static class DataSeeder
{
    extension(Faker faker)
    {
        internal ShoppingCartDataSet ShoppingCart() => ContextHelper.GetOrSet(faker, () => new ShoppingCartDataSet());
    }
}
