---
name: feature-implementation
description: Implement API endpoints, domain logic, and database access for given feature according to project standards.
---

# Feature Implementation

Use this skill when implementing a feature from a Drax Planner implementation plan.
Implement only the scope explicitly defined by the plan. Do not redesign existing
code, refactor unrelated areas, or make decisions assigned to a human.

## Required inputs

- Backend plan: `.github/plans/{feature}/backend-plan.md`
- Frontend plan: `.github/plans/{feature}/frontend-plan.md`
- Repository instructions required by the plan
- `.github/copilot-instructions.md`

## Backend workflow

Follow this order:

1. **Prepare**
   - Load `.github/copilot-instructions.md`.
   - Identify plan scope, affected module, aggregate, endpoint kind, route, and files listed in `## 2. Files to create / modify`.
   - For backend work, run the Slice Scaffold Script exactly as required by the implementation agent before loading additional implementation files.

2. **Load plan and instructions**
   - Load the applicable backend plan.
   - Load `.github/instructions/backend.instructions.md`.
   - Load `.github/instructions/backend-tests.instructions.md` when tests are in scope.
   - Load `.github/wiki/architecture.md` for cross-module or infrastructure wiring.

3. **Implement domain changes**
   - Apply the changes in `## 3. Domain changes`.
   - Add or update entities, value objects, aggregate behavior, constants, errors, validators, and domain tests as specified.
   - Keep business rules in the domain, not in endpoint handlers.

4. **Implement persistence**
   - Apply `## 4. Persistance` exactly.
   - Add or update EF configuration, repository specifications or queries, and DbContext wiring.
   - Create the migration using the planned name when migration is required.
   - Do not change persistence behavior outside the planned aggregate.

5. **Implement API contracts and slice**
   - Apply `## 5. API details`.
   - Implement DTOs, request/response contracts, commands or queries, handlers, validators, authorization, and endpoint registration in this order:
     `DTOs -> Request/Response -> Command/Query -> Handler -> Validator -> Endpoint`.
   - Keep API contracts separate from domain entities.
   - Publish integration events only through the planned contracts and module boundaries.

6. **Implement tests**
   - Add the unit tests and integration tests specified in `## 6. Tests`.
   - Follow repository naming, fixture, authorization, and Testcontainers conventions.
   - Do not add tests for unrelated behavior.

## Guardrails

- Do not edit files not listed or directly required by the plan.
- Do not add NuGet packages unless explicitly planned.
- Do not run builds or tests; the implementation workflow performs code changes and records the diff for a separate review/verification step.
- Do not run git commands other than the planned diff export.
- Do not silently skip a planned item. If the plan is incomplete or contradictory, stop and report the blocker.

## Completion

After implementation, create the planned diff artifact:

```powershell
git diff --no-color > .github/plans/{feature}/diff.patch
```

Finish with:

`Implementation completed. Diff saved to .github/plans/{feature}/diff.patch`
