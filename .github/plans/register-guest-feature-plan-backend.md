# Plan: Register Guest — Backend

## Metadata

**Type:** Feature  
**Layers Affected:** Domain, Identity Api, Persistance, Common.Contracts  
**Created:** 2025-01-29

---

## References

- Existing POST endpoint pattern: `MyHomeRamen.Identity.Api/Features/Account/Register/RegisterEndpoint.cs`
- Existing handler pattern with messaging: `MyHomeRamen.Identity.Api/Features/Account/Register/RegisterHandler.cs`
- Existing mappings pattern: `MyHomeRamen.Identity.Api/Features/Account/Register/Models/Mappings.cs`
- Integration event contract: `MyHomeRamen.Common.Contracts/Messaging/UserRegisteredIntegrationEvent.cs`
- User aggregate: `MyHomeRamen.Domain/Users/User.cs`
- UsersDbContext configuration: `MyHomeRamen.Persistance/Users/UsersDbContext.cs`
- DB extensions pattern: `MyHomeRamen.Persistance/Users/Extensions/AddressDbExtensions.cs`
- Existing User errors/constants: `MyHomeRamen.Domain/Common/User/UserErrors.cs`, `UserConstants.cs`
- **Database migrations required:** Yes — `GuestId` column added to `Users` table, `KeycloakUserId` becomes nullable

---

## Implementation Plan

### Step 1: Domain Changes — `MyHomeRamen.Domain`

#### 1.1 Modify `User.cs`

File: `MyHomeRamen.Domain/Users/User.cs`

- Make `KeycloakUserId` nullable: `public string? KeycloakUserId { get; private set; }`
- Add new nullable property: `public Guid? GuestId { get; private set; }`
- Add a new static factory method `CreateGuest()` that:
  - Sets `Id = Guid.CreateVersion7()`
  - Sets `GuestId = Guid.CreateVersion7()`
  - Leaves `KeycloakUserId` as `null`
  - Calls `UserValidator.ValidateUser(user)` before returning
- Keep existing `Create(...)` method unchanged (it sets `KeycloakUserId`, leaves `GuestId` null)

```csharp
// New factory method
public static User CreateGuest()
{
    var user = new User
    {
        Id = Guid.CreateVersion7(),
        GuestId = Guid.CreateVersion7(),
    };

    UserValidator.ValidateUser(user);
    return user;
}
```

#### 1.2 Add `UserValidator.cs` (or update if it already exists)

File: `MyHomeRamen.Domain/Users/UserValidator.cs`

> **Note:** The domain `User` currently has no dedicated validator (unlike other aggregates). One needs to be created.

Create `internal static class UserValidator` with method `internal static void ValidateUser(User user)` that:
- Validates that exactly one of `KeycloakUserId` or `GuestId` is set — not both, not neither
- Throws `UserErrors.InvalidIdentity()` if the invariant is violated

#### 1.3 Update `UserErrors.cs`

File: `MyHomeRamen.Domain/Common/User/UserErrors.cs`

- Add: `public static DomainException InvalidIdentity() => new("A user must have either a KeycloakUserId or a GuestId, not both and not neither.");`

#### 1.4 Update existing `User.Create(...)` to call validator

File: `MyHomeRamen.Domain/Users/User.cs`

- Call `UserValidator.ValidateUser(user)` at the end of the existing `Create(...)` method before returning, consistent with domain validation standards.

---

### Step 2: Integration Event Contract — `MyHomeRamen.Common.Contracts`

#### 2.1 Create `GuestUserCreatedIntegrationEvent.cs`

File: `MyHomeRamen.Common.Contracts/Messaging/GuestUserCreatedIntegrationEvent.cs`

```csharp
namespace MyHomeRamen.Common.Contracts.Messaging;

public record GuestUserCreatedIntegrationEvent(Guid UserId, Guid GuestId);
```

---

### Step 3: Database Changes — `MyHomeRamen.Persistance`

#### 3.1 Update EF Core configuration for `User` in `UsersDbContext.cs`

File: `MyHomeRamen.Persistance/Users/UsersDbContext.cs`

