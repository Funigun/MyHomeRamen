# 5. Permission-Based Authorization — Analysis

## Current State

| Component | Location | Status |
|---|---|---|
| `IAuthorizationPolicy<TRequest>` | `MyHomeRamen.Api.Common` | ✅ Defined |
| `AuthorizationFilter<TRequest>` | `MyHomeRamen.Api.Common` | ✅ Implemented (endpoint filter) |
| `ICurrentUser` / `CurrentUser` | `MyHomeRamen.Api.Common` | ✅ Implemented (Id, RestaurantId, Claims) |
| `PermissionConstants` (per module) | `MyHomeRamen.Domain.{Module}.Users` | ✅ Defined for Menu, Orders, Payments, Reservations, ShoppingCart |
| `RoleConstants` (per module) | `MyHomeRamen.Domain.{Module}.Users` | ✅ Defined with `DefaultPermissions` role→permission mappings |
| `KeycloakRolesClaimsTransformation` | `MyHomeRamen.Api` / `MyHomeRamen.Identity.Api` | ✅ Extracts `realm_access` and `resource_access` roles into `ClaimTypes.Role` |
| Endpoint-level auth | `.RequireAuthorization(policy)` | ✅ Role-based (RestaurantManager, RestaurantEmployee, RestaurantCustomer) |
| Feature-level `IAuthorizationPolicy` usage | `GetEmployeesAuthorizationPolicy` | ⚠️ Single stub (returns `true`) |
| Permission checks in handlers / HATEOAS | — | ❌ Not yet implemented |

### How Authorization Works Today

1. **JWT → Claims:** `KeycloakRolesClaimsTransformation` flattens Keycloak roles from `realm_access` and `resource_access` into standard `ClaimTypes.Role` claims.
2. **Endpoint-level:** Each endpoint calls `.RequireAuthorization("RestaurantManager")` (or similar). This is a coarse role gate — if you have the role, you reach the handler.
3. **Filter-level (opt-in):** Endpoints using `WithAuthenticationFilter<TRequest>()` resolve `IAuthorizationPolicy<TRequest>` from DI and run it before the handler. Currently only one stub exists.
4. **No fine-grained permission checks:** The `PermissionConstants` and `RoleConstants.DefaultPermissions` mappings are defined in Domain but never consumed at runtime by the API.

### Problem Statement

The current system only checks **roles** at the endpoint level. The project has already defined **fine-grained permissions** per module (e.g. `CanEditIngredient`, `CanAddProduct`) and **role→permission mappings** (e.g. Chef can edit products but not delete them). These need to be enforced at runtime in a way that:

1. Works with `IAuthorizationPolicy<TRequest>` in the endpoint filter pipeline.
2. Can be reused by HATEOAS link builders to decide which action links to include (future — see `06-hateoas-permission-integration.md`).
3. Avoids duplicating permission-check logic between the handler pipeline and HATEOAS response building.
4. Stays consistent across modules (Menu, Orders, Reservations, etc.).

---

## Three Approaches

### Approach A — Shared Permission Service (per module)

Introduce a scoped service per module that encapsulates "does the current user have permission X?". Both `IAuthorizationPolicy` implementations and future HATEOAS builders consume it.

**New interface in `MyHomeRamen.Api.Common`:**
```csharp
public interface IPermissionService
{
    bool HasPermission(string permission);
    IEnumerable<string> GetPermissions();
}
```

**Implementation per module (e.g. `MenuPermissionService` in `MyHomeRamen.Api`):**
```csharp
public sealed class MenuPermissionService(ICurrentUser currentUser) : IPermissionService
{
    private readonly Lazy<HashSet<string>> _permissions = new(() =>
    {
        HashSet<string> result = [];
        foreach (string role in currentUser.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value))
        {
            if (RoleConstants.DefaultPermissions.TryGetValue(role, out var perms))
                result.UnionWith(perms);
        }
        return result;
    });

    public bool HasPermission(string permission) => _permissions.Value.Contains(permission);

    public IEnumerable<string> GetPermissions() => _permissions.Value;
}
```

**Authorization policy usage:**
```csharp
public sealed class UpdateIngredientAuthorizationPolicy(IPermissionService permissions)
    : IAuthorizationPolicy<UpdateIngredientRequest>
{
    public Task<bool> IsAuthorized(UpdateIngredientRequest request)
        => Task.FromResult(permissions.HasPermission(PermissionConstants.CanEditIngredient));
}
```

**Registration:**
```csharp
// In Menu module DI setup
services.AddScoped<IPermissionService, MenuPermissionService>();
```

**Pros:**
- Single source of truth for permission resolution — both AuthPolicies and future HATEOAS builders call `IPermissionService.HasPermission(...)`.
- Lazy-evaluated once per request scope; subsequent calls are O(1) HashSet lookups.
- Module-specific implementations allow different modules to resolve permissions differently (e.g. Keycloak roles for Menu vs. DB-stored permissions for a future module).
- Auth policies remain thin wrappers — easy to test.
- Clean separation: `IPermissionService` does not know about endpoints, requests, or entities.

