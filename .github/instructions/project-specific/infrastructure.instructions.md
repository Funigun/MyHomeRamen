---
description : Guidelines for how infrastructure project is structured
applyTo: '*MyHomeRamen.Infrastructure*.cs'
---

# Infrastructure Instructions

## Overview
Infrastructure (`MyHomeRamen.Infrastructure`) project provides services for email, caching, messaging, etc.

## Guidelines
- Implement interfaces defined within `MyHomeRamen.Api.Common` project for external services.
- Each service should have its own folder (e.g., `Caching`, `Messaging`, `Email`).
- Each service should have its own {Service}Extensions.cs file for dependency injection setup.
- Each service should be well organized with folders for interfaces, models, dtos, implementations should stay in root folder by default which might change over time.

## Available Services:

### Caching Service
- Interface: `ICacheService`
- Operates with `ICachePolicy` which provides caching configuration for concrete operation e.g. expiration time, cache key, etc.
- Uses `HybridCache` library for caching implementation for both in-memory and distributed caching and possibility to use `Redis` or `FusionCache` under the hood.

### Keycloak Admin Service
- Interface: `IKeycloakAdminService`
- **Purpose**: Provides administrative operations for Keycloak to support user authentication and management in the MyHomeRamen application. This service enables the creation of users, retrieval of available roles, and fetching of employee data, aligning with the decision to use Keycloak as the authentication server (see ADR-0002: Users Authentication).
- **Components built around service**:
  - `KeycloakAdminService`: The concrete implementation of this interface, handling HTTP requests to Keycloak's admin API.
  - `KeycloakAdminExtensions`: Extension methods for registering the service and its dependencies in the dependency injection container.
  - `KeycloakAdminTokenHandler`: Manages the acquisition and caching of admin access tokens required for Keycloak API calls.
  - `KeycloakAdminOptions`: Configuration class holding settings like Keycloak server URL, client ID, and credentials.
  - `KeycloakAdminTokenCachePolicy`: Defines caching policies for admin tokens to optimize performance and reduce API calls.
  - DTOs (`KeycloakUserDto`, `KeycloakRoleDto`, etc.): Data transfer objects for exchanging data with Keycloak.
  - Dependencies: Relies on `ICacheService` for caching mechanisms and `HttpClient` for API communication.
	
### Messaging Service
<< TO DO >>

### Mailing Service
<< TO DO >>