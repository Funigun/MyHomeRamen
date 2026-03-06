---
description : Guidelines for backend code quality
applyTo: '*.cs'
---

# Backend Quality Instructions

## Overview
Backend quality ensures code maintainability, performance, and security across API, domain, persistence, etc.
This file covers general guidelines which will apply also to other projects like Persistance, Domain or even Blazor

## Guidelines
- Follow coding standards from .editorconfig, StyleCop, SonarAnalyzer.
- Use async/await for I/O operations.
- Review for code smells and refactor.
- Single file should not exceed 250 lines of code (excluding usings and namespaces).
- Never use `var` for any type
- Use latest C# syntax and features where appropriate.

## Tools
- .editorconfig
- StyleCop.Analyzers
- SonarAnalyzer.CSharp

## Nuget packages management
Project follows central package management approach:
- `Directory.Packages.props` defines all package versions centrally
- `Directory.Build.props` defines which packages are used in which projects