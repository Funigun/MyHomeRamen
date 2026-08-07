using MyHomeRamen.Domain.Orders.Users;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Orders.Features.Users.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Orders;

public sealed partial class UserRepository(OrdersDbContext ordersDbContext, ICacheService cacheService)
    : BaseRepository<User, UserId>(ordersDbContext, cacheService), IUserRepository
{
    public IUserQuery Query() => this;

    public IUserSpecification Specification() => this;
}