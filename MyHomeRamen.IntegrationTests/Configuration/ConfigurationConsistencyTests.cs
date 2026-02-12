using System.Data.Common;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using MyHomeRamen.Api.Common.Configuration;

namespace MyHomeRamen.IntegrationTests.Configuration;

public sealed class ConfigurationConsistencyTests : IAsyncLifetime
{
    private static RestaurantConfigurationProvider _appHostConfiguration;
    private static RestaurantConfigurationProvider _identityConfiguration;
    private static RestaurantConfigurationProvider _apiConfiguration;
    private static RestaurantConfigurationProvider _blazorConfiguration;
    private static RestaurantConfigurationProvider _emailWorkerConfiguration;
    private static RestaurantConfigurationProvider _messagesWorkerConfiguration;

    public ValueTask InitializeAsync()
    {
        string solutionRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../"));

        string appHostConfig = Path.Combine(solutionRoot, "MyHomeRamen.AppHost", "appsettings.Development.json");
        string identityConfig = Path.Combine(solutionRoot, "MyHomeRamen.Identity.Api", "appsettings.Development.json");
        string apiConfig = Path.Combine(solutionRoot, "MyHomeRamen.Api", "appsettings.Development.json");
        string blazorConfig = Path.Combine(solutionRoot, "MyHomeRamen.Blazor", "MyHomeRamen.Blazor", "appsettings.Development.json");
        string emailWorkerConfig = Path.Combine(solutionRoot, "MyHomeRamen.Worker.MailSender", "appsettings.Development.json");
        string messagesWorkerConfig = Path.Combine(solutionRoot, "MyHomeRamen.Worker.MessagesHandler", "appsettings.Development.json");

        IConfigurationRoot? config = new ConfigurationBuilder().AddJsonFile(appHostConfig, optional: false).Build();
        _appHostConfiguration = new(config);

        config = new ConfigurationBuilder().AddJsonFile(identityConfig, optional: false).Build();
        _identityConfiguration = new(config);

        config = new ConfigurationBuilder().AddJsonFile(apiConfig, optional: false).Build();
        _apiConfiguration = new(config);

        config = new ConfigurationBuilder().AddJsonFile(blazorConfig, optional: false).Build();
        _blazorConfiguration = new(config);

        config = new ConfigurationBuilder().AddJsonFile(emailWorkerConfig, optional: false).Build();
        _emailWorkerConfiguration = new(config);

        config = new ConfigurationBuilder().AddJsonFile(messagesWorkerConfig, optional: false).Build();
        _messagesWorkerConfiguration = new(config);

        return ValueTask.CompletedTask;
    }

    [Fact]
    public void IdentityModule_ShouldHaveAccessOnlyToRelevantConnectionStrings()
    {
        // Assert irrelevant
        Assert.Null(_identityConfiguration.MenuConnectionString);
        Assert.Null(_identityConfiguration.OrdersConnectionString);
        Assert.Null(_identityConfiguration.ReservationsConnectionString);
        Assert.Null(_identityConfiguration.ShoppingCartConnectionString);
        Assert.Null(_identityConfiguration.PaymentsConnectionString);

        // Assert relevant
        Assert.NotNull(_identityConfiguration.IdentityConnectionString);
    }

    [Fact]
    public void ApiModule_ShouldHaveAccessOnlyToRelevantConnectionStrings()
    {
        // Assert irrelevant
        Assert.Null(_apiConfiguration.IdentityConnectionString);
        Assert.Null(_apiConfiguration.WorkerConnectionString);

        // Assert relevant
        Assert.NotNull(_apiConfiguration.MenuConnectionString);
        Assert.NotNull(_apiConfiguration.OrdersConnectionString);
        Assert.NotNull(_apiConfiguration.ReservationsConnectionString);
        Assert.NotNull(_apiConfiguration.ShoppingCartConnectionString);
        Assert.NotNull(_apiConfiguration.PaymentsConnectionString);
    }

    [Fact]
    public void Blazor_ShouldNotHaveAccessToConnectionStrings()
    {
        Assert.Null(_blazorConfiguration.IdentityConnectionString);
        Assert.Null(_blazorConfiguration.MenuConnectionString);
        Assert.Null(_blazorConfiguration.OrdersConnectionString);
        Assert.Null(_blazorConfiguration.ReservationsConnectionString);
        Assert.Null(_blazorConfiguration.ShoppingCartConnectionString);
        Assert.Null(_blazorConfiguration.PaymentsConnectionString);
        Assert.Null(_blazorConfiguration.WorkerConnectionString);
    }

