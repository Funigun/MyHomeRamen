---
description : Step-by-step checklist for building a feature end-to-end following the modular monolith architecture
applyTo: '*MyHomeRamen.*.cs'
---

# End-to-End Feature Implementation Instructions

## Overview
This document outlines the sequential steps to implement a complete feature end-to-end across the MyHomeRamen workspace. It integrates domain, persistence, and API guidelines following the Vertical Slice Architecture.

## Implementation Steps

### 1. Define the Domain Model & Logic (`MyHomeRamen.Domain`)
- Define or identify the aggregate root, entities, and value objects.
- Ensure the aggregate root inherits from `AuditableEntity` and `IEntity<TId>`.
- Define the strongly typed ID.
- Create or update the `Validator` for the domain aggregate.
- Add domains events if necessary.

### 2. Configure Persistence (`MyHomeRamen.Persistance`)
- Implement the `IEntityTypeConfiguration` in the entity's configuration folder.
- Add necessary custom value converters.
- Define DbContext sets or specific persistence abstractions (e.g. `IOrdersDbContext`).
- Implement or update any required repository abstractions if caching or complex domain logic dictates it.

### 3. Setup the Feature Structure (`MyHomeRamen.Api` or `MyHomeRamen.Identity.Api`)
Follow the folder layout indicated in `feature-structure.instructions.md`.
- Create a new `{FeatureName}` folder inside the respective Module/Group.

### 4. Create Models and DTOs
- Create `{FeatureName}Request.cs` implementing `IRequest` or `IRequest<TResponse>`.
- Create `{FeatureName}Response.cs`.
- Define additional DTOs inside `Models/DTOs` for complex structures.
- Create static extension methods for mapping in `Mappings.cs`. Note: Never use AutoMapper.

### 5. Create Policies (Validation, Authorization, Caching)
- Create `{FeatureName}ValidationPolicy.cs` using FluentValidation (`AbstractValidator<TRequest>`). Divide basic string/len rules from database (persistence) rules.
- Optional: Create `AuthorizationPolicy` or `CachePolicy` according to business requirements.

### 6. Implement the Handler
- Create `{FeatureName}Handler.cs` (or within the `Endpoint` class depending on MediatR/minimal handler pattern used).
- Implement the asynchronous logic passing through repository / db context.

### 7. Create the Endpoint
- Create `{FeatureName}Endpoint.cs` implementing `IEndpoint`.
- Reference the correct `.WithGroupName()` defined in the `{FeatureNameGroup}.cs`.
- Attach required tags, descriptions, and policies via fluent configuration extension methods.

## Verification
- Run architecture tests to ensure modules aren't cross-referencing inappropriately.
- Validate that the single feature folder isolates the usecase logic appropriately from other endpoints.
