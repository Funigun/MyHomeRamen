# [feature|bug|refactor|optimize] <Feature Name>

---

## Guidance

A feature brief is a focused user story that captures **what** needs to be done and **why** — not how.
Keep it concise and implementation-detail free. Testing requirements and implementation patterns are derived during the planning step.

---

---

## Title

| Field | Value |
|---|---|
| **Type** | `feature \| bug \| refactor \| optimize` |
| **Module** | `Menu \| Orders \| ShoppingCart \| Reservations \| Payments \| Users` |
| **Aggregate** | `<Domain Aggregate>` |
| **Accessibility** | `Manager \| Employee \| Customer \| Anonymous` |
| **Name** | `<Short feature name>` |

---

---

## Description

> As a `<role>`, I want to `<goal>` so that `<benefit>`.

### Scope

| Area | Include? |
|---|---|
| `backend` — Domain + API + Persistence | yes \| no |
| `frontend` — Blazor Server / WASM | yes \| no |

### Backend

> *(Include only sections relevant to the task type)*

**Feature / Change:**
- What new behavior or data is introduced
- API endpoint(s): method, route, request/response shape (high level)
- Domain validation rules (business invariants on the aggregate)
- API-level validation rules (input constraints, e.g. required fields, length, format, IDs validations or other that require DB call)
- Caching requirements (e.g. cache invalidation rules, cache population rules)
- Domain events to publish (name + consuming module, if known)

**Refactor / Optimize:**
- Current behavior and its shortcomings
- Desired behavior and expected improvement (e.g. simplification, performance, separation of concerns)

**Bug:**
- Affected feature/aggregate
- Description of the incorrect behavior
- Steps to reproduce

### Frontend

> *(Include only sections relevant to the task type)*

**Feature / Change:**
- Page(s) to create or modify
- Component(s) to create or modify
- User interaction flow (high level)

**Refactor / Optimize:**
- Current implementation and its shortcomings
- Desired implementation and expected improvement

**Bug:**
- Affected page/component
- Description of the incorrect behavior
- Steps to reproduce

---