- In `OnModelCreating` inside `builder.Entity<User>(b => { ... })`:
  - Configure `GuestId` as optional: `b.Property(u => u.GuestId).IsRequired(false);`
  - Configure `KeycloakUserId` as optional: `b.Property(u => u.KeycloakUserId).IsRequired(false);`
  - Add a unique index on `GuestId` (when not null): `b.HasIndex(u => u.GuestId).IsUnique().HasFilter("[GuestId] IS NOT NULL");`

#### 3.2 Add DB extension for idempotency check

File: `MyHomeRamen.Persistance/Users/Extensions/AddressDbExtensions.cs`  
(or create `UserDbExtensions.cs` in the same folder if the partial class is becoming too large — follow existing naming pattern)

Add extension method to `IQueryable<User>`:
```csharp
public async Task<Guid?> GetGuestIdByGuestIdAsync(Guid guestId, CancellationToken cancellationToken)
    => await users.AsNoTracking()
                  .Where(u => u.GuestId == guestId)
                  .Select(u => u.GuestId)
                  .FirstOrDefaultAsync(cancellationToken);
```

#### 3.3 Generate EF Core migration

- Migration name: `20250129_AddGuestIdToUser`
- Run:
  ```
  dotnet ef migrations add 20250129_AddGuestIdToUser --project MyHomeRamen.Persistance --startup-project MyHomeRamen.Identity.Api --context UsersDbContext
  ```
- The migration must:
  - Make column `KeycloakUserId` nullable (`ALTER COLUMN` with `nvarchar(max) NULL`)
  - Add column `GuestId` as `uniqueidentifier NULL`
  - Add a unique filtered index on `GuestId` where `GuestId IS NOT NULL`

---

### Step 4: Backend Implementation — `MyHomeRamen.Identity.Api`

Follow REPR pattern: `RegisterGuestEndpoint` → `RegisterGuestHandler` (no request body required).

#### 4.1 Create feature folder structure

```
MyHomeRamen.Identity.Api/
└── Features/
    └── Account/
        └── RegisterGuest/
            ├── Models/
            │   ├── RegisterGuestResponse.cs
            │   └── Mappings.cs
            └── RegisterGuestEndpoint.cs
            └── RegisterGuestHandler.cs
```

#### 4.2 Create `RegisterGuestResponse.cs`

File: `MyHomeRamen.Identity.Api/Features/Account/RegisterGuest/Models/RegisterGuestResponse.cs`

```csharp
namespace MyHomeRamen.Identity.Api.Features.Account.RegisterGuest.Models;

public record RegisterGuestResponse(Guid GuestId);
```

#### 4.3 Create `Mappings.cs`

File: `MyHomeRamen.Identity.Api/Features/Account/RegisterGuest/Models/Mappings.cs`

```csharp
// Extension on User to produce RegisterGuestResponse
extension(User user)
{
    internal RegisterGuestResponse ToRegisterGuestResponse()
    {
        return new RegisterGuestResponse(user.GuestId!.Value);
    }
}
```

#### 4.4 Create `RegisterGuestHandler.cs`

File: `MyHomeRamen.Identity.Api/Features/Account/RegisterGuest/RegisterGuestHandler.cs`

- Implements `IRequestHandler<RegisterGuestResponse>` (no input record needed — use a dedicated empty record or existing convention).
  > **Note:** Inspect `IRequestHandler` contract — if it requires a request type, define `RegisterGuestRequest` as an empty record. Follow the pattern used by `RegisterEndpoint` where the handler accepts `RegisterRequest`.
- Handler receives `IUsersDbContext` and `IMessagesService` via primary constructor.
- **Idempotency**: Handler receives an optional `Guid? existingGuestId` parameter extracted from the cookie (passed from the endpoint). If provided, queries the DB via `GetGuestIdByGuestIdAsync`; if found, returns the existing `GuestId` without inserting.
- If no existing guest: calls `User.CreateGuest()`, adds to `usersDbContext.Users`, saves, publishes `GuestUserCreatedIntegrationEvent`.

