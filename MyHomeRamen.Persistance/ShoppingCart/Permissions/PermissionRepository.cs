using MyHomeRamen.Domain.ShoppingCart.Permissions;
using MyHomeRamen.Features.Common.Cache;
using MyHomeRamen.Features.ShoppingCart.Features.Permissions.Common;
using MyHomeRamen.Persistance.Common;

namespace MyHomeRamen.Persistance.ShoppingCart;

public sealed partial class PermissionRepository(ShoppingCartDbContext shoppingCartDbContext, ICacheService cacheService) : BaseRepository<Permission, PermissionId>(shoppingCartDbContext, cacheService), IPermissionRepository
{
    IPermissionQuery IPermissionRepository.Query() => this;

    IPermissionSpecification IPermissionRepository.Specification() => this;
}
