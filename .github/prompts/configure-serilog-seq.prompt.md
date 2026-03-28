---
description: Configure Serilog with Seq sink and Aspire integration for the entire solution.
model: gpt-4o
---

Your task is to configure Serilog for the current solution, enabling structured logging, OpenTelemetry integration (for Aspire Dashboard), and a Seq sink.

Follow these steps strictly. If you cannot find a specific file mentioned, ask the user to provide the path to the relevant project file.

### 1. Analyze Solution Structure
Identify the following projects in the solution:
- **Central Package Management**: `Directory.Packages.props`
- **Global Build Properties**: `Directory.Build.props`
- **Aspire Host**: The project ending in `.AppHost`
- **Service Defaults**: The project ending in `.ServiceDefaults` (used for shared startup logic)
- **Target Applications**: All API, Worker, and Web projects that need logging.

### 2. Update Package Dependencies
Check `Directory.Packages.props` and ensure the following packages are present with appropriate versions (target .NET 8/9/10 as appropriate):
- `Serilog`
- `Serilog.AspNetCore`
- `Serilog.Sinks.Seq`
- `Serilog.Sinks.OpenTelemetry`

Check `Directory.Build.props` and add `PackageReference` entries to the appropriate functional groups (e.g., Web API, Workers, Blazor Server) so individual projects inherit them.

### 3. Configure Infrastructure (AppHost)
In the `.AppHost` project `Program.cs`:
1.  Register a **Seq** container resource. Since there might not be a first-party `Aspire.Hosting.Seq` package, use the standard `builder.AddContainer("seq", "datalust/seq")` approach.
2.  Expose the UI port and the ingestion port (standard Seq ports: 5341 for ingestion, 80/5341 for UI).
3.  Ensure the Seq resource is added as a reference (`.WithReference()`) to all Target Application projects defined in Step 1.

### 4. Implement Shared Logging Logic (ServiceDefaults)
In the `.ServiceDefaults` project (usually `Extensions.cs` or similar):
1.  Locate the method configuring the Host (e.g., `AddServiceDefaults` or `ConfigureOpenTelemetry`).
2.  Add `builder.Host.UseSerilog((context, loggerConfig) => { ... })`.
3.  Configure Serilog to:
    - Read configuration from `context.Configuration`.
    - Enrich with functionality (ThreadId, MachineName, FromLogContext).
    - Write to **OpenTelemetry** (so logs appear in the Aspire Dashboard).
    - Write to **Console**.
    - Write to **Seq** *only if* the connection string/endpoint for Seq is present in the configuration. Use the service name defined in AppHost (e.g., `http://seq:5341`).

### 5. Update Application Configuration
For each target application (API, Worker, Blazor):
1.  Update `appsettings.json` and `appsettings.Development.json`.
2.  Remove the standard `Logging` section (or keep it minimal and subordinate).
3.  Add a `Serilog` section.
4.  Configure `MinimumLevel` overrides for noisy namespaces (e.g., `Microsoft.AspNetCore`, `System.Net.Http`).

### 6. Verification
Review the changes to ensure:
- No hardcoded connection strings (use Aspire service discovery).
- Log levels are set efficiently for Development vs Production.