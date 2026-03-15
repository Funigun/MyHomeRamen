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

## Domain Validation Tests

Domain validation tests verify that domain model factory methods (e.g., `Product.Create(...)`) enforce business rules by throwing `DomainException` on invalid input.

### Conventions
- One test class per domain model, named `{Model}ValidationTests` (e.g., `ProductValidationTests`).
- Use a private helper factory method (e.g., `CreateProduct(...)`) with optional named parameters and sensible valid defaults so each test only sets the value under test.
- Valid defaults must satisfy all domain constraints so that only the property under test is the failure trigger.
- Each test covers one invalid case and asserts:
  - `Assert.Throws<DomainException>(() => ...)` is thrown.
  - The exception message matches the corresponding `{Model}Errors.*().Message` constant.
- The happy path test (`Create_Should_SetPropertiesCorrectly_When_InputIsValid`) verifies all properties are set correctly.

### Naming pattern
`Create_Should_ThrowDomainException_When_{Reason}` (e.g., `Create_Should_ThrowDomainException_When_NameIsTooShort`)

### What to cover
- Every min/max boundary for string lengths, numeric ranges, and collections.
- Uniqueness constraints within collections (e.g., duplicate ingredients, duplicate categories).
- Type/enum constraints on related entities (e.g., a `CategoryType.Ingredient` category is invalid for a product).

## API Validator Tests

API validator tests verify that FluentValidation validators in `MyHomeRamen.Common.Contracts` enforce contract-level rules correctly.

### Conventions
- One test class per validator, named `{Model}ValidatorsTests` (e.g., `ProductValidatorsTests`).
- Instantiate the validator directly — no mocking required.
- Call `validator.Validate(value)` and assert on the returned `ValidationResult`.
- For failure cases assert `result.IsValid == false` and that `result.Errors` contains an error matching the expected message fragment.
- For success cases assert `result.IsValid == true`.
- Add a consistency test that verifies each validator constant (e.g., `MaxLength`, `MinLength`, `MinPrice`) matches the corresponding `ProductConstants` value to keep contract and domain in sync.

### Naming pattern
`{ValidatorName}_Should_{Pass|Fail}_When_{Reason}` (e.g., `ProductNameValidator_Should_Fail_When_NameIsEmpty`)

### What to cover
- Empty / null input.
- Each boundary violation (too short, too long, too low, too high).
- A valid value (happy path).
- A constant-consistency test per exported constant (e.g., `ProductNameValidator_Should_HaveSameMaxLengthAsDomain`).
