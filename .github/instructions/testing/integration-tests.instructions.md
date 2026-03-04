---
description: Guidelines for writing Integration Tests using Testcontainers and WebApplicationFactory
applyTo: '*MyHomeRamen.Tests.Integration*.cs'
---

# Integration Tests Instructions

## Overview
Integration Tests (`MyHomeRamen.Tests.Integration`) focus on bounded component testing, vertical slices (API -> Domain -> DB) in isolation using `WebApplicationFactory` and Testcontainers. They provide faster execution speed than full system tests while maintaining realism for persistence dependencies.

## Guidelines
- Spin up Testcontainers for persistence (DB, Cache).
- Use mocked/stubbed external boundaries (e.g., mock RabbitMQ publishers or Keycloak APIs) to avoid massive testing configurations.
- Reset database state between tests (using tools like Respawn or EF Core transaction rollbacks) to ensure test isolation.
- Focus on testing vertical slices within a single module.

## Tools
- `WebApplicationFactory`
- `xUnit`
- `Testcontainers` (MS SQL, Redis)

## Examples
- Testing `CreateProductEndpoint` to ensure it correctly maps requests, validates rules, returns a 201 Created, and stores data in the PostgreSQL DB.