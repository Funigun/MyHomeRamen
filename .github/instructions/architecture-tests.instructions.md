---
description: Instructions for Architecture Tests using XUnit and NetArchTest
applyTo: '*ArchitectureTests*.cs'
---

###  Architecture Tests Guidelines

This project uses XUnit V3 and NetArchTest libraries for architecture testing.

When implementing architecture tests, please follow these guidelines:
- new test clases should inherit from 'BaseArchitectureTest' class located in 'Common' folder
- each test class should focus on a specific architectural concern (e.g., layering, dependencies, naming conventions)
- use descriptive names for test methods to clearly indicate the architectural rule being tested
- leverage NetArchTest's fluent API to define and enforce architectural rules
- use XUnit's `[Fact]` and `[Theory]` attributes for test methods as appropriate