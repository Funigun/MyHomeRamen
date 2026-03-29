---
description: 'Instructions for architecture test projects'
applyTo: '**/MyHomeRamen.ArchitectureTests/**/*.cs'
---

# Architecture Tests Instructions

## 1) General Guidelines
- Use XUnit.v3 library as the testing framework.
- Use ArchUnitNET for structural / dependency assertions.
- Use AAA pattern: Arrange, Act, Assert.
- Each test method covers exactly one boundary rule and carries a descriptive name.
- Accepted test method naming conventions:
  - `{Subject}_ShouldNot_DependOn_{Target}` — cross-boundary dependency rule
  - `{Subject}_ShouldDepend_OnlyOn_{Target}` — positive isolation rule

## 2) Infrastructure

### 2.1) `ArchitectureBuilder` (assembly fixture)
`ArchitectureBuilder` is registered as an `[assembly: AssemblyFixture]` and is shared across **all** tests in the project. Inject it via the primary constructor — never create it manually.

It exposes:
- `Architecture` — the fully loaded `ArchUnitNET.Domain.Architecture` instance used for all `rule.Check(...)` calls.
- One `System.Reflection.Assembly` property per project (e.g., `ApiAssembly`, `DomainAssembly`, `PersistanceAssembly`, `BlazorServerAssembly`, etc.).
- `AllAssemblies` — array of all project assemblies, used for cross-project dependency rules.

```csharp
public sealed class MyTests(ArchitectureBuilder architectureBuilder) : BaseArchitectureTest(architectureBuilder)
```

### 2.2) `BaseArchitectureTest`
All test classes inherit from `BaseArchitectureTest`. It provides:

- **`ArchitectureBuilder ArchitectureBuilder { get; }`** — protected access to the fixture.
- **Layer providers** (`ApiLayer`, `DomainLayer`, `PersistanceLayer`, `BlazorServerLayer`, etc.) — pre-built `IObjectProvider<IType>` scoped to each project assembly for use in ad-hoc `ArchRuleDefinition` fluent rules.
- **`GetForbiddenDependenciesRules`** — static helper that generates one `IArchRule` per `(testedNamespace × forbiddenNamespace)` pair:

```csharp
protected static IEnumerable<IArchRule> GetForbiddenDependenciesRules(
    IEnumerable<string> testedTypes,
    IEnumerable<string> forbiddenTypes,
    string ruleDescription)   // must contain {0} (tested namespace) and {1} (forbidden namespace)
```

### 2.3) `TypeExtensions`
Extension methods on `System.Reflection.Assembly` for namespace-based lookups. Returns **distinct namespace strings** (not type names) for all types in the assembly whose `Namespace` starts with the given prefix.

```csharp
// Returns e.g. ["MyHomeRamen.Domain.Menu", "MyHomeRamen.Domain.Menu.Products", ...]
IEnumerable<string> namespaces = assembly.TypesInNamespace("MyHomeRamen.Domain.Menu");

// Same but for multiple root namespaces
IEnumerable<string> namespaces = assembly.TypesInNamespaces(new[] { "MyHomeRamen.Domain.Menu", "MyHomeRamen.Domain.Orders" });
```

> **Important:** Because `TypesInNamespace` uses `StartsWith`, the root namespace `"MyHomeRamen.Persistance"` also matches `"MyHomeRamen.Persistance.Menu"`, `"MyHomeRamen.Persistance.Orders"`, etc. Keep this in mind when building inclusion/exclusion filters.

## 3) Project-Level Tests (`ProjectTests/`)

### 3.1) Project dependency tests (`ProjectDependencyTests`)
Verify that each project assembly only references its explicitly allowed dependencies (no accidental project leakage).

**Pattern:**
- Define `IEnumerable<Assembly> allowedAssemblies` for the project under test.
- Call `PrepareProjectRules(projectAssembly, allowedAssemblies)` which derives the forbidden set from `architectureBuilder.AllAssemblies` and produces one `IArchRule` per forbidden assembly.
- Iterate and call `rule.Check(architectureBuilder.Architecture)`.

```csharp
[Fact]
public void Api_ShouldDepend_OnlyOnAllowedAssemblies()
{
    IEnumerable<Assembly> allowedAssemblies =
    [
        architectureBuilder.ApiCommonAssembly,
        architectureBuilder.DomainAssembly,
        architectureBuilder.PersistanceAssembly,
        // ...
    ];

    IEnumerable<IArchRule> rules = PrepareProjectRules(architectureBuilder.ApiAssembly, allowedAssemblies);

    foreach (IArchRule rule in rules)
    {
        rule.Check(architectureBuilder.Architecture);
    }
}
```

