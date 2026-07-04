# DB Access Refactor Plan — Menu

## Goal
Migrate Menu data access to the repository/unit-of-work pattern already used by the Menu features layer, removing direct EF Core coupling from domain and feature code where possible.

## Scope
- Complete the Menu refactor for remaining aggregates and query/specification surfaces.
- Preserve current behavior, schemas, and DI registrations.
- Keep changes aligned with existing Menu abstractions and persistence patterns.

## Planned work
1. Define aggregate contracts for Role and Permission aggregates
     - `I{Aggregate}Repository`
     - `I{Aggregate}Query`
     - `I{Aggregate}Specification`
   - Keep query APIs `AsNoTracking`-friendly and specification APIs write-capable.

Example :
     - `Categories/CategoryRepository.cs`
     - `Categories/CategoryQuery.cs`
     - `Categories/CategorySpecification.cs`

