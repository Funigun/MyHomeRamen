using System.Diagnostics;

namespace MyHomeRamen.Features.Common.Cache;

public static class CacheDiagnostics
{
    public static ActivitySource ActivitySource { get; } = new("MyHomeRamen.Activity.Cache");
}
