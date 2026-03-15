---
description : Guidelines for frontend code quality
applyTo: '*.razor, *.razor.cs'
---

# Frontend Quality Instructions

## Overview
Frontend quality ensures usability, performance, and maintainability across the Blazor Server and WASM projects.
This file covers general guidelines which apply to the UI projects.

## Guidelines
- Follow coding standards from .editorconfig, StyleCop, SonarAnalyzer.
- Never use `var` for any type.
- Optimize performance by deciding correctly between C# components and HTML. If a UI element is purely presentational and does not require C# interactivity, implement it using standard HTML/CSS instead of heavy Blazor components to reduce render tree size and improve user experience.
- Use the `[PersistentState]` attribute to serialize state during server prerendering and safely deserialize it on the client, avoiding double API calls and flickering.
- Validate forms consistently using `FluentValidation`. All validators should be composed of primitive type validators available in the `MyHomeRamen.Common.Contracts` project.
- Follow Blazor best practices and use ARIA for accessibility.
- Map models and DTOs manually. Never use AutoMapper, Mapster, or similar automated mapping libraries.
  - UI Models expose `ToXxxRequest()` methods to convert to API DTOs (e.g., `model.ToCreateRequest()`).
  - For reverse mapping (API response → UI Model), use static factory methods or extension methods.

## Tools
- **MudBlazor**: Use as the primary UI component library for building the application.
- bUnit for unit testing components.
- Lighthouse scores for performance.
- Accessibility audits.

## Nuget packages management
Project follows central package management approach:
- `Directory.Packages.props` defines all package versions centrally
- `Directory.Build.props` defines which packages are used in which projects

## Component File Structure

Organize every `.razor` file in the following order:

1. `@page` directive (if the component is a routable page)
2. `@using` statements
3. `@inject` directives
4. HTML / Razor markup section
5. `@code { }` block