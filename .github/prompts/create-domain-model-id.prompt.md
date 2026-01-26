---
agent: agent
model: Gemini 3 Pro (Preview)
description: Create a domain model id
---

Your task is to create domain model strongly typed Id in "MyHomeRamen.Domain" project under {Module} folder.
If {Module} folder does not exist, create it.

Create {DomainModel}Id.cs file which should be public record struct that accepts a Guid value.
	- the record struct should implement IEntityId interface from "MyHomeRamen.Api.Common" project.
	- add public static implicit operators to convert from Guid to {DomainModel}Id and from {DomainModel}Id to Guid.
	- add public override readonly string ToString() method that returns the string representation of the Guid value.
