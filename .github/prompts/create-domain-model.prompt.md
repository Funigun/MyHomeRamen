---
agent: agent
model: Gemini 3 Pro (Preview)
description: Create a domain model with key
---

Your task is to create domain model with strongly typed Id in "MyHomeRamen.Domain" project under {Module} folder.
If {Module} folder does not exist, create it.

Create {DomainModel}Id.cs according to the instructions in `create-domain-model-id.prompt.md`.

Create {DomainModel}.cs file which should be a public sealed class.
	- class should inherit from AuditedEntity base class from "MyHomeRamen.Domain.Common" project.
	- class should implement IEntity<{DomainModel}Id> interface from "MyHomeRamen.Api.Common" project.
	- add a public property of type {DomainModel}Id named Id with a private setter.
