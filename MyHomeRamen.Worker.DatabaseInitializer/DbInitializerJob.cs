using System.Runtime.CompilerServices;
using MyHomeRamen.Domain.Abstractions;
using MyHomeRamen.Domain.Orders.Database;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Features.Common.Repository;
using MyHomeRamen.Features.Menu.Features.Abstractions;
using MyHomeRamen.Features.Payments.Features.Abstractions;
using MyHomeRamen.Features.Reservations.Features.Abstractions;
using MyHomeRamen.Worker.DatabaseInitializer.Config;
using Quartz;

namespace MyHomeRamen.Worker.DatabaseInitializer;

internal class DbInitializerJob(IUsersDbContext userContext, IMenuDbContext menuDbContext, IShoppingCartDbContext shoppingCartDbContext,
                                IOrdersDbContext ordersDbContext, IReservationsDbContext reservationsDbContext,
                                IPaymentsDbContext paymentsDbContext, IConfiguration configuration,
                                ILogger<DbInitializerJob> logger)
             : IJob
{
    private static FormattableString CreateRawSql(string sql) => FormattableStringFactory.Create(sql);

    public async Task Execute(IJobExecutionContext context)
    {
        CancellationToken cancellationToken = context.CancellationToken;

        Dictionary<IBaseDbContext, DatabaseUserConfig> dbContexts = new()
        {
            { userContext, DatabaseUserConfig.Create("Identity", configuration) },
            { shoppingCartDbContext, DatabaseUserConfig.Create("ShoppingCart", configuration) },
            { ordersDbContext, DatabaseUserConfig.Create("Order", configuration) }
        };

        foreach (IBaseDbContext dbContext in dbContexts.Keys)
        {
            bool dbExists = await dbContext.EnsureCreated(cancellationToken);

            DatabaseUserConfig userConfig = dbContexts[dbContext];

            await dbContext.ExecuteSql(
                                       CreateRawSql(
                                       $@"IF (SCHEMA_ID('{userConfig.Schema}') IS NULL)
                                       BEGIN
                                            EXEC ('CREATE SCHEMA [{userConfig.Schema}]')
                                       END"),
                                       cancellationToken);

            await dbContext.Migrate(cancellationToken);
            await dbContext.Seed(Guid.Empty, cancellationToken);

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

        Dictionary<IUnitOfWork, DatabaseUserConfig> unitOfWorkContexts = new()
        {
            { menuDbContext, DatabaseUserConfig.Create("Menu", configuration) },
            { paymentsDbContext, DatabaseUserConfig.Create("Payment", configuration) },
            { reservationsDbContext, DatabaseUserConfig.Create("Reservation", configuration) }
        };

        foreach (IUnitOfWork dbContext in unitOfWorkContexts.Keys)
        {
            bool dbExists = await dbContext.EnsureCreated(cancellationToken);

            DatabaseUserConfig userConfig = unitOfWorkContexts[dbContext];

            await dbContext.ExecuteSql(
                                       CreateRawSql(
                                       $@"IF (SCHEMA_ID('{userConfig.Schema}') IS NULL)
                                       BEGIN
                                            EXEC ('CREATE SCHEMA [{userConfig.Schema}]')
                                       END"),
                                       cancellationToken);

            await dbContext.Migrate(cancellationToken);
            await dbContext.Seed(Guid.Empty, cancellationToken);

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
