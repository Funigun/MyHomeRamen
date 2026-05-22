# Feature Description Template

> **Instructions for planning agent**
> Fill in every section below. The planning agent will use this document to produce
> `plan.approved.md`, whose §3 table is consumed verbatim by `slice-scaffold.ps1`.
> Delete guidance lines (starting with `>`) before submitting.

---

## 1. Feature title

<!-- One-line name, e.g. "Create Order" -->

## 2. Module

<!--
Which bounded-context module does this feature belong to?
Must match the folder names used in MyHomeRamen.Api, MyHomeRamen.Domain,
MyHomeRamen.Persistance, and MyHomeRamen.Common.Contracts.

Known modules: Menu | Users | ShoppingCart | Orders | Reservations | Payments
-->

## 3. HTTP operation

<!--
Describe the public API surface for this feature.

  Verb    : GET | POST | PUT | DELETE
  Route   : route template relative to "api/", e.g.  menu/ingredients/{id}
  Auth    : one of the policy constants below (or "none" for anonymous):
              RestaurantCustomerPolicy
              RestaurantEmployeePolicy
              RestaurantManagerPolicy
              AnyAuthenticatedPolicy
-->

- **Verb**: 
- **Route**: `api/`
- **Auth policy**: 

## 4. Request / Response contract

<!--
Describe the shape of request and response data.
The planning agent will use this to scaffold files in MyHomeRamen.Common.Contracts.

### Request properties (leave blank for query-by-id / delete)
| Property | Type | Validation rules |
|----------|------|-----------------|
|          |      |                 |

### Response properties
| Property | Type | Notes |
|----------|------|-------|
|          |      |       |

### DTOs (optional — shared sub-objects)
| DTO name | Properties |
|----------|------------|
|          |            |
-->

## 5. Domain logic

<!--
Describe what must happen inside the handler:
- Which domain aggregate is created / mutated / queried?
- What business rules / invariants apply?
- What domain events (if any) should be raised?
-->

## 6. Persistence

<!--
- Which DbContext / DbSet is accessed?
- Any new EF Core configuration needed?
- Any new indexes or migrations?
-->

## 7. Cross-module integration (optional)

<!--
- Does this feature publish an integration event?
  If yes, name it: e.g. OrderCreatedIntegrationEvent
- Does it consume an integration event from another module?
-->

## 8. Validation rules

<!--
List all server-side validation rules that belong in the FluentValidation validator.
Include both sync rules (field length, format) and async rules (uniqueness DB checks).
-->

## 9. Authorization notes (optional)

<!--
Any resource-based authorization beyond the policy check on the endpoint?
E.g. "A customer may only read their own basket."
-->

## 10. Testing requirements

<!--
Describe which test scenarios the planning agent should scaffold.

Integration test cases (HTTP-level, TestContainers):
- [ ] Happy path — authenticated + authorized → expected status code
- [ ] Unauthenticated → 401
- [ ] Insufficient role → 403
- [ ] (add domain-specific failure cases, e.g. not found → 404, duplicate → 400)

Unit test cases (domain logic only):
- [ ] (add cases for each domain rule / invariant if applicable)
-->

## 11. Out of scope

<!--
Explicitly list anything that is NOT part of this feature to avoid scope creep.
-->

---

## Planning agent output contract

