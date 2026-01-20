---
agent: agent
model: Gemini 3 Pro (Preview)
description: Create a domain model with key
---

Your task is to create domain model with strongly typed Id in "MyHomeRamen.Api.Domain" project under {Module} folder.
If {Module} folder does not exist, create it.

Create {DomainModel}Id.cs file which should be public record struct that accepts a Guid value.
The record struct should implement IEntityId interface from "MyHomeRamen.Api.Common" project.
Add public static implicit operators to convert from Guid to {DomainModel}Id and from {DomainModel}Id to Guid.
Add public override readonly string ToString() method that returns the string representation of the Guid value.

Create {DomainModel}.cs file which should be a public sealed class.
The class should implement IEntity<{DomainModel}Id> interface from "MyHomeRamen.Api.Common" project.
Add a public property of type {DomainModel}Id named Id with a private setter.
