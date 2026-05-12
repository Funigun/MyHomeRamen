# Endpoint Pipeline Refactor

## Purpose
Refactor backend Request and Response objects to eliminate architecture test problems
Refactor IValidationFilter and IAuthorizationFilter to be CommandHandler decorators instead of filters so they can execute after extracting ID from URL and avoid architecture test problems

## Current problems
- endpoint handler use Request objects directly which might be 1:1 with blazor contracts which lead to some problems:
  - PUT endpoints require ID from URL in the object, but filters execute before reaching api handler so we need to extract that from HttpContextAccessor
  - backend object attach ID with custom attribute to pass architecture tests which is explicitly readable, but not ideal

## Solution
- introduce Command / Query objects that will wrap Request objects and be used in api handlers instead of Request objects directly
- move Request and Response objects into MyHomeRamen.Common.Contracts so they can be used by both backend and frontend so architecture tests wont be needed for this case
- update architecture tests to check Request / Response / Command / Query naming conventions
- introduce CommandHandler decorators for validation and authorization so filters will be removed