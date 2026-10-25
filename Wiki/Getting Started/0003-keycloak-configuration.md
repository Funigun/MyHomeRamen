# Keycloak configuration

My Home Ramen uses Keycloak for sign-in, token issuance, and backend user
provisioning. Application permissions are stored and checked by the backend;
see the [authorization guide](../Authorization/README.md).

## Tracked realm files

Aspire configures Keycloak in
`MyHomeRamen.AppHost\InfrastructureConfiguration\RegistrationExtensions.cs`
and imports the `MyHomeRamen.AppHost\Configurations\Keycloak` directory.
That directory currently contains:

- `realm.json`: application realm `my-home-ramen-realm`; currently declares
  client `blazor-client`.
- `realm-import.json`: Keycloak master-realm export.

The files currently do **not** show the admin service-account client described
in earlier versions of this guide. Verify the effective realm/client setup
before expecting local registration to work.

## Configuration flow

The AppHost passes realm and audience settings to the API and realm and Blazor
client credentials to the Blazor app. Aspire provides the Keycloak endpoint to
services. API JWT bearer validation uses the configured authority, audience,
and issuer. The backend admin client uses client credentials to call Keycloak's
Admin REST API.

Development settings currently include:

```json
"Authorization": {
  "Realm": "my-home-ramen-realm",
  "Audience": "my-home-ramen-client"
}
```

The tracked realm currently names its Blazor client `blazor-client`, while the
development API audience is `my-home-ramen-client`. Confirm that the imported
realm, OIDC client ID, token audience mapper, and API audience are aligned.
Do not assume matching client names or an audience mapper unless the effective
Keycloak configuration confirms them.

The Blazor app expects:

- `Authentication:Blazor:ClientId`
- `Authentication:Blazor:ClientSecret`

The backend admin integration expects:

- `Authentication:KeycloakAdmin:ClientId`
- `Authentication:KeycloakAdmin:ClientSecret`
- `Authorization:Realm`

Store secrets in AppHost User Secrets or another supported secret provider,
not in committed settings. Set values to match actual Keycloak clients:

```powershell
dotnet user-secrets set "Authentication:Blazor:ClientId" "<blazor-client-id>"
dotnet user-secrets set "Authentication:Blazor:ClientSecret" "<blazor-client-secret>"
dotnet user-secrets set "Authentication:KeycloakAdmin:ClientId" "<admin-client-id>"
dotnet user-secrets set "Authentication:KeycloakAdmin:ClientSecret" "<admin-client-secret>"
```

The admin client must have service accounts enabled and sufficient realm
permissions to create users. Current tracked realm imports do not document or
provision this client, so configure it in the effective realm or update the
tracked import before relying on registration.

## Configuration drift to resolve

Before treating local Keycloak setup as reproducible, align and verify:

1. The Blazor OIDC client ID and secret with the imported realm client.
2. API audience configuration with the access token's actual `aud` claim.
3. The backend admin client, service account, and required realm permissions.
4. Realm import and callback/redirect URIs for the local Blazor URLs.

The older application-scopes ADR describes Keycloak scopes and roles as
authorization inputs. That decision was superseded by
[ADR-0004](../Users%20ADR/0004-company-restaurant-roles-and-authorization.md);
current API permission checks use the Identity database. See
[authentication flow](../Authorization/authentication-flow.md) for request
flows and component responsibilities.
