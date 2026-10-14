using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MyHomeRamen.ArchitectureTests.Common;
using MyHomeRamen.Blazor.Infrastructure.Api;

namespace MyHomeRamen.ArchitectureTests.ConventionTests;

public sealed class BlazorSliceConventionsTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
{
    // Restaurants is the reference module; other modules are not migrated yet.
    private const string FeaturesNamespace = "MyHomeRamen.Blazor.Features.Restaurants";

    [Fact]
    public void RoutableComponents_ShouldHave_PageSuffix()
    {
        List<string> violations = GetFeatureTypes()
            .Where(IsRoutable)
            .Where(type => !type.Name.EndsWith("Page", StringComparison.Ordinal))
            .Select(type => type.FullName!)
            .ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public void PageComponents_ShouldNot_InjectApiDependencies()
    {
        List<string> violations = GetFeatureTypes()
            .Where(IsRoutable)
            .Where(type => GetInjectedTypes(type).Any(IsApiDependency))
            .Select(type => type.FullName!)
            .ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public void SharedComponents_ShouldNot_InjectApiDependencies()
    {
        List<string> violations = GetFeatureTypes()
            .Where(type => typeof(IComponent).IsAssignableFrom(type))
            .Where(type => type.Namespace is not null && type.Namespace.EndsWith(".Components", StringComparison.Ordinal))
            .Where(type => GetInjectedTypes(type).Any(IsApiDependency))
            .Select(type => type.FullName!)
            .ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public void Components_ShouldNot_InjectApiService()
    {
        List<string> violations = GetFeatureTypes()
            .Where(type => typeof(IComponent).IsAssignableFrom(type))
            .Where(type => GetInjectedTypes(type).Any(injected => injected == typeof(ApiService) || injected == typeof(HttpClient)))
            .Select(type => type.FullName!)
            .ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public void PageComponents_Should_DefineAuthorizationAndLayout()
    {
        List<string> violations = GetFeatureTypes()
            .Where(IsRoutable)
            .Where(type => (type.GetCustomAttribute<AuthorizeAttribute>() is null && type.GetCustomAttribute<AllowAnonymousAttribute>() is null)
                || type.GetCustomAttribute<LayoutAttribute>() is null)
            .Select(type => type.FullName!)
            .ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public void ApiClients_Should_ReturnApiResult()
    {
        List<string> violations = GetFeatureTypes()
            .Where(type => type.IsClass && type.Name.EndsWith("ApiClient", StringComparison.Ordinal))
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            .Where(method => !ReturnsApiResult(method.ReturnType))
            .Select(method => $"{method.DeclaringType!.FullName}.{method.Name}")
            .ToList();

        Assert.Empty(violations);
    }

    private IEnumerable<Type> GetFeatureTypes() =>
        ArchitectureBuilder.BlazorServerAssembly
            .GetTypes()
            .Where(type => type.Namespace is not null && type.Namespace.StartsWith(FeaturesNamespace, StringComparison.Ordinal));

    private static bool ReturnsApiResult(Type returnType) =>
        returnType.IsGenericType
        && returnType.GetGenericTypeDefinition() == typeof(Task<>)
        && returnType.GetGenericArguments()[0] is { IsGenericType: true } result
        && result.GetGenericTypeDefinition() == typeof(ApiResult<>);

    private static bool IsRoutable(Type type) =>
        typeof(IComponent).IsAssignableFrom(type) && type.GetCustomAttributes<RouteAttribute>().Any();

    private static IEnumerable<Type> GetInjectedTypes(Type type) =>
        type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(property => property.GetCustomAttribute<InjectAttribute>() is not null)
            .Select(property => property.PropertyType);

    private static bool IsApiDependency(Type type) =>
        type == typeof(ApiService) || type.Name.EndsWith("ApiClient", StringComparison.Ordinal);
}