### 3.2) Contract sync tests (`ApiToBlazorContractSyncTests`)
Verify that Blazor Server models stay structurally in sync with the API models they mirror. Uses `TypeExtensions` helpers:
- `GetTypesByNameSuffix(string suffix)` — returns all public types whose name ends with the given suffix (e.g., `"Request"`, `"Response"`).
- `GetEnums()` — returns all public enum types.

Three checks are performed:
- `BlazorRequest_ShouldMatch_ApiRequestShape` — public property names and types match between Blazor and API `*Request` classes with the same name.
- `BlazorResponse_ShouldMatch_ApiResponseShape` — same for `*Response` classes.
- `BlazorEnum_ShouldMatch_DomainEnumValues` — Blazor enum members and values match the corresponding domain enum.

## 4) Module-Level Tests (`ModuleTests/{Module}/`)

Each module (Menu, Orders, Payments, Reservations, ShoppingCart, Users) has three boundary test classes that follow the same structural pattern.

### 4.1) `DomainBoundariesTests`
Enforce that a module's domain layer does not depend on any other module's domain.

One `[Fact]` per foreign module — no catch-all tests.

```csharp
[Fact]
public void MenuModule_ShouldNot_DependOn_OrdersModule()
{
    // Arrange
    IEnumerable<string> menuDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Menu");
    IEnumerable<string> ordersDomain = ArchitectureBuilder.DomainAssembly.TypesInNamespace("MyHomeRamen.Domain.Orders");

    IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(
        menuDomain, ordersDomain,
        "Menu type '{0}' should not depend on Orders type '{1}'");

    // Act & Assert
    foreach (IArchRule rule in rules)
    {
        rule.Check(ArchitectureBuilder.Architecture);
    }
}
```

**Required tests per module:** one test for each of the other five modules (5 tests total).

### 4.2) `PersistanceBoundariesTests`
Two groups of rules per module:

1. **Domain isolation** — persistence code may only reference its own module's domain namespace.
   - Filter with `.Where(name => !name.StartsWith("MyHomeRamen.Domain.{Module}", StringComparison.Ordinal))` to derive the forbidden set from `DomainAssembly`.

2. **Cross-persistence isolation** — one test per foreign module's persistence namespace (5 tests total).

```csharp
[Fact]
public void MenuPersistance_ShouldDepend_OnlyOn_MenuDomain()
{
    // Arrange
    IEnumerable<string> menuPersistence = ArchitectureBuilder.PersistanceAssembly
        .TypesInNamespace("MyHomeRamen.Persistance.Menu");
    IEnumerable<string> otherDomains = ArchitectureBuilder.DomainAssembly
        .TypesInNamespace("MyHomeRamen.Domain")
        .Where(name => !name.StartsWith("MyHomeRamen.Domain.Menu", StringComparison.Ordinal));

    IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(
        menuPersistence, otherDomains,
        "Menu persistence type '{0}' should not depend on domain type '{1}'");

    // Act & Assert
    foreach (IArchRule rule in rules)
    {
        rule.Check(ArchitectureBuilder.Architecture);
    }
}

[Fact]
public void MenuPersistance_ShouldNot_DependOn_OrdersPersistance()
{
    // Arrange
    IEnumerable<string> menuPersistence = ArchitectureBuilder.PersistanceAssembly
        .TypesInNamespace("MyHomeRamen.Persistance.Menu");
    IEnumerable<string> ordersPersistence = ArchitectureBuilder.PersistanceAssembly
        .TypesInNamespace("MyHomeRamen.Persistance.Orders");

    IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(
        menuPersistence, ordersPersistence,
        "Menu persistence type '{0}' should not depend on Orders persistence type '{1}'");

    // Act & Assert
    foreach (IArchRule rule in rules)
    {
        rule.Check(ArchitectureBuilder.Architecture);
    }
}
```

### 4.3) `ApiBoundariesTests`
Three groups of rules per module:

1. **Cross-API isolation** — one test per foreign module's API namespace (4 tests; Users module is excluded because it lives in a separate `Identity.Api` assembly).
2. **Domain isolation** — API code may only reference its own module's domain namespace.
3. **Persistence isolation** — API code may only reference its own module's persistence namespace plus the shared namespaces `MyHomeRamen.Persistance.Common` and the root `MyHomeRamen.Persistance`.

