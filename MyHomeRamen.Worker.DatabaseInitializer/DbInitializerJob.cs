using System.Runtime.CompilerServices;
using MyHomeRamen.Api.Common.Domain;
using MyHomeRamen.Domain.Menu.Database;
using MyHomeRamen.Domain.Orders.Database;
using MyHomeRamen.Domain.Payments.Database;
using MyHomeRamen.Domain.Reservations.Database;
using MyHomeRamen.Domain.ShoppingCart.Database;
using MyHomeRamen.Domain.Users.Database;
using MyHomeRamen.Worker.DatabaseInitializer.Config;
using Quartz;

namespace MyHomeRamen.Worker.DatabaseInitializer;

internal class DbInitializerJob(IUsersDbContext userContext, IMenuDbContext menuDbContext, IShoppingCartDbContext shoppingCartDbContext,
                                IOrdersDbContext ordersDbContext, IReservationsDbContext reservationsDbContext,
                                IPaymentsDbContext paymentsDbContext, ILogger<DbInitializerJob> logger)
             : IJob
{
    private static FormattableString CreateRawSql(string sql) => FormattableStringFactory.Create(sql);

    public async Task Execute(IJobExecutionContext context)
    {
        CancellationToken cancellationToken = context.CancellationToken;
        Dictionary<IBaseDbContext, DatabaseUserConfig> dbContexts = new()
        {
            { userContext, DatabaseUserConfig.CreateUserAdmin() },
            { menuDbContext, DatabaseUserConfig.CreateMenuAdmin() },
            { shoppingCartDbContext, DatabaseUserConfig.CreateShoppingCartAdmin() },
            { ordersDbContext, DatabaseUserConfig.CreateOrderAdmin() },
            { reservationsDbContext, DatabaseUserConfig.CreateReservationAdmin() },
            { paymentsDbContext, DatabaseUserConfig.CreatePaymentAdmin() }
        };

        bool newDatabaseCreated = await userContext.EnsureCreated(cancellationToken);
        string databaseCreationComment = newDatabaseCreated ? "Database created successfully." : "Database already exists.";
        logger.LogInformation("{Comment}", databaseCreationComment);

        foreach (IBaseDbContext dbContext in dbContexts.Keys)
        {
            DatabaseUserConfig userConfig = dbContexts[dbContext];

            await dbContext.ExecuteSql(
                                       CreateRawSql(
                                       $@"IF (SCHEMA_ID('{userConfig.Schema}') IS NULL)
                                       BEGIN
                                            EXEC ('CREATE SCHEMA [{userConfig.Schema}]')
                                       END"),
                                       cancellationToken);

            await dbContext.Migrate(cancellationToken);

            if (newDatabaseCreated)
            {
                await dbContext.ExecuteSql(CreateRawSql($"CREATE ROLE [{userConfig.Role}]"), cancellationToken);
                await dbContext.ExecuteSql(CreateRawSql($"ALTER ROLE [{userConfig.Role}] ADD MEMBER [{userConfig.User}]"), cancellationToken);

                //await dbContext.ExecuteSql($"REVOKE SELECT, INSERT, UPDATE, DELETE, EXECUTE ON SCHEMA::{userConfig.Schema} FROM public", CancellationToken.None);

                await dbContext.ExecuteSql(CreateRawSql($"GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::[{userConfig.Schema}] TO [{userConfig.Role}]"), cancellationToken);

                await dbContext.ExecuteSql(CreateRawSql($"GRANT EXECUTE ON SCHEMA::[{userConfig.Schema}] TO [{userConfig.Role}]"), cancellationToken);

                await dbContext.ExecuteSql(CreateRawSql($"GRANT CONTROL ON SCHEMA::[{userConfig.Schema}] TO [{userConfig.Role}]"), cancellationToken);
                await dbContext.ExecuteSql(CreateRawSql($"ALTER AUTHORIZATION ON SCHEMA::[{userConfig.Schema}] TO [{userConfig.Role}]"), cancellationToken);
            }

            logger.LogInformation("{Comment}", $"Schema {userConfig.Schema} configured successfully.");
        }
    }
}