```csharp
// Pseudocode
public async Task<RegisterGuestResponse> Handle(RegisterGuestRequest request, CancellationToken cancellationToken)
{
    if (request.ExistingGuestId.HasValue)
    {
        Guid? existing = await dbContext.Users
                                        .GetGuestIdByGuestIdAsync(request.ExistingGuestId.Value, cancellationToken);
        if (existing.HasValue)
            return new RegisterGuestResponse(existing.Value);
    }

    User guest = User.CreateGuest();
    dbContext.Users.Add(guest);
    await dbContext.SaveChangesAsync(cancellationToken);

    await messagesService.PublishAsync(new GuestUserCreatedIntegrationEvent(guest.Id, guest.GuestId!.Value), cancellationToken);

    return new RegisterGuestResponse(guest.GuestId!.Value);
}
```

#### 4.5 Create `RegisterGuestEndpoint.cs`

File: `MyHomeRamen.Identity.Api/Features/Account/RegisterGuest/RegisterGuestEndpoint.cs`

- `GroupName = "Account"` — reuses the existing `AccountGroup`
- Maps `POST /guest`
- `.AllowAnonymous()` — no authentication required
- Reads `guest_id` cookie from `HttpContext.Request.Cookies`; passes it as `ExistingGuestId` to the handler
- On success: returns `201 Created` with `RegisterGuestResponse` body; sets `HttpOnly` `guest_id` cookie with 30-day expiry via `HttpContext.Response.Cookies.Append`

```csharp
// Cookie append example (server-side Blazor honoring HttpOnly):
httpContext.Response.Cookies.Append("guest_id", response.GuestId.ToString(), new CookieOptions
{
    HttpOnly = true,
    Expires = DateTimeOffset.UtcNow.AddDays(30),
    Path = "/",
    SameSite = SameSiteMode.Lax
});
```

> **Important:** The cookie must be set by the **Identity API** (not Blazor Server). Blazor Server calls this endpoint; the Identity API sets the cookie in its response; Blazor Server must forward the `Set-Cookie` header back to the browser. See Frontend plan for the forwarding mechanism.

---

### Step 5: Tests

#### 5.1 Unit Tests — `MyHomeRamen.UnitTests`

Folder: `MyHomeRamen.UnitTests/UsersModule/User/`

File: `UserValidationTests.cs`

Test cases:
- `CreateGuest_ShouldCreateUser_WithGuestIdSet_AndNullKeycloakUserId`
- `Create_ShouldCreateUser_WithKeycloakUserIdSet_AndNullGuestId`
- `CreateGuest_ShouldThrowDomainException_When_BothIdentitiesAreSet` (hypothetical — validates validator logic)
- `ValidateUser_ShouldThrowDomainException_When_NeitherIdentityIsSet`
- `ValidateUser_ShouldThrowDomainException_When_BothIdentitiesAreSet`
  - Assert exception message matches `UserErrors.InvalidIdentity().Message`

#### 5.2 Integration Tests — `MyHomeRamen.IdentityApi.IntegrationTests`

Folder: `MyHomeRamen.IdentityApi.IntegrationTests/IdentityModule/Account/`

File: `RegisterGuestTests.cs`

Test cases:
- `RegisterGuest_ShouldReturn201_WithGuestId_WhenNoCookiePresent`
  - POST `/api/account/guest` with no cookie
  - Assert `201 Created`
  - Assert response body contains a valid `GuestId` (non-empty Guid)
  - Assert response contains `Set-Cookie: guest_id=...` header with `HttpOnly`
- `RegisterGuest_ShouldReturn201_WithExistingGuestId_WhenValidGuestCookiePresent`
  - Seed a guest user in DB via `DataSeeder`
  - POST `/api/account/guest` with `Cookie: guest_id={seededGuestId}`
  - Assert `201 Created`
  - Assert response body `GuestId` equals the already-seeded `GuestId` (idempotency)
  - Assert no new `User` row was inserted
- `RegisterGuest_ShouldReturn201_AndCreateNew_WhenUnknownGuestCookiePresent`
  - POST `/api/account/guest` with a `guest_id` cookie that does not exist in DB
  - Assert `201 Created` with a new `GuestId`

> **DataSeeder additions:**  
> Add `SeedGuestUser()` helper in `DataSeeder.cs` that creates a `User` via `User.CreateGuest()` and saves it, exposing the `SeededGuestId` as a static property.

