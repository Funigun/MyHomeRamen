# Feature Brief

---

## 0) Feature Brief guidance

While creating a feature brief focus on providing high-level overview of the feature such as:
- type of task (new feature, refactor, bug fix, etc.)
- module and feature name
- short description of the feature for both backend and frontend
- reference features that are being replaced or related to the new feature
- scope of the feature (which parts of the system are affected)
- who is the feature for (which user roles will benefit from it: Anonymous, Manager, Employee, Customer)
- what are the testing requirements for the feature (unit tests, integration tests, architecture tests, system tests)

Do not include implementation details in the feature brief, focus on the high-level overview and requirements.

---

---

## 1) Task Overview

| Field | Value |
|---|---|
| **Task type** | `Feature | Refactor | Change | Bugfix` |
| **Module** | `Menu | Orders | ShoppingCart | Reservations | Payments | Users` |
| **Accessibility** | `Manager | Employee | Customer | Anonymous` |
| **Feature name** | `<Name of feature>` |
| **Short backend description** | `<Short backend description>`|
| **Short frontend description** | `<Short frontend description>` |
| **Reference feature** | `<Reference feature>` |

---

---
## 2) Scope

| Scope | Include? |
|---|---|
| `backend` — API + Domain + Persistence | yes |
| `frontend` — Blazor Server / WASM | yes |

---

---
## 3) Feature description (Backend scope)

**For new feature flow include:**
- description of the new API endpoint(s) to be created
- reference feature

**For refactor or change flow include:**
- description of the current implementation
- description of the new implementation
- impact of the change e.g. simplification, performance improvement, better separation of concerns, etc.
- reference feature
 
**For bugfix flow include:**
- feature that is affected by the bug
- description of the bug
- steps to reproduce the bug

---

---

## 4) Feature description (Frontend scope)

**For new feature flow include:**
- description of the new page(s) to be created/modified
- description of the new component(s) to be created/modified
- reference feature

**For refactor or change flow include:**
- description of the current implementation
- description of the new implementation
- impact of the change e.g. simplification, better user experience, etc.
- reference feature

**For bugfix flow include:**
- feature that is affected by the bug
- description of the bug
- steps to reproduce the bug

---

---
## 5) Testing Requirements

**Unit tests:**
- if they are in scope or not with justification
- if they are in scope, provide high-level description of the tests to be created or modified
- reference tests for similar features that can be used as a pattern

**Integration tests:**
- if they are in scope or not with justification
- if they are in scope, provide high-level description of the tests cases to be created or modified
- reference tests for similar features that can be used as a pattern

**Architecture tests:**
- if they are in scope or not with justification
- if they are in scope, provide high-level description of the tests to be created or modified
- reference tests for similar features that can be used as a pattern

**System tests:**
- if they are in scope or not with justification
- if they are in scope, provide high-level description of the tests to be created or modified
- reference tests for similar features that can be used as a pattern

---

---

## 6) Additional Notes


---