#### Cross-API test
```csharp
[Fact]
public void MenuApi_ShouldNot_DependOn_OrdersApi()
{
    // Arrange
    IEnumerable<string> menuApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Menu");
    IEnumerable<string> ordersApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Orders");

    IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(
        menuApi, ordersApi,
        "Menu API type '{0}' should not depend on Orders API type '{1}'");

    // Act & Assert
    foreach (IArchRule rule in rules)
    {
        rule.Check(ArchitectureBuilder.Architecture);
    }
}
```

#### Domain isolation test
```csharp
[Fact]
public void MenuApi_ShouldDepend_OnlyOn_MenuDomain()
{
    // Arrange
    IEnumerable<string> menuApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Menu");
    IEnumerable<string> otherDomains = ArchitectureBuilder.DomainAssembly
        .TypesInNamespace("MyHomeRamen.Domain")
        .Where(name => !name.StartsWith("MyHomeRamen.Domain.Menu", StringComparison.Ordinal));

    IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(
        menuApi, otherDomains,
        "Menu API type '{0}' should not depend on domain type '{1}'");

    // Act & Assert
    foreach (IArchRule rule in rules)
    {
        rule.Check(ArchitectureBuilder.Architecture);
    }
}
```

#### Persistence isolation test
The persistence filter must:
- Allow the module's own persistence namespace (e.g., `MyHomeRamen.Persistance.Menu`).
- Allow `MyHomeRamen.Persistance.Common` (shared `DbExtensions` helpers).
- Allow the root `MyHomeRamen.Persistance` namespace (shared `DependencyInjection` registration class) via an explicit exact-match exclusion — do **not** add it to the prefix list, as `StartsWith("MyHomeRamen.Persistance")` would inadvertently match every sub-namespace.

```csharp
[Fact]
public void MenuApi_ShouldDepend_OnlyOn_MenuPersistance()
{
    // Arrange
    IEnumerable<string> allowedPersistanceNamespaces =
    [
        "MyHomeRamen.Persistance.Menu",
        "MyHomeRamen.Persistance.Common",
    ];

    IEnumerable<string> menuApi = ArchitectureBuilder.ApiAssembly.TypesInNamespace("MyHomeRamen.Api.Menu");
    IEnumerable<string> forbiddenPersistence = ArchitectureBuilder.PersistanceAssembly
        .TypesInNamespace("MyHomeRamen.Persistance")
        .Where(name => name != "MyHomeRamen.Persistance"                              // allow root namespace (DependencyInjection)
                    && allowedPersistanceNamespaces.All(n => !name.StartsWith(n, StringComparison.Ordinal)));

    IEnumerable<IArchRule> rules = GetForbiddenDependenciesRules(
        menuApi, forbiddenPersistence,
        "Menu API type '{0}' should not depend on persistence type '{1}'");

    // Act & Assert
    foreach (IArchRule rule in rules)
    {
        rule.Check(ArchitectureBuilder.Architecture);
    }
}
```

> **Common pitfall:** Using `!n.StartsWith(n, ...)` instead of `!name.StartsWith(n, ...)` in the `All(...)` predicate makes the filter always return an empty set, causing the test to pass vacuously without checking anything.

## 5) Structure
```
├── MyHomeRamen.ArchitectureTests/
│   ├── Common/
│   │   ├── ArchitectureBuilder.cs      ← Assembly fixture; all assembly properties + Architecture
│   │   ├── BaseArchitectureTest.cs     ← Layer providers + GetForbiddenDependenciesRules helper
│   │   └── TypeExtensions.cs           ← TypesInNamespace / TypesInNamespaces / GetTypesByNameSuffix / GetEnums
│   ├── ProjectTests/
│   │   ├── ProjectDependencyTests.cs   ← Cross-project assembly dependency rules
│   │   └── ApiToBlazorContractSyncTests.cs ← Request/Response/Enum shape sync between API and Blazor
│   └── ModuleTests/
│       └── {Module}/
│           ├── DomainBoundariesTests.cs     ← 5 tests: module domain vs each other module domain
│           ├── PersistanceBoundariesTests.cs ← 6 tests: domain isolation + 5 cross-persistence
│           └── ApiBoundariesTests.cs        ← 6 tests: 4 cross-API + domain isolation + persistence isolation
```
