---
description: Guidelines for writing System and E2E Tests orchestrated by .NET Aspire
applyTo: '*MyHomeRamen.Tests.System*.cs'
---

# System & E2E Tests Instructions

## Overview
System/E2E Tests (`MyHomeRamen.Tests.System`) test complex distributed workflows spanning multiple independent services (API + Identity + Workers + External IdP). These tests are orchestrated by `.NET Aspire` to run the actual distributed application topology.

## Guidelines
- Reference the `MyHomeRamen.AppHost` project to bootstrap the complete application.
- Do NOT mock infrastructure (Keycloak, RabbitMQ, PostgreSQL, Redis). Let Aspire spin up the full containerized topology exactly like in the development environment.
- Focus testing on complete data flows, pub/sub consistency, and cross-module end-to-end contracts.

## Tools
- `.NET Aspire Testing` (`Aspire.Hosting.Testing`)
- `xUnit`

## Examples
- **User registration flow:** AppHost begins test -> Trigger API call -> Verify Identity module created user -> Verify Keycloak User Created -> Verify RabbitMQ Event emitted -> Verify `Worker.MailSender` picks up event and handles it.