> The planning agent **must** produce a `plan.approved.md` file whose §3 table
> follows the format below exactly so that `slice-scaffold.ps1` can process it.
>
> **Column rules**
> - `File`    — back-tick-quoted path using `\` separators, relative to repo root.
> - `Action`  — `create` or `modify` (lowercase).
> - `Type`    — one of the supported type tokens listed below (lowercase, exact spelling).
> - `Options` — space-separated `key=value` pairs; omit if not applicable.
> - `Rationale` — one sentence.
>
> **Supported type tokens**
>
> | Token | Scaffold target | Required options |
> |-------|----------------|-----------------|
> | `endpoint` | `{Op}{Entity}Endpoint.cs` | `verb=` `route=` `auth=` |
> | `query` | `{Op}{Entity}Query.cs` | `verb=GET` `route=` |
> | `query-handler` | `{Op}{Entity}Handler.cs` | `verb=GET` |
> | `command` | `{Op}{Entity}Command.cs` — `ICommand<TResponse>` | `verb=POST\|PUT` |
> | `command-void` | `{Op}{Entity}Command.cs` — `ICommand` (no response) | `verb=DELETE` |
> | `command-handler` | `{Op}{Entity}Handler.cs` | `verb=` |
> | `mappings` | `Mappings.cs` | — |
> | `validator` | `{Op}{Entity}Validator.cs` | `verb=` |
> | `contract-request` | `Common.Contracts\{Module}\{Entity}\Requests\*.cs` | — |
> | `contract-response` | `Common.Contracts\{Module}\{Entity}\Responses\*.cs` | — |
> | `contract-dto` | `Common.Contracts\{Module}\{Entity}\DTOs\*.cs` | — |
> | `contract-validator` | `Common.Contracts\{Module}\{Entity}\Validators\*.cs` | — |
> | `domain-event` | `Domain\{Module}\Events\*Event.cs` | — |
> | `integration-event` | `Common.Contracts\Messaging\*IntegrationEvent.cs` | — |
> | `integration-test` | `IntegrationTests\{Module}Module\{Entity}\*Tests.cs` | `verb=` `route=` |
> | `unit-test` | `UnitTests\{Module}Module\{Entity}\*Tests.cs` | — |
>
> **Options reference**
>
> | Key | Values | Used by |
> |-----|--------|---------|
> | `verb` | `GET` `POST` `PUT` `DELETE` | endpoint, query, query-handler, command, command-void, command-handler, validator, integration-test |
> | `route` | route template without leading `/api/`, e.g. `menu/ingredients` or `menu/ingredients/{id}` | endpoint, integration-test |
> | `auth` | `RestaurantCustomerPolicy` `RestaurantEmployeePolicy` `RestaurantManagerPolicy` `AnyAuthenticatedPolicy` | endpoint |
> | `tags` | string — Swagger tag, defaults to Entity name | endpoint |
> | `desc` | string (no spaces — use underscores; agent may omit if short) | endpoint |
> | `hasresp` | `true` `false` | command-handler |
>
> **Path patterns** (must match exactly for the parser to recognise them)
>
> ```
> API slice   : MyHomeRamen.Api\{Module}\Features\{Entity}\{Operation}\{TypeName}.cs
> Contracts   : MyHomeRamen.Common.Contracts\{Module}\{Entity}\{Requests|Responses|DTOs|Validators}\{TypeName}.cs
> Domain event: MyHomeRamen.Domain\{Module}\Events\{TypeName}.cs
> Int. event  : MyHomeRamen.Common.Contracts\Messaging\{TypeName}.cs
> Int. test   : MyHomeRamen.IntegrationTests\{Module}Module\{Entity}\{TypeName}.cs
> Unit test   : MyHomeRamen.UnitTests\{Module}Module\{Entity}\{TypeName}.cs
> ```
>
> **Canonical file order for a POST slice** (planning agent should follow this order)
>
> 1. `contract-request`
> 2. `contract-response`
> 3. `contract-dto` (if needed)
> 4. `contract-validator` (if shared validation needed)
> 5. `command`
> 6. `validator`
> 7. `mappings`
> 8. `command-handler`
> 9. `endpoint`
> 10. `domain-event` (if raised)
> 11. `integration-event` (if published)
> 12. `integration-test`
> 13. `unit-test` (if domain logic warranted)
> 14. `modify` rows for DependencyInjection.cs, domain aggregate, etc.
>
> **Canonical file order for a GET slice**
>
> 1. `contract-request` (if query has parameters beyond route id)
> 2. `contract-response`
> 3. `contract-dto` (if needed)
> 4. `query`
> 5. `validator` (if query parameters need validation)
> 6. `mappings`
> 7. `query-handler`
> 8. `endpoint`
> 9. `integration-test`
> 10. `modify` rows for DependencyInjection.cs, etc.
>
> **Canonical file order for a DELETE slice**
>
> 1. `command-void`
> 2. `validator`
> 3. `command-handler`
> 4. `endpoint`
> 5. `domain-event` (if raised)
> 6. `integration-event` (if published)
> 7. `integration-test`
> 8. `modify` rows for DependencyInjection.cs, domain aggregate, etc.

---

## Example — completed description (POST)

> This section shows a filled-in example. Remove it from your actual submission.

### Feature title
Create Ingredient

### Module
Menu

### HTTP operation
- **Verb**: POST
- **Route**: `api/menu/ingredients`
- **Auth policy**: RestaurantManagerPolicy

### Request / Response contract

**Request**
| Property | Type | Validation rules |
|----------|------|-----------------|
| Name | string | 3–100 chars, unique across ingredients |
| Description | string | 10–500 chars |
| Price | decimal | > 0 |
| CategoryIds | `IEnumerable<Guid>` | at least one, all must exist |

**Response**
| Property | Type | Notes |
|----------|------|-------|
| Id | Guid | id of the created ingredient |

### Domain logic
Call `Ingredient.Create(...)` on the `Ingredient` aggregate.
Fetch the referenced `Category` entities from the DB and pass them to `Create`.
Persist and return the new id.

### Persistence
`IMenuDbContext` — `Ingredients` DbSet.

### Validation rules
- `Name`: 3–100 chars; must be unique (async DB check).
- `Description`: 10–500 chars.
- `Price`: must be > 0.
- `CategoryIds`: not empty; all ids must resolve to existing categories.

### Testing requirements
Integration:
- [x] POST valid body → 201 Created with `{ Id }`.
- [x] Unauthenticated → 401.
- [x] Employee/Customer role → 403.
- [x] Duplicate name → 400.

Unit: none (domain `Create` validation covered by existing `IngredientValidationTests`).

---

## Example §3 table (planning agent output)

> The planning agent must produce a table exactly like this inside `plan.approved.md §3`.

| File | Action | Type | Options | Rationale |
|------|--------|------|---------|-----------|
| `MyHomeRamen.Common.Contracts\Menu\Ingredients\Requests\CreateIngredientRequest.cs` | create | contract-request | | Public request DTO |
| `MyHomeRamen.Common.Contracts\Menu\Ingredients\Responses\CreateIngredientResponse.cs` | create | contract-response | | Public response DTO |
| `MyHomeRamen.Api\Menu\Features\Ingredients\CreateIngredient\CreateIngredientCommand.cs` | create | command | verb=POST | ICommand carrying the request |
| `MyHomeRamen.Api\Menu\Features\Ingredients\CreateIngredient\CreateIngredientValidator.cs` | create | validator | verb=POST | FluentValidation rules |
| `MyHomeRamen.Api\Menu\Features\Ingredients\CreateIngredient\Mappings.cs` | create | mappings | | Request → domain mappings |
| `MyHomeRamen.Api\Menu\Features\Ingredients\CreateIngredient\CreateIngredientHandler.cs` | create | command-handler | verb=POST | Orchestrates domain + persistence |
| `MyHomeRamen.Api\Menu\Features\Ingredients\CreateIngredient\CreateIngredientEndpoint.cs` | create | endpoint | verb=POST route=menu/ingredients auth=RestaurantManagerPolicy tags=Ingredients desc=Creates_a_new_ingredient | Minimal API endpoint |
| `MyHomeRamen.IntegrationTests\MenuModule\Ingredients\CreateIngredientTests.cs` | create | integration-test | verb=POST route=menu/ingredients auth=RestaurantManagerPolicy | HTTP-level integration tests |
| `MyHomeRamen.Api\Menu\DependencyInjection.cs` | modify | | | Register new validator and handler |
