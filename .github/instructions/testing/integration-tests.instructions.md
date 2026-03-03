# Integration Tests Instructions

## Overview
Integration tests (`MyHomeRamen.Tests.Integration`) test component interactions using real DB and services via Testcontainers.

## Guidelines
- Use Testcontainers for DB, Redis, RabbitMQ.
- Test full workflows (e.g., API to persistence).
- Clean up data between tests.
- Focus on data flow and contracts.
- Run in CI/CD pipelines.

## Tools
- xUnit for tests.
- Testcontainers for containers.
- EF Core for DB setup.

## Examples
- Test user registration with DB.
- Test messaging between workers.