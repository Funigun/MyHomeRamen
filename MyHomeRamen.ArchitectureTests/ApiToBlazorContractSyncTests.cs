using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests;

public sealed class ApiToBlazorContractSyncTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{

    public static TheoryData<Type, Type> GetMatchingRequestTypes()
    {
        Dictionary<string, Type> blazorRequests = BlazorServerAssembly
            .GetTypes()
            .Where(t => t.IsPublic && t.Name.EndsWith("Request", StringComparison.Ordinal))
            .ToDictionary(t => t.Name);

        TheoryData<Type, Type> data = [];

        foreach (Type apiType in ApiAssembly.GetTypes().Where(t => t.IsPublic && t.Name.EndsWith("Request", StringComparison.Ordinal)))
        {
            if (blazorRequests.TryGetValue(apiType.Name, out Type? blazorType))
            {
                data.Add(blazorType, apiType);
            }
        }

        return data;
    }

#pragma warning disable xUnit1045
    [Theory]
    [MemberData(nameof(GetMatchingRequestTypes))]
#pragma warning restore xUnit1045
    public void BlazorRequest_ShouldMatch_ApiRequestShape(Type blazorType, Type apiType)
    {
        Architecture arch = Architecture;

        Class blazorClass = arch.GetClassOfType(blazorType);
        Class apiClass = arch.GetClassOfType(apiType);

        static IEnumerable<(string Name, string TypeName)> GetPublicProperties(Class c) =>
            c.Members
             .OfType<PropertyMember>()
             .Where(p => p.Visibility == Visibility.Public)
             .Select(p => (p.Name, p.Type.FullName))
             .OrderBy(p => p.Name);

        Assert.Equal(
            GetPublicProperties(blazorClass),
            GetPublicProperties(apiClass));
    }

    public static TheoryData<Type, Type> GetMatchingResponseTypes()
    {
        Dictionary<string, Type> blazorRequests = BlazorServerAssembly
            .GetTypes()
            .Where(t => t.IsPublic && t.Name.EndsWith("Response", StringComparison.Ordinal))
            .ToDictionary(t => t.Name);

        TheoryData<Type, Type> data = [];

        foreach (Type apiType in ApiAssembly.GetTypes().Where(t => t.IsPublic && t.Name.EndsWith("Response", StringComparison.Ordinal)))
        {
            if (blazorRequests.TryGetValue(apiType.Name, out Type? blazorType))
            {
                data.Add(blazorType, apiType);
            }
        }

        return data;
    }

#pragma warning disable xUnit1045
    [Theory]
    [MemberData(nameof(GetMatchingRequestTypes))]
#pragma warning restore xUnit1045
    public void BlazorResponse_ShouldMatch_ApiResponseShape(Type blazorType, Type apiType)
    {
        Architecture arch = Architecture;

        Class blazorClass = arch.GetClassOfType(blazorType);
        Class apiClass = arch.GetClassOfType(apiType);

        static IEnumerable<(string Name, string TypeName)> GetPublicProperties(Class c) =>
            c.Members
             .OfType<PropertyMember>()
             .Where(p => p.Visibility == Visibility.Public)
             .Select(p => (p.Name, p.Type.FullName))
             .OrderBy(p => p.Name);

        Assert.Equal(
            GetPublicProperties(blazorClass),
            GetPublicProperties(apiClass));
    }

    public static TheoryData<Type, Type> GetMatchingEnumTypes()
    {
        Dictionary<string, Type> blazorEnums = BlazorServerAssembly
            .GetTypes()
            .Where(t => t.IsPublic && t.IsEnum)
            .ToDictionary(t => t.Name);

        TheoryData<Type, Type> data = [];

        foreach (Type domainEnum in DomainAssembly.GetTypes().Where(t => t.IsPublic && t.IsEnum))
        {
            if (blazorEnums.TryGetValue(domainEnum.Name, out Type? blazorEnum))
            {
                data.Add(blazorEnum, domainEnum);
            }
        }

        return data;
    }

#pragma warning disable xUnit1045
    [Theory]
    [MemberData(nameof(GetMatchingEnumTypes))]
#pragma warning restore xUnit1045
    public void BlazorEnum_ShouldMatch_DomainEnumValues(Type blazorEnum, Type domainEnum)
    {
        static IEnumerable<(string Name, int Value)> GetMembers(Type type) =>
            System.Enum.GetValues(type)
                .Cast<object>()
                .Select(v => (Name: v.ToString()!, Value: (int)v))
                .OrderBy(m => m.Value);

        Assert.Equal(
            GetMembers(blazorEnum),
            GetMembers(domainEnum));
    }
}
