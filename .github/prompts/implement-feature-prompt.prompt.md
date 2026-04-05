---
agent: agent
---

The user must provide the **feature name** (e.g. `create-todo-item`). Use it as `{feature}` for all file paths below.

Verify scope of work based on user information and `.github/agents/input/{feature}-brief.md`.

Perform following steps based on scope:

1) If scope includes backend - run [drax-implementer] agent (`.github/agents/002-implementation.agent.md`) to implement backend part of the feature. Pass `{feature}` to the agent.
2) If scope includes frontend - run [drax-implementer] agent (`.github/agents/002-implementation.agent.md`) to implement frontend part of the feature. Pass `{feature}` to the agent.
3) Ensure that solution is building correctly.
4) Run [drax-formatter] agent (`.github/agents/003-formatter.agent.md`) to format the code according to project standards. Pass `{feature}` to the agent.
5) Run [drax-reviewer] agent (`.github/agents/004-reviewer.agent.md`) to review the implementation and suggest improvements. Pass `{feature}` to the agent.