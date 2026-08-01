using System.Runtime.CompilerServices;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Identity.Abstractions;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Orders.Features.Abstractions;
using MyHomeRamen.Features.Payments.Features.Abstractions;
using MyHomeRamen.Features.Reservations.Features.Abstractions;
using MyHomeRamen.Features.ShoppingCart.Features.Abstractions;
using MyHomeRamen.Worker.DatabaseInitializer.Config;
using Quartz;

namespace MyHomeRamen.Worker.DatabaseInitializer;

internal class DbInitializerJob(IIdentityDbContext userContext, IMenuDbContext menuDbContext, IShoppingCartDbContext shoppingCartDbContext,
                                IOrdersDbContext ordersDbContext, IReservationsDbContext reservationsDbContext,
                                IPaymentsDbContext paymentsDbContext, IConfiguration configuration,
                                ILogger<DbInitializerJob> logger)
             : IJob
{
    private static FormattableString CreateRawSql(string sql) => FormattableStringFactory.Create(sql);

    public async Task Execute(IJobExecutionContext context)
    {
        CancellationToken cancellationToken = context.CancellationToken;

        Dictionary<IUnitOfWork, DatabaseUserConfig> unitOfWorkContexts = new()
        {
            { userContext, DatabaseUserConfig.Create("Identity", configuration) },
            { menuDbContext, DatabaseUserConfig.Create("Menu", configuration) },
            { shoppingCartDbContext, DatabaseUserConfig.Create("ShoppingCart", configuration) },
            { paymentsDbContext, DatabaseUserConfig.Create("Payment", configuration) },
            { reservationsDbContext, DatabaseUserConfig.Create("Reservation", configuration) },
            { ordersDbContext, DatabaseUserConfig.Create("Order", configuration) }
        };

        bool dbExists = await userContext.EnsureCreated(cancellationToken);

        foreach (IUnitOfWork dbContext in unitOfWorkContexts.Keys)
        {
            DatabaseUserConfig userConfig = unitOfWorkContexts[dbContext];

            await dbContext.ExecuteSql(
                                       CreateRawSql(
                                       $@"IF (SCHEMA_ID('{userConfig.Schema}') IS NULL)
                                       BEGIN
                                            EXEC ('CREATE SCHEMA [{userConfig.Schema}]')
                                       END"),
                                       cancellationToken);

            await dbContext.Migrate(cancellationToken);
            await dbContext.Seed(cancellationToken);

            if (!dbExists)
            {
                await dbContext.ExecuteSql(CreateRawSql($"CREATE LOGIN {userConfig.User} with PASSWORD = '{userConfig.Password}';"), cancellationToken);
                await dbContext.ExecuteSql(CreateRawSql($"CREATE USER {userConfig.User} FOR LOGIN {userConfig.User};"), cancellationToken);

                await dbContext.ExecuteSql(CreateRawSql($"CREATE ROLE {userConfig.Role};"), cancellationToken);
                await dbContext.ExecuteSql(CreateRawSql($"ALTER ROLE {userConfig.Role} ADD MEMBER {userConfig.User};"), cancellationToken);

                await dbContext.ExecuteSql(CreateRawSql($"GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::[{userConfig.Schema}] TO {userConfig.Role};"), cancellationToken);

                await dbContext.ExecuteSql(CreateRawSql($"GRANT EXECUTE ON SCHEMA::[{userConfig.Schema}] TO {userConfig.Role};"), cancellationToken);

                await dbContext.ExecuteSql(CreateRawSql($"GRANT CONTROL ON SCHEMA::[{userConfig.Schema}] TO {userConfig.Role};"), cancellationToken);
                await dbContext.ExecuteSql(CreateRawSql($"ALTER AUTHORIZATION ON SCHEMA::[{userConfig.Schema}] TO {userConfig.Role};"), cancellationToken);
            }

            logger.LogInformation("{Comment}", $"Schema {userConfig.Schema} configured successfully.");
        }
    }
}
