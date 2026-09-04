---
description: 'Instructions for backend projects'
applyTo: '**/MyHomeRamen.UnitTests/**/*.cs'
---

# Backend Layer Instructions

## 1) General Guidelines
- Packages: XUnit.v3, NSubstitute
- Assertions: XUnit syntax
- Use AAA pattern
- Use `Theory` and `InlineData` for tests that cover up to 5 scenarios
- Use `Theory` and `MemberData` for complex tests that require multiple parameters or complex objects
- Define test data in separate files
- Always use `TheoryData<T>` (not `IEnumerable<object[]>`) for `public static [MemberData]` sources
- Endpoints, Query/Command handlers and Validation policies are globally registered, no need to verify that.

## 2) Unit tests (`MyHomeRamen.UnitTests`)

### 2.1) Conventions and strategy
- Domain models creation, validation and behavior.
- Location pattern: `{Module}/{Model}/{Concern}Tests.cs`.
- E.g. concern `ProductValidationTests`, `ProductValidatorTests`, `ProductBehaviorTests`, `ProductEventsTests`, etc.

### 2.2) Domain Creation & Validation
- Use a private helper factory method (e.g., `CreateProduct(...)`) with optional named parameters and sensible valid defaults so each test only sets the value under test.
- Every min/max boundary for string lengths, numeric ranges, and collections.
- Uniqueness constraints within collections (e.g., duplicate ingredients, duplicate categories).
- Type/Enum constraints on related entities (e.g., a `CategoryType.Ingredient` category is invalid for a product).
- Invalid state can cover one possibility per single run and verify results according to pattern:
  - `Assert.Throws<DomainException>(() => ...)` is thrown.
  - The exception message matches the corresponding `{Model}Errors.*().Message` constant.

### 2.3) Domain behavior and events
- Focus on whole domain logic from Aggregate root methods to domain events.

### 2.4) API Validators
API validator tests verify that FluentValidation validators in `MyHomeRamen.Common.Contracts`
Add one test **per exported constant** that matches corresponding `{Model}Constants` value in the Domain layer

Verify FluentValidation messages fragment as per table:
| Rule | Fragment to assert |
|------|--------------------|
| `NotEmpty` | `"not be empty"` |
| `MinimumLength` | `"minimum length"` |
| `MaximumLength` | `"maximum length"` |
| `GreaterThanOrEqualTo` | `"greater than or equal to"` |
| `LessThanOrEqualTo` | `"less than or equal to"` |