---
agent: agent
---

Verify scope of work based on user information and `.github/agents/input/feature-brief.md`.

Perform following steps based on scope:

1) If scope includes backend - run [drax-implementer] agent (`.github/agents/002-implementation.agent.md`) to implement backend part of the feature.
2) If scope includes frontend - run [drax-implementer] agent (`.github/agents/002-implementation.agent.md`) to implement frontend part of the feature.
3) Ensure that solution is building correctly.
4) Run [drax-reviewer] agent (`.github/agents/003-review.agent.md`) to review the implementation and suggest improvements.