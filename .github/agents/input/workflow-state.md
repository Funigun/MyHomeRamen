# Workflow State

> Set these fields before running any agent. All agents read scope from this file.

---

## Feature Info

| Field | Value |
|---|---|
| **Feature branch** | `feature/get_categories_options` |
| **Backend sub-branch** | `feature/get_categories_options_backend` |
| **Frontend sub-branch** | `feature/get_categories_options_frontend` |

---

## Active Scopes

> Set `yes` for each scope that applies to this feature. Planner produces one plan file per active scope.

| Scope | Active |
|---|---|
| `common` — shared contracts (`Common.Contracts`; no API/Blazor changes) | no |
| `backend` — API + Domain + Persistence + Worker changes (no Blazor) | yes |
| `frontend` — Blazor Server / WASM changes only | yes |

---

## Current Execution

> Update before each agent run. Points to the scope currently being processed.

| Field | Value |
|---|---|
| **Scope** | `backend` |
| **Mode** | `feature` |
| **Iteration** | `1` |

> **Mode values:**
> - `feature` — fresh run; full agent loop: 000 → 001 → 002 → 003 → 004
> - `review-fixes` — addressing reviewer feedback; skip researcher; runs: 001 → 002 → 003 → 004

> **Iteration**: 1–3. Set to `1` on a fresh scope run. Increment by 1 before each `review-fixes` run.
> After iteration 3, any remaining unresolved critical issues require human review.

---

## Scope Status

### `common` — runs on: `feature/<name>` (main feature branch)

| Step | Agent | Status | Output file |
|---|---|---|---|
| 0 | `000-researcher` | ⬜ pending | `research-report-common.md` |
| 1 | `001-planner` | ⬜ pending | `automated-plan-common.md` |
| 2 | `002-implementer` | ⬜ pending | — |
| 3 | `003-formatter` | ⬜ pending | — |
| 4 | `004-reviewer` | ⬜ pending | `review-results-common.md` |

### `backend` — runs on: `feature/<name>_backend` → merge → `feature/<name>`

| Step | Agent | Status | Output file |
|---|---|---|---|
| 0 | `000-researcher` | ⬜ pending | `research-report-backend.md` |
| 1 | `001-planner` | ⬜ pending | `automated-plan-backend.md` |
| 2 | `002-implementer` | ⬜ pending | — |
| 3 | `003-formatter` | ⬜ pending | — |
| 4 | `004-reviewer` | ⬜ pending | `review-results-backend.md` |

### `frontend` — runs on: `feature/<name>_frontend` → merge → `feature/<name>`

| Step | Agent | Status | Output file |
|---|---|---|---|
| 0 | `000-researcher` | ⬜ pending | `research-report-frontend.md` |
| 1 | `001-planner` | ⬜ pending | `automated-plan-frontend.md` |
| 2 | `002-implementer` | ⬜ pending | — |
| 3 | `003-formatter` | ⬜ pending | — |
| 4 | `004-reviewer` | ⬜ pending | `review-results-frontend.md` |

> Update status to `🔄 running`, `✅ done`, or `❌ failed` as agents complete.
> After the reviewer completes: set `Mode=review-fixes`, increment `Iteration`, re-run from step 1 (up to iteration 3).

---

## Execution Order

```
[You] Fill feature-brief.md + create feature branch + set Active Scopes above

  IF common is active:
    └─► checkout: feature/<name>
        └─► 000 → 001 → [002 → 003 → 004] × up to 3 iterations

  IF backend is active (if common is included it has to be finished before starting this step):
    └─► checkout: feature/<name>_backend
        └─► 000 → 001 → [002 → 003 → 004] × up to 3 iterations
        └─► merge feature/<name>_backend → feature/<name>

  IF frontend is active (if common is included it has to be finished before starting this step):
    └─► checkout: feature/<name>_frontend
        └─► 000 → 001 → [002 → 003 → 004] × up to 3 iterations
        └─► merge feature/<name>_frontend → feature/<name>

[You] Fix any remaining build errors
```
