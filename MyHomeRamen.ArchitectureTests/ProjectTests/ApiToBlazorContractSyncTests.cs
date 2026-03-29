using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ProjectTests;

public sealed class ApiToBlazorContractSyncTests(ArchitectureBuilder architectureBuilder, ITestOutputHelper outputHelper) : BaseArchitectureTest(architectureBuilder), IAsyncLifetime
{
    private static System.Reflection.Assembly _apiAssembly;
    private static System.Reflection.Assembly _blazorServerAssembly;
    private static System.Reflection.Assembly _domainAssembly;

    public async ValueTask InitializeAsync()
    {
        _apiAssembly = architectureBuilder.ApiAssembly;
        _blazorServerAssembly = architectureBuilder.BlazorServerAssembly;
        _domainAssembly = architectureBuilder.DomainAssembly;
    }

    public async ValueTask DisposeAsync()
    {

    }

    [Fact]
    public void BlazorRequest_ShouldMatch_ApiRequestShape()
    {
        Architecture arch = architectureBuilder.Architecture;

        Dictionary<string, Type> blazorRequests = _blazorServerAssembly.GetTypesByNameSuffix("Request");
        Dictionary<string, Type> apiRequests = _apiAssembly.GetTypesByNameSuffix("Request");

        foreach (string requestName in blazorRequests.Keys.Intersect(apiRequests.Keys))
        {
            outputHelper.WriteLine($"Comparing {requestName} shapes...");

            Class blazorClass = arch.GetClassOfType(blazorRequests[requestName]);
            Class apiClass = arch.GetClassOfType(apiRequests[requestName]);

            static IEnumerable<(string Name, string TypeName)> GetPublicProperties(Class c) =>
                c.Members
                 .OfType<PropertyMember>()
                 .Where(p => p.Visibility == Visibility.Public)
                 .Select(p => (p.Name, p.Type.FullName))
                 .OrderBy(p => p.Name);

            Assert.Equal(GetPublicProperties(blazorClass), GetPublicProperties(apiClass));
        }
    }

    [Fact]
    public void BlazorResponse_ShouldMatch_ApiResponseShape()
    {
        Architecture arch = architectureBuilder.Architecture;

        Dictionary<string, Type> blazorRequests = _blazorServerAssembly.GetTypesByNameSuffix(nameSuffix: "Response");
        Dictionary<string, Type> apiRequests = _apiAssembly.GetTypesByNameSuffix("Response");

        foreach (string responseName in blazorRequests.Keys.Intersect(apiRequests.Keys))
        {
            outputHelper.WriteLine($"Comparing {responseName} shapes...");

            Class blazorClass = arch.GetClassOfType(blazorRequests[responseName]);
            Class apiClass = arch.GetClassOfType(apiRequests[responseName]);

            static IEnumerable<(string Name, string TypeName)> GetPublicProperties(Class c) =>
                c.Members
                 .OfType<PropertyMember>()
                 .Where(p => p.Visibility == Visibility.Public)
                 .Select(p => (p.Name, p.Type.FullName))
                 .OrderBy(p => p.Name);

            Assert.Equal(GetPublicProperties(blazorClass), GetPublicProperties(apiClass));
        }
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
