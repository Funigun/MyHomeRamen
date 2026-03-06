using ArchUnitNET.Domain;
using ArchUnitNET.Loader;
using MyHomeRamen.ArchitectureTests.Common;

[assembly: AssemblyFixture(typeof(ArchitectureBuilder))]

namespace MyHomeRamen.ArchitectureTests.Common;

public sealed class ArchitectureBuilder
{
    public Architecture Architecture { get; private set; } = default!;

    public void Setup(IEnumerable<System.Reflection.Assembly> assemblies)
    {
        if (Architecture == default)
        {
            Architecture = new ArchLoader().LoadAssemblies(assemblies.ToArray()).Build();
        }
    }
}
