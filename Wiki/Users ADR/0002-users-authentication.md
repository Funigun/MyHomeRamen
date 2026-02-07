---
title: "ADR-0001: Define Domain Model for Users Module"
status: "Accepted"
date: "2026-02-08"
authors: "Funigun"
tags: ["domain", "users", "architecture"]
---

### Status

**Accepted**

### Context

In MyHomeRamen application Authentication is handled by Keycloak in order to provide secure solution for user authentication flow.
Authentication from the other hand is handled by `MyHomeRamen.Identity.Api` (see [ADR-0001](0001-users-domain.md)).

**Key constraints and requirements:**
- **Proper authentication flow**: users registration, login etc. should be implemented according to best practices (OAuth2 protocol)
- **Simplicity**: avoid unnecessary complexity (manual implementation) in authentication flow

### Decision

We will use Keycloak as an authentication server for MyHomeRamen application

**The decision includes:**
1.  **Configuration**: Configure Keycloak via Aspire AppHost project for centralized infrastructure management and to ensure that all modules can easily integrate with it.
2.  **Integration**: Integrate Keycloak with `MyHomeRamen.Blazor`, `MyHomeRamen.Identity.Api` to handle user registration, login, and token management.

### Consequences

#### Positive

- **POS-001**: **Safety**: We are using battle-tested solution for authentication flow, which is crucial for security of the application.
- **POS-002**: **Separation**: In case of any changes to flow we can update Keycloak configuration or replace it with another solution without affecting business logic in `Users` module and its data.

#### Negative

- **NEG-001**: **Complexity**: We need to integrate Keycloak with our application including configuration and events (e.g. user registration) to ensure `Users` module is ready to serve other modules with new users.

