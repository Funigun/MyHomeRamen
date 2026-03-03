# Blazor Instructions

## Overview
The Blazor frontend (`MyHomeRamen.Blazor` for Server, `MyHomeRamen.Blazor.Client` for WASM) provides the UI for restaurant management.

## Architecture Assumptions
- Server-side for initial load, WASM for client-side interactivity.
- Use Fluxor or similar for state management.
- Integrate with API via HttpClient.
- Handle authentication via `AuthHeaderHandler`.

## Guidelines
- Follow component-based architecture.
- Use Blazor's routing and forms.
- Ensure responsive design with Bootstrap or similar.
- Implement role-based UI rendering.
- Optimize for performance in WASM.

## Patterns
- Vertical slices for features (e.g., user management UI).
- Shared components in common libraries.