**Cons:**
- One `IPermissionService` per DI scope means a single module's permissions. If a single request crosses modules (unlikely in modular monolith), would need per-module keyed services.
- If permissions are ever stored in DB or fetched from Keycloak admin API at runtime, the service becomes async, requiring interface change.

**Multi-module consideration:**
Since the API is a modular monolith where each request stays within a single module, a single `IPermissionService` per scope works. If cross-module permission checks are ever needed (e.g. the aggregated IdentityApi endpoint), use keyed DI:
```csharp
services.AddKeyedScoped<IPermissionService, MenuPermissionService>("Menu");
services.AddKeyedScoped<IPermissionService, OrdersPermissionService>("Orders");
```

---

### Approach B — Enrich `CurrentUser` with Permissions

Extend `ICurrentUser` and `CurrentUser` to resolve permissions at construction time and expose `HasPermission(...)` directly.

**Updated `ICurrentUser`:**
```csharp
public interface ICurrentUser
{
    string Id { get; init; }
    Guid RestaurantId { get; init; }
    IEnumerable<Claim> Claims { get; init; }
    bool HasPermission(string permission);
    IEnumerable<string> Permissions { get; }
}
```

**Updated `CurrentUser`:**
```csharp
public sealed class CurrentUser(
    IHttpContextAccessor httpContextAccessor,
    RestaurantConfigurationProvider configurationProvider,
    IPermissionResolver permissionResolver) : ICurrentUser
{
    public string Id { get; init; } = /* same as today */;
    public Guid RestaurantId { get; init; } = /* same as today */;
    public IEnumerable<Claim> Claims { get; init; } = /* same as today */;

    // Resolved once at construction
    public IEnumerable<string> Permissions { get; init; } =
        permissionResolver.Resolve(httpContextAccessor.HttpContext?.User?.Claims ?? []);

    public bool HasPermission(string permission) =>
        Permissions.Contains(permission);
}
```

Where `IPermissionResolver` is a module-specific strategy that maps claims to permissions.

**Authorization policy usage:**
```csharp
public sealed class UpdateIngredientAuthorizationPolicy(ICurrentUser currentUser)
    : IAuthorizationPolicy<UpdateIngredientRequest>
{
    public Task<bool> IsAuthorized(UpdateIngredientRequest request)
        => Task.FromResult(currentUser.HasPermission(PermissionConstants.CanEditIngredient));
}
```

**Pros:**
- Extremely convenient — every service that already injects `ICurrentUser` gets permission checks for free.
- Single resolution point at scope creation.
- Natural API: `currentUser.HasPermission("CanEditIngredient")`.

**Cons:**
- **Stale permissions risk:** `CurrentUser` is scoped and constructed once per request. If permissions were to change mid-request (unlikely but worth noting for HATEOAS: the handler modifies state that affects what the response links should show), the permissions snapshot is stale. In practice, this is not a real problem since roles don't change within a single HTTP request.
- **Module coupling:** `CurrentUser` lives in `Api.Common` but needs module-specific permission resolution. Requires an abstraction (`IPermissionResolver`) to avoid `Api.Common` referencing Domain module namespaces.
- **Interface growth:** `ICurrentUser` becomes a broader abstraction. Every module's test that mocks `ICurrentUser` must now also deal with `HasPermission` / `Permissions`.

---

### Approach C — Authorization Policies Only (no shared service)

Each `IAuthorizationPolicy<TRequest>` implementation directly reads `ICurrentUser.Claims` and checks permissions inline. No shared permission service.

**Authorization policy:**
```csharp
public sealed class UpdateIngredientAuthorizationPolicy(ICurrentUser currentUser)
    : IAuthorizationPolicy<UpdateIngredientRequest>
{
    public Task<bool> IsAuthorized(UpdateIngredientRequest request)
    {
        var roles = currentUser.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value);

        var permissions = roles
            .Where(RoleConstants.DefaultPermissions.ContainsKey)
            .SelectMany(r => RoleConstants.DefaultPermissions[r])
            .ToHashSet();

        return Task.FromResult(permissions.Contains(PermissionConstants.CanEditIngredient));
    }
}
```

**Pros:**
- No new abstractions. Each policy is self-contained.
- Easy to understand in isolation.
- Possible to add entity-existence checks per policy (e.g. verify the ingredient ID exists before returning `Forbidden` vs `NotFound`).

