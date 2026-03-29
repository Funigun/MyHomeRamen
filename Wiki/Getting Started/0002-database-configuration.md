# Database Configuration

As described in [ADR-0003: Isolated Module Database Contexts](../Architecture/0003-module-specific-database-contexts.md), the **My Home Ramen** application implements a modular monolith architecture where each module (Menu, Order, Identity, etc.) has its own isolated database context and schema. 
To enforce security and strict isolation, each module connects to the database using a dedicated database user with limited permissions.

This requires a specific configuration setup to ensure that the correct connection strings are generated and passed to the services (API, Workers) by the Aspire AppHost.

## Configuration Structure

The configuration relies on constants defined in `MyHomeRamen.AppHost.Configurations.Common.ConfigurationConstants`.
The main configuration section is `{SectionName}`, which corresponds to `DatabaseConfiguration`.
Module configurations are nested under this section using their respective module names.

### required Configuration Constants

- **Section Name**: `DatabaseConfiguration`
- **Modules**:
  - `Menu`
  - `Reservation`
  - `Order`
  - `ShoppingCart`
  - `Payment`
  - `Identity`

## Setting Up User Secrets

For development, it is recommended to use **User Secrets** to store sensitive database credentials. This prevents passwords from being committed to the source control.

You can manage user secrets via Visual Studio ("Manage User Secrets" on the AppHost project) or via the CLI.

### 1. Server and Database Settings

Define the database server address and the physical database name.

```powershell
dotnet user-secrets set "DatabaseConfiguration:Server" "."
dotnet user-secrets set "DatabaseConfiguration:DatabaseName" "MyHomeRamenDb"
```

### 2. Module Credentials

Create a specific user and password for **each** module. These credentials will be used by the application to connect to the module's specific schema.

**Pattern**: `dotnet user-secrets set "DatabaseConfiguration:{Module}:User|Password" "{Value}"`

**Example (Menu Module):**
```powershell
dotnet user-secrets set "DatabaseConfiguration:Menu:User" "MenuUser"
dotnet user-secrets set "DatabaseConfiguration:Menu:Password" "StrongPassword123!"
```

**Repeat this step for all modules:** `Menu`, `Reservation`, `Order`, `ShoppingCart`, `Payment`, `Identity`.

### 3. Connection String Template

Define a connection string template that the application will use to construct the final connection string for each module. 
The application will replace the placeholders `[Server]`, `[DbName]`, `[UserName]`, and `[Password]` with the values configured above.

```powershell
dotnet user-secrets set "DatabaseConfiguration:ConnectionTemplate" "Server=[Server];Database=[DbName];User Id=[UserName];Password=[Password];Trusted_Connection=False;TrustServerCertificate=True"
```

> **Note:** Ensure usage of `Trusted_Connection=False` so that the specific User/Password credentials are used instead of Windows Authentication. `TrustServerCertificate=True` is often needed for local development environments.

## AppHost Orchestration

The **Aspire AppHost** reads these configurations and injects the appropriate connection strings into the dependent projects (API, Identity API, Workers). 
You do **not** need to configure these settings individually in every project's `appsettings.json`, as they are centrally managed and passed down.

## Database Initialization Worker

The `MyHomeRamen.Worker.DbInitializer` project plays a critical role in the database setup. It connects with a privileged account (usually implied by the default creation or a specific admin setup not valid for runtime modules) to perform restricted operations.

On application startup, this worker executes once and is responsible for:
1.  **Creating the Database**: If it does not exist.
2.  **Creating Schemas**: Ensures a schema exists for each module (e.g., `menu`, `order`).
3.  **Running Migrations**: Applies Entity Framework Core migrations for every module to keep the schema up to date.
4.  **Security Setup**:
    - Creates the SQL Logins and Users for each module (using the credentials from the configuration).
    - Assigns specific roles and permissions to ensure a module can only access its own schema.

This automation ensures that when the API and UI projects start, the database is fully prepared and secured.
