using MyHomeRamen.Features.Common.Cache;

namespace MyHomeRamen.Infrastructure.Cache;

internal sealed class IdentityCacheModule : ICacheModule
{
    public static string ModuleName => "User";
}