**Cons:**
- **Duplicated logic:** Every policy repeats the claims→roles→permissions resolution. This will be 30+ policies across all modules.
- **HATEOAS problem:** When HATEOAS link builders need the same permission check, they cannot reuse the policy (it's typed to a specific `TRequest`). Permission resolution logic must be duplicated again.
- **Testing:** Each policy test must set up full claims infrastructure.

---

## Comparison

| Criterion | A — PermissionService | B — CurrentUser | C — Policies Only |
|---|---|---|---|
| Single source of truth | ✅ `IPermissionService` | ✅ `ICurrentUser` | ❌ Duplicated per policy |
| HATEOAS reusability | ✅ Inject same service | ✅ Already available | ❌ Must duplicate logic |
| Interface growth on `ICurrentUser` | None | ⚠️ Grows | None |
| Module isolation | ✅ Per-module impl | ⚠️ Needs `IPermissionResolver` | ✅ Self-contained |
| Test simplicity | ✅ Mock `IPermissionService` | ⚠️ Mock grows | ⚠️ Full claims setup |
| Async-ready (DB/Keycloak permissions) | ✅ Easy to make async | ⚠️ Constructor resolution | ❌ Inline in each policy |
| Entity-existence checks in auth | ✅ Policy still has DB access | ✅ Same | ✅ Same |
| Implementation effort | Medium | Medium | Low per policy, high total |

---

## Entity-Existence Checks in Authorization Policies

One consideration raised: should auth policies check whether the target entity exists (e.g. "does ingredient with this ID exist?") so that requests with invalid IDs get `403 Forbidden` instead of `404 Not Found`?

**Arguments for:**
- Prevents ID enumeration attacks (attacker cannot distinguish "exists but forbidden" from "doesn't exist").

**Arguments against:**
- Adds DB queries to the auth layer, which should ideally be fast and stateless.
- `404 Not Found` is the semantically correct response and what API consumers expect.
- The handler already validates existence and returns a proper error.
- Mixing authorization with resource validation blurs responsibilities.

**Recommendation:** Do **not** add entity-existence checks in auth policies. Keep auth policies purely permission-based. Let validation policies handle existence checks. If ID enumeration is a security concern for specific endpoints, add it as a separate, explicit security filter — not as a default behavior.

---

## Recommendation

**Use Approach A (Shared Permission Service)** as the primary pattern, with the following design:

1. **`IPermissionService`** in `MyHomeRamen.Api.Common.Authorization` — interface with `HasPermission(string)` and `GetPermissions()`.
2. **Module-specific implementations** (e.g. `MenuPermissionService`) in each API module's `Services/` folder — resolves permissions from `ICurrentUser.Claims` using the module's `RoleConstants.DefaultPermissions`.
3. **`IAuthorizationPolicy<TRequest>` implementations** inject `IPermissionService` and delegate to `HasPermission(...)`. They remain thin, testable, and focused on mapping "which permission does this request require?".
4. **Future HATEOAS link builders** will also inject `IPermissionService` to decide which links to include (see `06-hateoas-permission-integration.md`).

### Why not B (CurrentUser)?

While convenient, enriching `CurrentUser` with permissions creates a larger interface that every consumer must deal with. The `IPermissionResolver` abstraction needed to keep `Api.Common` module-agnostic is essentially `IPermissionService` with extra steps. Keeping `CurrentUser` focused on identity (who) and `IPermissionService` focused on authorization (what can they do) is a cleaner separation.

### Why not C (Policies Only)?

The duplication across 30+ policies and the inability to share logic with HATEOAS builders makes this unsustainable as the project grows.

---

## Implementation Sketch

### File structure

```
MyHomeRamen.Api.Common/
└── Authorization/
    ├── IAuthorizationPolicy.cs          ← existing
    ├── ICurrentUser.cs                  ← existing (no changes)
    ├── CurrentUser.cs                   ← existing (no changes)
    └── IPermissionService.cs            ← NEW

MyHomeRamen.Api/
└── Menu/
    ├── Services/
    │   └── MenuPermissionService.cs     ← NEW
    └── Features/
        └── Ingredients/
            └── UpdateIngredient/
                └── Policies/
                    ├── UpdateIngredientValidationPolicy.cs     ← existing
                    └── UpdateIngredientAuthorizationPolicy.cs  ← NEW
```

### Registration

```csharp
// In Menu module DI registration
services.AddScoped<IPermissionService, MenuPermissionService>();
services.AddAuthorizationPolicies(typeof(MenuModule).Assembly); // existing scan
```

### Pipeline flow

```
HTTP Request
  → JWT Authentication (Keycloak)
  → RequireAuthorization("RestaurantManager")          ← coarse role gate (existing)
  → AuthorizationFilter<TRequest>                      ← fine permission check (NEW)
      → IAuthorizationPolicy<TRequest>.IsAuthorized()
          → IPermissionService.HasPermission(...)
  → ValidationFilter<TRequest>                         ← input validation (existing)
  → Handler                                            ← business logic (existing)
  → (Future) HATEOAS link building
      → IPermissionService.HasPermission(...)          ← same service, same scope
```

### Migration strategy

1. Implement `IPermissionService` and `MenuPermissionService` first.
2. Add `UpdateIngredientAuthorizationPolicy` as a proof-of-concept.
3. Wire the endpoint with `.WithAuthenticationFilter<TRequest>()`.
4. Gradually add auth policies to other features.
5. Coarse `.RequireAuthorization(...)` can remain as a first-pass gate. The fine-grained `IAuthorizationPolicy` adds the permission-level check on top.
