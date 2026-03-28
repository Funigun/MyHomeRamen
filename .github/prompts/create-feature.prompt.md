---
agent: agent
model: cloude-sonnet-4.6
---

Read `.github/agents/input/feature-brief.md` and `.github/agents/input/workflow-state.md` to understand the feature requirements and current workflow state.
Use feature/get_categories_options as current branch and target branch for PR.

Based on user input follow proper scope (backend, frontend, common) by:
- creating proper branch based on current branch
- updating workflow state with current scope and mode
- creating a Draft PR with proper title and description (see below for details)
- executing workflow according to `.github/agents/input/workflow-state.md` for proper scope

