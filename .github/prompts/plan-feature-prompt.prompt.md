---
agent: agent
---

The user must provide the **feature name** (e.g. `create-todo-item`). Use it as `{feature}` for all file paths below.

Clear following files content:
- `.github/agents/output/{feature}-plan-backend.md`
- `.github/agents/output/{feature}-plan-frontend.md`

Pass `{feature}` to [drax-planner] agent (`.github/agents/001-planning.agent.md`) to plan feature implementation
based on user information and `.github/agents/input/{feature}-brief.md`.
