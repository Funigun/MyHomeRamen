---
description : Guidelines for structuring feature folders
applyTo: '*MyHomeRamen.*Features*.cs'
---

# Feature folder Instructions

## Overview
This file provides guidelines for structuring feature folders in the MyHomeRamen project, following the Vertical Slice architecture pattern.

## Guidelines
- Each feature should be organized into its own folder
- Feature for read operations should implement following structure:
	- `{Feature}Endpoint.cs` which implements `IEndpoint` from `MyHomeRamen.Api.Common`
    - `Get{EntityName}List` folder for getting list of entities


## Example structures
|../Features/{Module}/{FeatureName}
|	-- Models/
|		-- RequestDto.cs
|		-- ReponseDto.cs
|		-- {FeatureName}Request.cs
|		-- {FeatureName}Response.cs
|		-- Mappings.cs
|	-- Policies/
|	-- {FeatureName}ValidationPolicy.cs
|	-- {FeatureName}AuthorizationPolicy.cs
|	-- {FeatureName}CachePolicy.cs
|-- {FeatureName}Endpoint.cs
|-- {FeatureName}Handler.cs


|../{Module}/Features/{FeatureName}
|	-- Models/
|		-- RequestDto.cs
|		-- ReponseDto.cs
|		-- {FeatureName}Request.cs
|		-- {FeatureName}Response.cs
|		-- Mappings.cs
|	-- Policies/
|	-- {FeatureName}ValidationPolicy.cs
|	-- {FeatureName}AuthorizationPolicy.cs
|	-- {FeatureName}CachePolicy.cs
|-- {FeatureName}Endpoint.cs
|-- {FeatureName}Handler.cs

