namespace MyHomeRamen.ArchitectureTests.Common;

internal static class TypeExtensions
{
    internal static IEnumerable<string> TypesInNamespace(this System.Reflection.Assembly assembly, string rootNamespace)
    {
        return assembly.GetTypes().Where(t => t.Namespace is not null && t.Namespace.StartsWith(rootNamespace, StringComparison.OrdinalIgnoreCase))
                                  .Select(t => t.Namespace!).Distinct().AsEnumerable();
    }

    internal static IEnumerable<string> TypesInNamespaces(this System.Reflection.Assembly assembly, IEnumerable<string> rootNamespaces)
    {
        return assembly.GetTypes().Where(t => t.Namespace is not null && rootNamespaces.Any(ns => t.Namespace.StartsWith(ns, StringComparison.OrdinalIgnoreCase)))
                                  .Select(t => t.Namespace!).Distinct().AsEnumerable();
    }
}
