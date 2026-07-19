using Bogus;
using Bogus.Premium;
using MyHomeRamen.Domain.ShoppingCart.Roles;
using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;

namespace MyHomeRamen.ShoppingCartApi.IntegrationTests.Common.Data;

internal static class DataSeeder
{
     internal static async Task SeedDatabase(IShoppingCartDbContext dbContext)
    {
        await dbContext.Migrate(TestContext.Current.CancellationToken);
        await dbContext.Seed(TestContext.Current.CancellationToken);

        IEnumerable<Role> rolesList = await dbContext.Role.Specification().GetAllWithPermissions(TestContext.Current.CancellationToken);
        IEnumerable<User> users = GenerateUsers(rolesList);

        dbContext.User.AddRange(users);
        await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static IEnumerable<User> GenerateUsers(IEnumerable<Role> roles)
    {
        List<Role> rolesList = roles.ToList();
        
        Role adminRole = rolesList.First(r => r.Name == RoleConstants.Admin);
        User admin = User.Create(new UserId(Guid.NewGuid()), [adminRole], adminRole.Permissions.ToList());

        IEnumerable<string> employeeRoles = [RoleConstants.Employee, RoleConstants.Waiter, RoleConstants.Chef];
        List<Role> employeeRole = rolesList.Where(r => employeeRoles.Contains(r.Name)).ToList();
        User employee = User.Create(new UserId(Guid.NewGuid()), employeeRole, employeeRole.SelectMany(role => role.Permissions).ToList());

        Role customerRole = rolesList.First(r => r.Name == RoleConstants.Customer);
        User customer = User.Create(new UserId(Guid.NewGuid()), [customerRole], customerRole.Permissions.ToList());

        User guest = User.CreateGuest(Guid.NewGuid());

        return [admin, employee, customer, guest];
    }

    extension(Faker faker)
    {
        internal ShoppingCartDataSet ShoppingCart() => ContextHelper.GetOrSet(faker, () => new ShoppingCartDataSet());
    }
}
