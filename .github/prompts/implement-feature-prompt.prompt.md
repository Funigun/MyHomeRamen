---
agent: agent
---

The user must provide the **feature name** (e.g. `create-todo-item`). Use it as `{feature}` for all file paths below.

Verify scope of work based on user information and `.github/agents/input/{feature}-brief.md`.

Perform following steps based on scope:

1) If scope includes backend - run [drax-implementer] agent (`.github/agents/002-implementation.agent.md`) to implement backend part of the feature. Pass `{feature}` to the agent.
2) If scope includes frontend - run [drax-implementer] agent (`.github/agents/002-implementation.agent.md`) to implement frontend part of the feature. Pass `{feature}` to the agent.
3) Ensure that solution is building correctly.