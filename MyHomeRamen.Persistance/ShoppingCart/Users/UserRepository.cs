using MyHomeRamen.Domain.ShoppingCart.Users;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.ShoppingCart.Features.Users.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public sealed partial class UserRepository(ShoppingCartDbContext shoppingCartDbContext, ICacheService cacheService) : BaseRepository<User, UserId>(shoppingCartDbContext, cacheService), IUserRepository
{
    IUserQuery IUserRepository.Query() => this;

    IUserSpecification IUserRepository.Specification() => this;
}