    [Fact]
    public void AllModules_MustHaveTheSameConfigurationForNonDatabaseRelatedFields()
    {
        List<PropertyInfo>? properties = typeof(RestaurantConfigurationProvider)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p =>
                p.Name != "InfrastructurePrefix" &&
                !p.Name.Contains("ConnectionString"))
            .ToList();

        RestaurantConfigurationProvider[]? configs =
        [
            _appHostConfiguration,
            _identityConfiguration,
            _apiConfiguration,
            _blazorConfiguration,
            _emailWorkerConfiguration,
            _messagesWorkerConfiguration
        ];

        foreach (PropertyInfo property in properties)
        {
            List<object?>? values = configs.Select(c => property.GetValue(c)).Distinct().ToList();
            Assert.True(values.Count == 1, $"Property {property.Name} differs across modules.");
        }
    }

    [Fact]
    public void AllModules_ShouldConnectToTheSameServerAndDatabase()
    {
        string? referenceString = _identityConfiguration.IdentityConnectionString;
        (string? refServer, string? refDb, _) = ParseConnectionString(referenceString);

        Assert.NotNull(refServer);
        Assert.NotNull(refDb);

        IEnumerable<string> connectionsToCheck =
        [
            _apiConfiguration.MenuConnectionString ?? string.Empty,
            _apiConfiguration.OrdersConnectionString ?? string.Empty,
            _apiConfiguration.ShoppingCartConnectionString ?? string.Empty,
            _apiConfiguration.PaymentsConnectionString ?? string.Empty,
            _apiConfiguration.ReservationsConnectionString ?? string.Empty,
            _emailWorkerConfiguration.WorkerConnectionString ?? string.Empty,
            _messagesWorkerConfiguration.WorkerConnectionString ?? string.Empty
            ];

        foreach (string config in connectionsToCheck)
        {
            (string? server, string? db, _) = ParseConnectionString(config);

            Assert.NotNull(server);
            Assert.NotNull(db);
            Assert.Equal(refServer, server);
            Assert.Equal(refDb, db);
        }
    }

    [Fact]
    public void AllModules_ExceptWorkers_MustConnectToDatabaseWithDifferentUser()
    {
        (_, _, string? identityUser) = ParseConnectionString(_identityConfiguration.IdentityConnectionString);
        (_, _, string? apiUser) = ParseConnectionString(_apiConfiguration.MenuConnectionString);
        (_, _, string? orderUser) = ParseConnectionString(_apiConfiguration.OrdersConnectionString);
        (_, _, string? shoppingCartUser) = ParseConnectionString(_apiConfiguration.ShoppingCartConnectionString);
        (_, _, string? paymentUser) = ParseConnectionString(_apiConfiguration.PaymentsConnectionString);
        (_, _, string? reservationUser) = ParseConnectionString(_apiConfiguration.ReservationsConnectionString);

        List<string?> users = [identityUser, apiUser, orderUser, shoppingCartUser, paymentUser, reservationUser];
        int numberOfDistinctUsers = users.Distinct().Count();

        Assert.Equal(users.Count, numberOfDistinctUsers);
        Assert.DoesNotContain(users.AsEnumerable(), u => u is null);
    }

    [Fact]
    public void Workers_MustConnectToDatabaseAsTheSameUser()
    {
        Assert.NotNull(_emailWorkerConfiguration.WorkerConnectionString);
        Assert.NotNull(_messagesWorkerConfiguration.WorkerConnectionString);
        Assert.Equal(_emailWorkerConfiguration.WorkerConnectionString, _messagesWorkerConfiguration.WorkerConnectionString);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    private static (string? Server, string? Database, string? User) ParseConnectionString(string? connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            return (null, null, null);
        }

        DbConnectionStringBuilder? builder = new();
        try
        {
            builder.ConnectionString = connectionString;
        }
        catch
        {
            return (null, null, null);
        }

        string? server = builder["Server"] as string;
        string? database = builder["Database"] as string;
        string? user = builder["User Id"] as string;

        return (server, database, user);
    }

    private static void CheckConnectionStrings(RestaurantConfigurationProvider config, string expectedServer, string expectedDb)
    {
        IEnumerable<PropertyInfo>? properties = typeof(RestaurantConfigurationProvider)
            .GetProperties()
            .Where(p => p.Name.EndsWith("ConnectionString"));

        foreach (PropertyInfo prop in properties)
        {
            string? connString = prop.GetValue(config) as string;

            (string? server, string? db, _) = ParseConnectionString(connString);
            Assert.Equal(expectedServer, server);
            Assert.Equal(expectedDb, db);
        }
    }
}
