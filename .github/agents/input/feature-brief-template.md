# Feature Brief

> Copy this file to `feature-brief.md`, fill in all sections, then run the agent workflow.
> Agents skip interactive Q&A when this file is fully filled (no `TBD` or blank values).

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature` / `Bug` / `Refactor` / `Chore` |
| **Module** | `Menu` / `Orders` / `Payments` / `Reservations` / `ShoppingCart` / `Users` |
| **Feature name** | TBD |
| **Short description** | TBD |
| **Reference feature** | TBD — existing feature to use as implementation reference (e.g. `CreateCategory`) |

---

## 2) Scope

| Scope | Include? |
|---|---|
| `common` — shared contracts (`Common.Contracts`) | yes / no |
| `backend` — API + Domain + Persistence | yes / no |
| `frontend` — Blazor Server / WASM | yes / no |

---

## 3) API Details (backend scope)

| Field | Value |
|---|---|
| **HTTP method** | `GET` / `POST` / `PUT` / `PATCH` / `DELETE` |
| **Route** | TBD (e.g. `/api/menu/categories`) |
| **Authorization policy** | `Anonymous` / `Admin` / `Employee` / `Customer` |
| **Applies IValidator** | yes / no |
| **Applies ICachePolicy** | yes / no |
| **Applies IAuthorizationPolicy** | yes / no |

---

## 4) Domain Details (backend scope)

| Field | Value |
|---|---|
| **Aggregate / entity** | TBD |
| **New domain entity needed** | yes / no |
| **Domain events produced** | TBD — list events or write `none` |
| **Asynchronous messaging** | TBD — `RabbitMQ` / `SSE` / `SignalR` / `none` |

---

## 5) Persistence Details (backend scope)

| Field | Value |
|---|---|
| **EF migration needed** | yes / no |
| **New DbContext configuration needed** | yes / no |
| **New DB extension method needed** | yes / no — e.g. `IsNameUniqueAsync` |

---

## 6) Frontend Details (frontend scope)

| Field | Value |
|---|---|
| **Pages to create or update** | TBD |
| **Components to create or update** | TBD |
| **API service to create or update** | TBD |
| **Reference frontend feature** | TBD — existing Blazor feature to use as reference |

---

## 7) Testing Requirements

| Test type | Required | Notes |
|---|---|---|
| Unit tests | yes / no | TBD |
| Integration tests | yes / no | TBD — reference test class (e.g. `CreateCategoryTests`) |
| Architecture tests | yes / no | TBD — new boundary rules needed? |
| System tests | yes / no | TBD |

---

## 8) Additional Notes

> Any context that doesn't fit above — edge cases, constraints, dependencies on other features, links to designs or specs.

TBD
