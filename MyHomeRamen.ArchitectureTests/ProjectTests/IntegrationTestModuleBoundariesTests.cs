using ArchUnitNET.Fluent;
using ArchUnitNET.xUnitV3;
using MyHomeRamen.ArchitectureTests.Common;

namespace MyHomeRamen.ArchitectureTests.ProjectTests;

public sealed class IntegrationTestModuleBoundariesTests(ArchitectureBuilder architectureBuilder)
    : BaseArchitectureTest(architectureBuilder)
{
    [Fact]
    public void MenuIntegrationTests_ShouldNotDependOn_RestaurantsModuleTypes()
    {
        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(
            ArchitectureBuilder.MenuApiIntegrationTestsAssembly.TypesInNamespace("MyHomeRamen.MenuApi.IntegrationTests"),
            GetModuleNamespaces("Restaurants"),
            "Menu integration tests namespace '{0}' should not depend on Restaurants namespace '{1}'");

        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void RestaurantsIntegrationTests_ShouldNotDependOn_MenuModuleTypes()
    {
        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(
            ArchitectureBuilder.RestaurantsApiIntegrationTestsAssembly.TypesInNamespace("MyHomeRamen.RestaurantsApi.IntegrationTests"),
            GetModuleNamespaces("Menu"),
            "Restaurants integration tests namespace '{0}' should not depend on Menu namespace '{1}'");

        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void ShoppingCartIntegrationTests_ShouldNotDependOn_MenuModuleInternals()
    {
        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(
            ArchitectureBuilder.ShoppingCartApiIntegrationTestsAssembly.TypesInNamespace("MyHomeRamen.ShoppingCartApi.IntegrationTests"),
            GetModuleNamespaces("Menu", includeFeatureApi: false),
            "Shopping Cart integration tests namespace '{0}' should not depend on Menu internals namespace '{1}'");

        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    [Fact]
    public void IdentityIntegrationTests_ShouldNotDependOn_OtherModuleTypes()
    {
        IEnumerable<string> forbiddenNamespaces = new[]
        {
            GetModuleNamespaces("Menu"),
            GetModuleNamespaces("Restaurants"),
            GetModuleNamespaces("ShoppingCart")
        }.SelectMany(namespaces => namespaces);
        IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(
            ArchitectureBuilder.IdentityApiIntegrationTestsAssembly.TypesInNamespace("MyHomeRamen.IdentityApi.IntegrationTests"),
            forbiddenNamespaces,
            "Identity integration tests namespace '{0}' should not depend on another module namespace '{1}'");

        foreach (IArchRule rule in rules)
        {
            rule.Check(ArchitectureBuilder.Architecture);
        }
    }

    private IEnumerable<string> GetModuleNamespaces(string module, bool includeFeatureApi = true)
    {
        IEnumerable<string> namespaces =
        [
            .. ArchitectureBuilder.DomainAssembly.TypesInNamespace($"MyHomeRamen.Domain.{module}"),
            .. ArchitectureBuilder.PersistanceAssembly.TypesInNamespace($"MyHomeRamen.Persistance.{module}")
        ];

        if (includeFeatureApi)
        {
            namespaces = namespaces.Concat(
                ArchitectureBuilder.ApiFeaturesAssembly.TypesInNamespace($"MyHomeRamen.Features.{module}"));
        }

        return namespaces;
    }
}
