using System.Reflection;
using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using MyHomeRamen.Api.Common.Endpoint.Models;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ProjectTests;

public sealed class ApiToBlazorContractSyncTests(ArchitectureBuilder architectureBuilder, ITestOutputHelper outputHelper) : BaseArchitectureTest(architectureBuilder), IAsyncLifetime
{
    private static System.Reflection.Assembly _blazorServerAssembly;
    private static System.Reflection.Assembly _domainAssembly;

    public async ValueTask InitializeAsync()
    {
        _blazorServerAssembly = architectureBuilder.BlazorServerAssembly;
        _domainAssembly = architectureBuilder.DomainAssembly;
    }

    public async ValueTask DisposeAsync()
    {

    }

    [Fact]
    public void BlazorEnum_ShouldMatch_DomainEnumValues()
    {
        Dictionary<string, Type> blazorEnums = _blazorServerAssembly.GetEnums();
        Dictionary<string, Type> domainEnums = _domainAssembly.GetEnums();

        static IEnumerable<(string Name, int Value)> GetMembers(Type type) =>
                System.Enum.GetValues(type)
                .Cast<object>()
                .Select(v => (Name: v.ToString()!, Value: (int)v))
                .OrderBy(m => m.Value);

        foreach (string enumName in blazorEnums.Keys)
        {
            outputHelper.WriteLine($"Comparing {enumName} enum values...");

            Type blazorEnum = blazorEnums[enumName];
            Type? domainEnum = domainEnums.TryGetValue(enumName, out Type? foundDomainEnum) ? foundDomainEnum : null;

            if (domainEnum is not null)
            {
                Assert.Equal(GetMembers(blazorEnum), GetMembers(domainEnum));
            }
        }
    }
}
