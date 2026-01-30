---
title: "ADR-0002: .NET Aspire for Application Orchestration"
status: "Accepted"
date: "2025-01-30"
authors: "Funigun"
tags: ["architecture", "decision", "orchestration", "devops"]
---

### Status

**Accepted**

### Context

The MyHomeRamen solution consists of multiple interacting components:
- **Backend API**: The Modular Monolith core.
- **Workers**: Background processing (MailSender, MessagesHandler).
- **Frontend**: Blazor Server and Client (WASM).
- **Infrastructure**: Redis (caching), RabbitMQ (messaging), SQL Server (data).

Orchestrating these dependencies during local development is complex. Developers typically need to manage Docker Compose files, connection strings, port allocations, and startup sequences manually. Without a unified tool, "it works on my machine" issues are frequent, and onboarding new developers is slow.

We need a solution that simplifies local development orchestration, provides service discovery, and integrates well with the .NET ecosystem.

### Decision

We will use **.NET Aspire** for orchestration.

Key aspects of this decision:
1.  **AppHost Project**: A dedicated `MyHomeRamen.AppHost` project will define the system topology in C#.
2.  **Container Management**: Aspire will handle the lifecycle of external dependencies (Redis, RabbitMQ, SQL Server) as containers.
3.  **Service Discovery**: Aspire provides built-in service discovery, injecting endpoints into projects via environment variables, eliminating manual connection string management for internal services.
4.  **Observability**: The Aspire Dashboard will be the standard tool for viewing logs, traces, and metrics across all distributed components during development.

### Consequences

#### Positive

- **POS-001**: **Unified Experience**: One "Start" button launches the whole stack (API, Frontend, Database, Broker, Cache) in the correct order.
- **POS-002**: **Observability**: Immediate access to structured logs and distributed traces correlates requests across the frontend, API, and workers.
- **POS-003**: **Configuration as Code**: Infrastructure setup is defined in C#, which is strongly typed and refactor-friendly, unlike YAML.
- **POS-004**: **Standardization**: Enforces a consistent way to consume configuration and connection strings across the team.

#### Negative

- **NEG-001**: **Tooling Dependency**: Requires Visual Studio or specific DOTNET CLI workloads, which must be installed on all developer machines.
- **NEG-002**: **Leaky Abstraction**: While it simplifies local dev, the deployment story to production (e.g., to Azure Container Apps or Kubernetes) requires mapping the Aspire manifest to infrastructure manifests (though `Aspir8` or Azure Developer CLI helps).

### Alternatives Considered

#### Docker Compose

- **ALT-001**: **Description**: Manually maintain `docker-compose.yml` for infrastructure and Dockerfiles for .NET apps.
- **ALT-001**: **Rejection Reason**: Lacks deep integration with the IDE debugger. Harder to manage dynamic port allocation and service discovery configuration compared to Aspire.

#### Manual Setup

- **ALT-003**: **Description**: Run services manually and install infrastructure (Redis, RabbitMQ) directly on the host OS.
- **ALT-003**: **Rejection Reason**: High friction for setup, version conflicts, pollution of developer's OS environment.
