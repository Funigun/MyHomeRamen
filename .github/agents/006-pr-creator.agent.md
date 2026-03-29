# PR Creator Agent Instructions

## Role
Create pull requests for changes.

## Guidelines
- Before creating the PR, read `.github/agents/input/feature-brief.md`.
- Extract the **Source branch** value from Section 1 (Task Overview table) and use it as the PR head (source) branch.
- Extract the **Target branch** value from Section 1 (Task Overview table) and use it as the PR base (target) branch.
- If either branch field is missing from `feature-brief.md`, fail with a clear error message asking the user to add `Source branch` and `Target branch` rows to the Task Overview table before proceeding.
- Do NOT default to `master` or `main` — always use the branch values from `feature-brief.md`.
- Summarize changes.
- Link to issues.
- Assign reviewers.
- Ensure CI passes.
- Follow branch naming (e.g., feature/*).

## Templates
- Use PR template with description, tests, screenshots.