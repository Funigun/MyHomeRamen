---
description : Guidelines for the Blazor UI Presentation layer
applyTo: '*MyHomeRamen.Blazor*.razor, *MyHomeRamen.Blazor*.cs'
---

# Blazor Layer Instructions

## Overview
The Blazor frontend (`MyHomeRamen.Blazor` for Server, `MyHomeRamen.Blazor.Client` for WASM) provides the UI for restaurant management. 
It follows the Modular Monolith and Vertical Slice architecture patterns mirroring the backend modules.

## Server vs WASM Best Practices
- **Server Project (`MyHomeRamen.Blazor`)**: Best for initial page loads, SEO, prerendering, and components requiring direct, secure access to backend resources or heavy computations.
- **WASM Project (`MyHomeRamen.Blazor.Client`)**: Best for highly interactive UI features, offloading rendering logic to the client, and rich client-side responsiveness.

## Architecture & Patterns
- Follow **Vertical Slice Architecture** for features (e.g., placing `.razor`, `.razor.cs`, Form Models, and Validators in the same feature folder).
- Integrate with API via `HttpClient`.
- For state management during prerendering, rely on the `[PersistState]` attribute to securely hand off data from the Server to the WASM client.
- Handle authentication via `AuthHeaderHandler`.

## Guidelines
- Use **MudBlazor** as the core component library. Do not use Bootstrap.
- **Performance**: Implement purely presentational features with standard HTML rather than writing unnecessary Blazor components. Save Blazor components for stateful, interactive elements.
- **Validation**: Secure forms using `FluentValidation`. Validators should aggressively reuse primitive base validators from `MyHomeRamen.Common.Contracts`.
- Implement role-based UI rendering.

## Services
- **Common Services**: Belong in a shared Common folder. These include services like `MessageService` (wrapping MudBlazor dialogs/snackbars) or `BaseHttpClient` (handling auth headers, token refresh, and common API interactions).
- **Module-Specific Services**: Belong strictly within their respective module folders to maintain boundaries. This includes strongly typed HTTP clients (e.g., `CatalogHttpClient`) that leverage the central `BaseHttpClient`.

## Models & Forms
- **Decouple API DTOs from UI Models**: Do not bind UI forms directly to backend API DTOs.
  - Use **API DTOs** (e.g., `ProductDto`, `ProductToCreateDto`) strictly for backend network payloads.
  - Use **UI Models** (e.g., `ProductModel`) for housing UI state, reactive properties, and `FluentValidation` validator bindings.
- **Unified Forms**: Keep UI logic reusable by creating a single `{Feature}Form.razor` component that handles Create, View, and Update scenarios. Control this state with an explicit enum parameter (e.g., `[Parameter] public FormMode Mode { get; set; }`).

## Structure Example
|-- Components/ (Global components like MainLayout, NavMenu)
|-- Common/
|	-- Services/ (e.g., BaseHttpClient, MessageService)
|   -- Models/ (e.g. FormMode enum)
|-- Features/
|	-- {ModuleName}/
|		-- Common/
|			-- Services/ (e.g. {ModuleName}HttpClient.cs)
|			-- Models/ (e.g. {FeatureName1}Model.cs)
|		-- Components/
|			-- {FeatureName1}Form.razor
|			-- {FeatureName1}Model.cs
|			-- {FeatureName1}Validator.cs
|		-- {FeatureName1}/
|			-- {FeatureName1}Page.razor
|			-- {FeatureName1}Page.razor.cs
|			-- {FeatureName1}Dto.cs (if needed for API interactions)
|		-- {FeatureName2}/
|			-- ...