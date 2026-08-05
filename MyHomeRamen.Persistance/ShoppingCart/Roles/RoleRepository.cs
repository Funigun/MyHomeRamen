using MyHomeRamen.Domain.ShoppingCart.Roles;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.ShoppingCart.Features.Roles.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public sealed partial class RoleRepository(ShoppingCartDbContext shoppingCartDbContext, ICacheService cacheService) : BaseRepository<Role, RoleId>(shoppingCartDbContext, cacheService), IRoleRepository
{
    IRoleQuery IRoleRepository.Query() => this;

    IRoleSpecification IRoleRepository.Specification() => this;
}
