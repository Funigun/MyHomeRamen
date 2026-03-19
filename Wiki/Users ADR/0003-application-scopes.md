---
title: "ADR-0003: Define Application Scopes and Role Bindings"
status: "Accepted"
date: "2026-03-14"
authors: "Funigun"
tags: ["security", "scopes", "roles", "architecture", "keycloak"]
---

### Status

**Accepted**

### Context

Following the decision to use Keycloak for authentication (see [ADR-0002](0002-users-authentication.md)), the MyHomeRamen application requires a scalable authorization strategy. To properly support the Modular Monolith architecture, we need a mechanism that separates global application access from module-specific permissions. 

**Key constraints and requirements:**
- Ensure the principle of least privilege across different application modules.
- Maintain global roles for general operational contexts.
- Tokens must only contain scopes that the user is explicitly authorized to hold.

### Decision

We will implement a structured system of roles and scopes within Keycloak, explicitly linking module-specific roles to their corresponding module scopes.

**The decision includes:**

1. **Global Roles:** Define top-level roles to manage general user classification across the application:
   - `Customer`
   - `Employee`
   - `Admin`
	
	Roles above should match with roles defined in `RoleConstants` class in `MyHomeRamen.Domain.Users` to maintain consistency between the domain model and Keycloak configuration.

2. **Per-Module Roles:** Define explicit roles per domain module to determine access levels within that boundary. For example, for a Menu module:
   - `MenuCustomer`
   - `MenuEmployee`
   - `MenuAdmin`

	Module specific roles should match with roles defined in `RoleConstants` class in `MyHomeRamen.Domain.{Module}` to maintain consistency between the domain model and Keycloak configuration.

3. **Application Scopes:** Configure optional scopes in Keycloak (which will be enforced as required by the backend API):
   - **General Scope (`my-home-ramen-scope`):** Provides general context to the token, such as audience and global roles.
   - **Module Scopes (e.g., `menu`):** Provides access to specific module endpoints.

4. **Scope-Role Linkage:** Module scopes are continuously linked to their respective module roles. A user token cannot be issued with a module scope (e.g., `menu`) unless the authenticated user is assigned at least one corresponding module role (e.g., `menu_customer`, `menu_employee`, or `menu_admin`).

### Consequences

#### Positive

- **POS-001: Precise Access Control:** By coupling scopes to structural module roles, the API naturally enforces strict boundary security.
- **POS-002: Modular Scalability:** New domains added to the system can simply introduce their own isolated roles and scopes without interfering with global application access.

#### Negative

- **NEG-001: Configuration Complexity:** Requires comprehensive Keycloak configuration mapping (managing client scopes, roles, and protocol mappers) for every new module introduced.
- **NEG-002: Client Overhead:** Client applications (such as the Blazor frontend) must be aware of and explicitly request the necessary optional scopes when attempting to access specific backend modules.