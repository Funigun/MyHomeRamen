# Architecture Tests Instructions

## Overview
Architecture tests (`MyHomeRamen.Tests.Architecture`) enforce rules using NetArchTest.

## Guidelines
- Define rules for layer dependencies (e.g., domain doesn't depend on infrastructure).
- Test for naming conventions.
- Ensure vertical slices are isolated.
- Run as part of build.

## Tools
- NetArchTest for assertions.

## Examples
- Domain should not reference EF Core.
- API should depend on domain, not persistence directly.