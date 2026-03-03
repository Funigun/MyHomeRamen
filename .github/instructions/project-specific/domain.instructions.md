---
description : Guidelines for how domain project is structured in line with DDD principles and modular monolith architecture
applyTo: '*MyHomeRamen.Domain*.cs'
---

# Domain Instructions

## Overview
The domain layer (`MyHomeRamen.Domain`) contains core business logic, entities, value objects, and domain services.
It follows Domain-Driven Design (DDD) principles within the modular monolith architecture.

## Guidelines
- Define aggregates, entities, and value objects here.
- Keep domain logic pure, without external dependencies.
- Use domain events for cross-aggregate communication.
- Ensure entities have identity and behavior, not just data.
- Ensure that modules do not directly reference each other (enforced by architecture tests).
- Each aggregate should inherit from `AuditableEntity` and implement `IEntity<TId>` interface.
- Each model should have corresponding `Validator` class in the same folder which is a static class that with single internal method that validates the model.

## Structure
- Common folder for shared domain concepts, constants and errors
- Core folder for each of modules for logical separation
- Individual module folder contains:
	- Database subfolder - defined database interface
	- Events subfolder - domain events related to the aggregate
	- Subfolders for each aggregate which contains:
		- aggregate root class
		- strongly typed ID class
		- validator class for the aggregate root
		- enums related to the aggregate

## Example structure

MyHomeRamen.Domain/
|-- Common/
|-- Orders/
|   |-- Database/
|   |-- Events/
|   |-- Ingredients/
|   |-- Orders/
|   |-- Payments/
|   |-- Products/
|   |-- Users/