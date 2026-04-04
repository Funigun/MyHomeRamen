---
agent: agent
---

The user must provide the **feature name** (e.g. `create-todo-item`). Use it as `{feature}` for all file paths below.

Update or create following file depending on user description:
- `.github/agents/output/{feature}-brief.md`

- use `.github/agents/input/feature-brief-template.md` as template for the content
- use `.github/agents/input/feature-brief-example.md` as an example how to structure the content and what information to include

Do not modify any other files. Only update or create the file mentioned above.
