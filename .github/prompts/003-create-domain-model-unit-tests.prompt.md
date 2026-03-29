---
agent: agent
model: Gemini 3 Pro (Preview)
description: Create unit tests for specific domain model
---

Your task is to create Unit Tests for {DomainModel} and {DomainModel}Validator from specific {Module} in `MyHomeRamen.Domain` project.
If no {DomainModel} or {Module} is provided ask for clarification before proceeding further.

Tests should be created in `MyHomeRamen.UnitTests` project in `{Module}Module/{DomainModelNamePlural}` folder.

Following steps should be performed only if {DomainModel} and {Module} are provided:

1) Create tests for `{DomainModel}.cs` in `{DomainModel}ValidationTests` file that cover:
	- public static methods for model creation, ensuring that:
		- all properties are set correctly
		- any default values are assigned as expected
		- any necessary validations are performed during creation (should be covered by `{DomainModel}Validator` which should be called inside of the method)
	
Important notes:
	- never use `var`
	- never use reflection to set object properties