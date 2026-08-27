using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyHomeRamen.Domain.Identity.Permissions;
using MyHomeRamen.Domain.Identity.Roles;
using MyHomeRamen.Domain.Identity.Users;
using MyHomeRamen.Features;
using MyHomeRamen.Features.Common.Authorization;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Identity.ExternalApi;
using MyHomeRamen.Features.Identity.Features.Permissions.Common;
using MyHomeRamen.Features.Identity.Features.Roles.Common;
using MyHomeRamen.Features.Identity.Features.Users.Common;
using MyHomeRamen.Features.Identity.Services;
using MyHomeRamen.Infrastructure.Cache;
using MyHomeRamen.Persistance.Identity;

namespace MyHomeRamen.IntegrationTests.Identity;

public class IdentityTestData
{
    private IIdentityDbContext IdentityDbContext { get; set; } = null!;

    private ServiceCollection? _serviceCollection = null;

    public async Task SetIdentityService(ICurrentUser user, string connectionString)
    {
        DbContextOptions<IdentityDbContext> options = new DbContextOptionsBuilder<IdentityDbContext>()
                                                          .UseSqlServer(connectionString)
                                                          .Options;

        if (_serviceCollection is null)
        {
            _serviceCollection = new();
            _serviceCollection.AddSingleton(options);
            _serviceCollection.AddScoped(provider => user);
            _serviceCollection.AddScoped(provider => new IdentityDbContext(options, user, provider));
            _serviceCollection.AddScoped<IIdentityDbContext>(provider => provider.GetRequiredService<IdentityDbContext>());
            _serviceCollection.AddScoped<IUserRepository, UserRepository>();
            _serviceCollection.AddScoped<IRoleRepository, RoleRepository>();
            _serviceCollection.AddScoped<IPermissionRepository, PermissionRepository>();
            _serviceCollection.AddPermissionCatalogServices();
            _serviceCollection.AddScoped<IPermissionCatalogSynchronizer, PermissionCatalogSynchronizer>();
            _serviceCollection.AddCacheService();
        }
        
        ServiceProvider serviceProvider = _serviceCollection.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();

        IdentityDbContext seedDbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        await seedDbContext.Database.MigrateAsync();

        IdentityDbContext = scope.ServiceProvider.GetRequiredService<IIdentityDbContext>();
        await SeedAsync(scope);
    }

    private async Task SeedAsync(IServiceScope seedScope)
    {
        IPermissionCatalogSynchronizer permissionCatalogSynchronizer = seedScope.ServiceProvider.GetRequiredService<IPermissionCatalogSynchronizer>();
        await permissionCatalogSynchronizer.Synchronize(TestContext.Current.CancellationToken);
    }

    public async Task<(string KeycloakId, Guid UserId)> SeedUser((string roleName, IEnumerable<string> permissions) role, string userName, string name)
    {
        ServiceProvider serviceProvider = _serviceCollection!.BuildServiceProvider();
        using IServiceScope scope = serviceProvider.CreateScope();
        IdentityDbContext = scope.ServiceProvider.GetRequiredService<IIdentityDbContext>();

        string keycloakUserId = $"test-keycloak-{userName}";

        IEnumerable<Permission> permissions = await IdentityDbContext.Permission.Load().All(TestContext.Current.CancellationToken);
        IEnumerable<PermissionId> permissionIds = permissions.Where(p => role.permissions.Contains(p.Name)).Select(p => p.Id);

        Role roleToSeed = Role.Create(role.roleName, $"{role.roleName} role for testing purposes", permissionIds);
        User user = User.Create(keycloakUserId, userName, name, "User", $"{userName}@example.com", "123456789", roleToSeed);

        IdentityDbContext.Role.Add(roleToSeed);
        IdentityDbContext.User.Add(user);
        await IdentityDbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        IEnumerable<Permission> userPermissions = await IdentityDbContext.Permission.Query().ByUserId(user.Id.Value, TestContext.Current.CancellationToken);

        return (keycloakUserId, user.Id);
    }
}
