---
name: feature-completion
description: Close a planned feature by validating review responses, creating a pull request, and moving its Azure DevOps work item to Code Review.
---

# Feature Completion

Use this skill after implementation and `drax-reviewer` finish.

## Preconditions

- Current branch contains feature changes and has no unresolved generated or unrelated changes in the PR diff.
- `.github/plans/{feature}/backend-plan.md` or `frontend-plan.md` exists.
- `.github/plans/{feature}/verify-report.md` exists and reports `PASS`.
- `.github/plans/{feature}/code-review.md` exists.
- Plan contains an `## Azure DevOps` section with work-item ID and project metadata.
- `develop` exists as target branch.

Never create a PR or change work-item state when verification failed, blocking findings remain, or Azure metadata is missing.

## Review response protocol

`code-review.md` findings table must contain stable finding numbers, severity, and status. Every finding requires a response in `.github/plans/{feature}/review-responses.md`:

```markdown
# Review Responses — {Feature}

| Finding | Status | Response | Evidence |
|---------|--------|----------|----------|
| 1 | addressed | <what changed or why no code change is needed> | <file:line, commit, or test> |
```

Allowed statuses: `addressed`, `accepted`, `not-applicable`. `open`, blank, or missing rows are unresolved. `accepted` and `not-applicable` require a concrete explanation and evidence. Blocking findings cannot be accepted without explicit user approval; stop and report them.

If reviewer output uses the legacy table without status, treat every non-`#` finding as unresolved and stop. Do not infer responses from a vague commit message.

## Azure DevOps and PR configuration

Use environment variables:

- `AZURE_DEVOPS_ORG`
- `AZURE_DEVOPS_PROJECT`
- `AZURE_DEVOPS_REPO`

Require `az login`, the `azure-devops` extension, and configured defaults. Never print credentials.

## Completion workflow

1. Read plan metadata, verify report, code review, and review responses.
2. Confirm all findings have response rows and no blocking finding is unresolved.
3. Inspect `git status --short` and `git diff --check`. Do not rewrite or discard user changes.
4. Push current branch when needed:

   ```powershell
   git push --set-upstream origin <feature-branch>
   ```

5. Create one PR from current branch to `develop`:

   ```powershell
   az repos pr create --repository "$AZURE_DEVOPS_REPO" --source-branch "<feature-branch>" --target-branch "develop" --title "<title>" --description "<body>" --output json
   ```

   PR body must include feature summary, work-item link, verifier result, review result, and review-response path. Do not create a duplicate PR; first query open PRs for the source branch:

   ```powershell
   az repos pr list --repository "$AZURE_DEVOPS_REPO" --source-branch "refs/heads/<feature-branch>" --status active --output json
   ```

6. Move work item to `Code Review` only after PR creation succeeds:

   ```powershell
   az boards work-item update --id <id> --state "Code Review" --output json
   ```

7. Report PR ID/URL and work-item ID/state. If either operation fails, report the failed operation and do not claim completion.

## Failure rules

Do not merge, complete, abandon, or delete branches. Do not modify source files. Do not bypass review responses. Do not change work-item state before PR creation.
