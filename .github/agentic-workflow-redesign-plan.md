# Agentic Workflow Redesign Plan

## Goals

- Reduce token usage by eliminating iterative retry loops and expensive codebase scanning
- Keep developer review gates (HITL) after planning and after code review
- Replace dynamic pattern discovery with a static `patterns.md` reference file
- Make the workflow compatible with `slice-scaffold.ps1` for stub generation

---

## Supporting Files

### `.github/patterns.md` *(new)* - Done

A static, curated reference file that replaces runtime codebase scanning for "existing patterns".
It will be loaded by both the planning agent and implementation agent instead of them scanning the repo.

Contents structure:
- **Slice patterns** – canonical examples of `IRequestHandler`, `IEndpoint`, `IGroupedEndpoint` implementations with annotated code snippets
- **Domain patterns** – how entities, value objects, and domain services are structured
- **Test patterns** – xUnit unit test and integration test conventions
- **Naming conventions** – summary table of naming rules (Command/Query/Request/Response suffixes, file locations, etc.)
- **Common mistakes** – short list of things agents tend to get wrong (e.g. using `var`, wrong namespace)

This file is maintained by developers. When a new accepted pattern emerges from a code review, the reviewer note should be used as a prompt to update `patterns.md`.

---

## Scripts

### Scripts to Keep / Refactor

| Script | Status | Changes needed |
|---|---|---|
| `slice-scaffold.ps1` | **Keep, adapt** | Update path constants from `BookSlot.*` to `MyHomeRamen.*`; update template namespaces and class shapes to match this project's conventions |
| `lint-plan.ps1` | **Keep** | No changes needed; validates plan structure before HITL | 
| `lint-review.ps1` | **Keep** | No changes needed; validates review.md structure before HITL |
| `plan-context.ps1` | **Refactor → `plan-context-lite.ps1`** | Remove similar-slices scanning (replaced by `patterns.md`); remove agent-decisions.md rules section; keep only: keyword extraction from prompt, domain entity file listing, output `plan-context.md` next to the prompt |
| `scope-check.ps1` | **Keep, adapt** | Update allowed paths from `BookSlot.*` to `MyHomeRamen.*`; keep agent allow/deny logic |
| `review-precompute.ps1` | **Keep, adapt** | Update deny patterns and `BookSlot.*` path references to `MyHomeRamen.*`; simplify by removing verify-report.md dependency if verify step is removed |

### Scripts to Drop

| Script | Reason |
|---|---|
| `verify.ps1` | Verification loop removed (no retry iterations) |
| `agents-check-drift.ps1` | File structure verification not needed for now |
| `pr-finalize.ps1` | PR automation out of scope for now |
| `task-scaffold.ps1` | Duplicate of `plan-context.ps1` logic; consolidate into `plan-context-lite.ps1` |

### Scripts to Create

#### `new-task.ps1` *(new)*

Scaffolds a new task file at `.github/agents/tasks/{name}-task.md` using a standard template that is compatible with `slice-scaffold.ps1`.

Parameters:
- `-Name` – kebab-case name used for file naming
- `-Scope` – `backend` | `frontend` | `fullstack`

Template output includes pre-filled sections:
- Purpose
- Scope (`backend` / `frontend`)
- Area + Operation (used by slice-scaffold.ps1 to derive paths)
- Acceptance criteria (empty bullets for developer to fill)

---

## Updated Workflow

```
Developer writes task → [new-task.ps1] → task file
         ↓
[plan-context-lite.ps1] → plan-context.md
         ↓
Planning Agent → plan.md
         ↓
[lint-plan.ps1] → lint output
         ↓
⏸ DEVELOPER REVIEW → approves → plan.approved.md
         ↓
[slice-scaffold.ps1] → stub files created
         ↓
Implementation Agent → fills in stubs + non-stub files
         ↓
[review-precompute.ps1] → review-input.md
         ↓
Code Review Agent → review.md
         ↓
[lint-review.ps1] → lint output
         ↓
⏸ DEVELOPER REVIEW → accept or fix manually
```

No retry loops. Each agent runs once. Developer handles any remaining issues manually.

---

## Agent Descriptions

### Planning Agent (`drax-planning.agent.md`)

**Current state:** Loads instruction files conditionally, scans the repo for similar slices via `plan-context.ps1`, pulls rules from `agent-decisions.md`.

**Proposed changes:**
- Remove all dynamic codebase scanning instructions — agent must NOT scan for existing patterns
- Replace pattern discovery with a mandatory load of `.github/patterns.md` at the start
- Load `plan-context.md` (pre-computed by `plan-context-lite.ps1`) for keyword context and domain entity list only
- Plan output must remain compatible with `slice-scaffold.ps1`: the §3 "Files to create / modify" table must use the exact path format the scaffold script expects (`MyHomeRamen.*` paths)
- Remove frontend plan file process or keep it as a clearly separated optional section
- Keep the HITL gate: agent writes `plan.md`, lint runs, developer reviews and renames to `plan.approved.md`

### Implementation Agent (`drax-implementation.agent.md`)

**Current state:** Loads plan, loads instruction files, implements step by step, runs `dotnet build` and retries up to 3 times on errors.

**Proposed changes:**
- Load `.github/patterns.md` instead of searching the codebase for reference implementations
- Remove the build-retry loop — agent runs `dotnet build` once, reports any errors in a structured summary, and stops. Developer fixes manually if needed
- Explicitly use stubs created by `slice-scaffold.ps1` as the starting point — agent fills in logic, does not recreate file skeletons
- Keep loading scoped instruction files (backend/frontend) and `copilot-instructions.md`

### Code Review Agent (`drax-reviewer.agent.md`)

**Current state:** Loads instruction files, diffs changed files, reviews production and test code, runs architecture and unit tests, can trigger re-implementation loop.

**Proposed changes:**
- Load `review-input.md` (pre-computed by `review-precompute.ps1`) as the primary input instead of running git commands directly — reduces token use on mechanical greps
- Load `.github/patterns.md` as the reference for what "correct" looks like
- Remove any instruction to trigger re-implementation — review ends with `review.md` output only
- Keep severity levels (Critical / Warning / Information) and structured Findings table format validated by `lint-review.ps1`
- Keep running architecture tests and unit tests as part of the review (these are fast and high-value signal)
- HITL gate: developer reads review, decides what to fix manually or accept as-is

---

## File Layout After Redesign

```
.github/
├── agents/
│   ├── drax-planning.agent.md       ← updated
│   ├── drax-implementation.agent.md ← updated
│   ├── drax-reviewer.agent.md       ← updated
│   └── tasks/
│       └── {name}-task.md           ← created by new-task.ps1
├── plans/
│   ├── {name}-plan.md               ← written by planning agent
│   └── {name}-plan.approved.md      ← renamed by developer after HITL
├── scripts/
│   ├── new-task.ps1                 ← new
│   ├── plan-context-lite.ps1        ← refactored from plan-context.ps1
│   ├── slice-scaffold.ps1           ← adapted paths
│   ├── lint-plan.ps1                ← unchanged
│   ├── lint-review.ps1              ← unchanged
│   ├── review-precompute.ps1        ← adapted paths
│   └── scope-check.ps1              ← adapted paths
└── patterns.md                      ← new, maintained by developers
```
