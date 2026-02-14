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
                                IPaymentsDbContext paymentsDbContext)
             : IJob
{
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

        await userContext.EnsureCreated(cancellationToken);

        foreach (IBaseDbContext dbContext in dbContexts.Keys)
        {
            DatabaseUserConfig userConfig = dbContexts[dbContext];

            await dbContext.ExecuteSql($"CREATE SCHEMA {userConfig.Schema}", cancellationToken);
            await dbContext.Migrate(cancellationToken);

            await dbContext.ExecuteSql($"CREATE ROLE {userConfig.Schema}", cancellationToken);
            await dbContext.ExecuteSql($"ALTER ROLE {userConfig.Role}", cancellationToken);
            await dbContext.ExecuteSql($"ADD MEMBER {userConfig.User}", cancellationToken);

            //await dbContext.ExecuteSql($"REVOKE SELECT, INSERT, UPDATE, DELETE, EXECUTE ON SCHEMA::{userConfig.Schema} FROM public", CancellationToken.None);

            await dbContext.ExecuteSql($"GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::{userConfig.Schema} TO {userConfig.Role}", cancellationToken);

            await dbContext.ExecuteSql($"GRANT EXECUTE ON SCHEMA::{userConfig.Schema} TO {userConfig.Role}", cancellationToken);

            await dbContext.ExecuteSql($"GRANT CONTROL ON SCHEMA::{userConfig.Schema} TO {userConfig.Role}", cancellationToken);
            await dbContext.ExecuteSql($"ALTER AUTHORIZATION ON SCHEMA::{userConfig.Schema} TO {userConfig.Role}", cancellationToken);
        }
    }
}
