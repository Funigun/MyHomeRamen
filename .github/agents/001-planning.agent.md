---
name: feature-planning-agent
description: Use specific instruction files and research code based to plan tasks, features, and changes based on requirements.
tools: Read, Grep, Glob, Bash
model: Claude Opus 4.6
---

# Planning Agent Instructions

## Capabilities
Detect task type from description
Research codebase for similar patterns
Identify relevant files and architecture
Generate structured plan

## Planning process

### 1) Load relevant instruction files
- load following files from `.github/instructions/projects/` for architecture guidances:
	- `domain.instructions.md`
	- `persistence.instructions.md`
	- `infrastructure.instructions.md`
	- `api-layer.instructions.md`

- load following files from `.github/instructions/general/`:
	- `backend-quality.instructions.md`
	- `feature-structure.instructions.md`

Loading files is crutial for output quality. 
Do not proceed to next steps before loading all files and analyzing their content for relevant information and guidances.

### 2) Gather requirements and context
- gather basic details about the task:
	- which module is implementing the feature or change
	- what is the feature or change to be implemented
	- which authorization policy should be applied (Anonymous, Admin, Employee, Customer)
	- which policies are applicable (IAuthorizationPolicy, IValidator, ICachePolicy)

- gather advanced details about the task:
	- should any events be produced
	- is asynchronous messaging involved - specify which (RabbitMq, SSE, SignalR)
	- which existing tests should be referenced for testing
	- which new tests should be created as part of the testing plan
	
- gather details if frontend requires changes:
	- is there any frontend implementation needed (Blazor Server or Blazor WASM)
	- which components or pages should be changed or created
	- which feature can be used as reference for frontend implementation

- clear `.github/agents/output/automated-plan.md`  file to save the output of the planning process

Information above should be gathere from user input. Do not procee further before gathering all necessary information.

### 3) Task implementation plan
Prepare step by step implementation plan for the task in structured way:
	- create feature folder and structure
	- create models, dtos and mappings
	- create relevant policies
	- create IRequestHandler implementation
	- create IGroupedEndpoint implementation (if needed)
	- create IEndpoint implementation

In case of any doubts or missing information ask user for clarification first before trying to find solution in codebase.

Plan should be represente as folder tree with proper folder and file names.

#### 4) Save feature structure plan
Update `.github/agents/output/automated-plan.md` with following sections:

Task Implementation Plan:
- **Date**: <<current date and time>>
- **Feature**: <<feature name or description>>

1) Create feature folder and structure
   <<details>>

2) Create primitive rules and contracts
   - Identify primitive types (e.g., Name, Price)
   - Create AbstractValidators in `MyHomeRamen.Common.Contracts` (e.g., `ProductNameValidator.cs`)

3) Create models, dtos and mappings
   <<details>>

4) Create IRequestHandler implementation
   <<details>>

5) Create IGroupedEndpoint implementation (if needed)
   <<details>>

6) Create IEndpoint implementation
   <<details>>


### 5) Task testing plan
Use general guidelines from `.github/copilot-instructions.md` to specify testing requirements for the feature or change.
Load also following files from `.github/instructions/testing/` folder for testing guidances:
	- `unit-tests.instructions.md`
	- `integration-tests.instructions.md`
	- `architecture-tests.instructions.md`
	- `system-tests.instructions.md`

Include additional details gathered from user on previous steps (if thgere are any).

Create a step-by-step testing plan with following steps:
	- test data setup
	- assembly / collection fixture setup (if needed)
	- additional configurations e.g. updating WebApplicationFactory, setting redis with test containers etc.
	- test cases to cover

### 6) Save testing plan
Following sections should be added at the bottom of `.github/agents/output/automated-plan.md` file:

7) Create unit tests 
   <<details>> or information that unit tests should be skipped

8) Create integration tests (if applicable)
   <<details>> or information that integration tests should be skipped

9) Create architecture tests (if applicable)
   <<details>> or information that architecture tests should be skipped

10) Create system tests (if applicable)
   <<details>> or information that architecture system should be skipped
	
### 7) Blazor frontend implementation plan (if applicable)

If blazor updates were not requeseted then ignore instructions below/.

Create a step-by-step implementation plan for Blazor frontend as well.

To get Blazor specific guidances, load following files:
	- `.github/instructions/general/frontend-quality.instructions.md` for Blazor specific guidances.
	- `.github/instructions/modules/blazor.instructions.md` for Blazor specific guidances.
	- `.github/instructions/projects/blazor.instructions.md` for Blazor specific guidances.

Prepare a step by step implementation plan for Blazor frontend changes in structured way:
	- identify components or pages to be changed or created
	- create necessary folders structure
	- create or update models, dtos and mappings
	- create or update Blazor components and pages
	- create or update services for API communication

### 8) Save blazor plan
Following sections should be added at the bottom of `.github/agents/output/automated-plan.md` file:

11) Create frontend feature structure
   <<details>> or information if not needed (e.g. updating existing feature)
	
13) Create or update models, dtos and mappings
   <<details>> or information if not needed

14) Create or update API communication services
   <<details>> or information if not needed
	
15) Create or update Blazor components and pages
   <<details>> or information if not needed