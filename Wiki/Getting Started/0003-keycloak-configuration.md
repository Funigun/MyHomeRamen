# Keycloak Configuration

As described in [ADR-0002: Users Authentication](../Users%20ADR/0002-users-authentication.md), **My Home Ramen** uses **Keycloak** as the central authentication server. 
While Keycloak handles identity management and authentication flows (login, registration), authorization logic is largely handled within our own services (see `0005-keycloak-integration.md` for implementation details).

## Integration Overview

The Keycloak integration follows the same "infrastructure prefix" convention used throughout the Aspire orchestration.
Resources and configuration settings are named consistently (e.g., `my-home-ramen-realm`, `my-home-ramen-client`) to ensure clear mapping between the application and the identity provider.

## Realm Configuration

The project includes a pre-configured realm export file located at:  
`MyHomeRamen.AppHost/Configurations/Keycloak/keycloak-config.json`

This file is automatically imported by the Keycloak container on startup (via `RegistrationExtensions.cs`), ensuring that the local development environment is bootstrapped with:
- **Realm**: `my-home-ramen-realm`
- **Clients**: e.g., `my-home-ramen-client` (Frontend), `my-home-ramen-admin-client` (Admin API)
- **Roles & Scopes**: Pre-defined roles and scopes required by the application.

## AppHost Configuration

To connect to the Keycloak instance, the `MyHomeRamen.AppHost` project requires specific settings in `appsettings.Development.json` under the `Authorization` section.

```json
"Authorization": {
  "BaseUrl": "http://localhost:8080",
  "Realm": "my-home-ramen-realm",
  "Audience": "my-home-ramen-client"
}
```

- **BaseUrl**: The URL to the Keycloak instance. By default, the Aspire orchestration configures Keycloak to run on port `8080`.
- **Realm**: The name of the realm to use. This **must** match the `"realm"` property defined in `keycloak-config.json`.
- **Audience**: The expected audience claim for tokens, typically referencing the main frontend client.

## User Secrets (Credentials)

Sensitive information, such as client secrets, should not be stored in `appsettings.json`. Instead, use **User Secrets** for the AppHost project.

You need to configure the Client/Secret pairs for both the Blazor frontend (to perform authentication) and the Keycloak Admin client (used by the Identity API to manage users).

Run the following commands in the `MyHomeRamen.AppHost` directory (or use "Manage User Secrets" in Visual Studio):

```powershell
# Blazor Client Configuration
dotnet user-secrets set "Authentication:Blazor:ClientId" "my-home-ramen-client"
dotnet user-secrets set "Authentication:Blazor:ClientSecret" "[YOUR_BLAZOR_CLIENT_SECRET]"

# Keycloak Admin Client Configuration
dotnet user-secrets set "Authentication:KeycloakAdmin:ClientId" "my-home-ramen-admin-client"
dotnet user-secrets set "Authentication:KeycloakAdmin:ClientSecret" "[YOUR_ADMIN_CLIENT_SECRET]"
```

> **Note**: Since the `keycloak-config.json` file often masks secrets (e.g., `**********`), you may need to retrieve the actual generated secrets from the Keycloak Admin Console (`http://localhost:8080`) after the initial container startup, or coordinate with the team to get the development secrets.

## Aspire Orchestration

Once configured, the **Aspire AppHost** reads these values and injects them as environment variables or configuration settings into the dependent services:

- **Blazor Frontend**: Receives `ClientId` and `ClientSecret` to configure OIDC authentication.
- **Identity API**: Receives Admin credentials to perform user management tasks.
- **API Service**: Receives Authority (BaseUrl + Realm) configuration to validate incoming tokens.
