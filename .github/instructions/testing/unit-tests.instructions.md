# Unit Tests Instructions

## Overview
Unit tests (`MyHomeRamen.Tests.Unit`) focus on isolated component testing using xUnit and Moq.

## Guidelines
- Test individual classes/methods in isolation.
- Mock dependencies (e.g., DbContext, services).
- Use AAA pattern: Arrange, Act, Assert.
- Cover happy paths and edge cases.
- Run tests via dotnet test.

## Tools
- xUnit for framework.
- Moq for mocking.
- FluentAssertions for assertions.

## Examples
- Test domain services without DB.
- Test API endpoints with mocked handlers.