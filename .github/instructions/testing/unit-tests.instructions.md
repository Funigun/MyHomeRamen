---
description : Guidelines for implementing unit tests
applyTo: '*MyHomeRamen.UnitTests*'
---

# Unit Tests Instructions

## Overview
Unit tests (`MyHomeRamen.Tests.Unit`) focus on domain models and simple services that don't require infrastructure or external dependencies.

## Guidelines
- Test domain validators and methods.
- Test services that contain business logic without infrastructure or external dependencies.
- Mock dependencies (e.g., DbContext, services).
- Use AAA pattern: Arrange, Act, Assert.
- Cover happy paths, validations and edge cases.
- Use `Theory` and `InlineData` for parameterized tests that cover up to 5 scenarios per test method.
- Use `Theory` and `MemberData` for more complex parameterized tests that require multiple parameters or complex objects.
  Test data should be defined in separate class in dedicated folder for reusability and maintainability
- Use `AssemblyFixture` for setup shared across all tests
- Use `CollectionFixture` for setup shared accross tests for specific module

## Tools
- xUnit for framework.
- NSubstitute for mocking.
- NSubstitute syntax for assertions.

## How to structure tests
- Dedicated project for unit tests: `MyHomeRamen.UnitTests`.
- Organize tests by module e.g. `MenuModule`, `OrderModule`.
- Organize module folders by domain models e.g. `Ingredients`, `Orders`.
- Create a `Common` folder for concrete module which contains shared test utilities e.g. Collection Fixtures, Test data, etc.

## Naming conventions
- Test class names should end with `Tests` (e.g., `IngredientValidatorTests`)
- Domain model tests might be divided into separate classes e.g. `IngredientBehaviorTests`, `IngredientEventsTests`, etc.

## Example
|../MyHomeRamen.UnitTests/
|-- MenuModule/
|	-- Common/
|		-- AssemblyFixtures/
|		-- TestData/
|	-- Ingredients/
|		-- Common/
|			-- CollectionFixtures/
|			-- TestData/
|		-- IngredientValidatorTests.cs
|		-- IngredientBehaviorTests.cs
|		-- IngredientEventsTests.cs
|	-- Products
|	-- Categories
