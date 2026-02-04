---
agent: agent
model: Gemini 3 Pro (Preview)
description: Create rules for specific domain model
---

Your task is to create rules for {DomainModel} from specific {Module} in `MyHomeRamen.Domain` project.
If no {DomainModel} or {Module} is provided ask for clarification before proceeding further.

Following steps should be performed only if {DomainModel} and {Module} are provided:

1) Check for existing rules

As we are working with modular monolith some models appear multiple times as they serve different purposes.
So the next step is to verify if rules for {DomainModel} already exist by performing following steps:
- check if `Common/{DomainModel}Constants.cs` file exists
- check if `Common/{DomainModel}Errors.cs` file exists

If files exist, read them and extract rules from them and move to point 3, otherwise move to point 2.

2) Create rules for {DomainModel}

Perform following steps to create rules for {DomainModel}:
	- read `{Module}/{DomainModelNamePlural}/{DomainModel}.cs` file and extract all public properties with their types
	- create `Common/{DomainModel}Constants.cs` file add constants related to {DomainModel} such:
		- min/max length for string properties, 
		- min/max values for numeric properties
	- create `Common/{DomainModel}Errors.cs` file and add error messages related to {DomainModel} such as
		- "Name too short" / "Name too long" for string properties
		- "Value too small" / "Value too large" for numeric properties
		- "Items are not unique" for collection properties
		- file should contain public static methods that create DomainException with appropriate error message for each rule

3) Create Validator for {DomainModel}
	- file should be created in `{Module}/{DomainModelNamePlural}/{DomainModel}Validator.cs`
	- it should be public static class with one method `Validate` that accepts {DomainModel} as parameter
	- `Validate` method should use Constants and Errors from Common folder to validate properties of {DomainModel}
	- `Validate` method should throw proper DomainException based on on `Common/{DomainModel}Errors.cs` file
