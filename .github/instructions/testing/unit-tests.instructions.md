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
- One test class per domain model grouping **all** its validators, named `{Model}ValidatorsTests` (e.g., `ProductValidatorsTests` covers `ProductNameValidator`, `ProductDescriptionValidator`, `ProductPriceValidator`).
- Instantiate each validator directly — no mocking required.
- Call `validator.Validate(value)` and assert on the returned `ValidationResult`.
- For failure cases assert `result.IsValid == false` and that `result.Errors` contains an entry where `string.IsNullOrEmpty(e.PropertyName)` (validators on primitives use `RuleFor(x => x)`) and `e.ErrorMessage` contains the expected FluentValidation message fragment (see fragments below).
- For success cases assert `result.IsValid == true`.
- Add one consistency test **per exported constant** that verifies the validator constant matches the corresponding `{Model}Constants` value in the Domain layer to keep contract and domain in sync.

### FluentValidation error message fragments
| Rule | Fragment to assert |
|------|--------------------|
| `NotEmpty` | `"not empty"` |
| `MinimumLength` | `"minimum length"` |
| `MaximumLength` | `"maximum length"` |
| `GreaterThanOrEqualTo` | `"greater than or equal to"` |
| `LessThanOrEqualTo` | `"less than or equal to"` |

### Naming pattern
`{ValidatorName}_Should_{Pass|Fail}_When_{Reason}` (e.g., `ProductNameValidator_Should_Fail_When_NameIsEmpty`)

Consistency tests: `{ValidatorName}_Should_HaveSame{ConstantName}AsDomain` (e.g., `ProductNameValidator_Should_HaveSameMinLengthAsDomain`)

### What to cover
Per validator, add the following tests in order:
1. Empty / null input (if the validator enforces `NotEmpty`).
2. **Min boundary violation** — value one unit below the minimum (e.g., `MinLength - 1` chars, `MinPrice - 0.1m`).
3. **Max boundary violation** — value one unit above the maximum (e.g., `MaxLength + 1` chars, `MaxPrice + 0.1m`).
4. A valid value (happy path).
5. **One consistency test per exported constant** — every `MinLength`, `MaxLength`, `MinPrice`, `MaxPrice`, etc. must have its own `_Should_HaveSame{Constant}AsDomain` test.
