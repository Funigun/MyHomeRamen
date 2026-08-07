using MyHomeRamen.Domain.Payments.Users;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.Payments.Features.Users.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.Payments;

public sealed partial class UserRepository(PaymentsDbContext paymentsDbContext, ICacheService cacheService)
    : BaseRepository<User, UserId>(paymentsDbContext, cacheService), IUserRepository
{
    public IUserQuery Query() => this;

    public IUserSpecification Specification() => this;
}