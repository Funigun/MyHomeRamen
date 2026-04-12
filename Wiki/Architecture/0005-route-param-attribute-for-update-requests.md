---
title: "ADR-0005: RouteParamAttribute for Route-Bound Properties on Update Requests"
status: "Accepted"
date: "2026-06-12"
authors: "Funigun"
tags: ["architecture", "api", "contracts", "testing", "blazor"]
---

### Status

**Accepted**

### Context

The project enforces an architecture rule (`BlazorRequest_ShouldMatch_ApiRequestShape`) that verifies Blazor and API request contracts share identical public properties. This ensures the Blazor frontend stays in sync with the backend API as it evolves.

Update endpoints (PUT/PATCH) require the resource `Id` to be sourced from the URL route (e.g., `PUT /ingredients/{id}`), not the request body. This is standard REST convention and prevents a client from supplying a mismatched `Id` in the body.

The handler, however, operates on a single unified request object (`IRequest<TResponse>`) that must carry the `Id` alongside the body fields. This creates a tension:

- If `Id` is a **public property**, the architecture test fails because the Blazor request (which only models the body) does not have it.
- If `Id` is an **internal property**, the test passes, but the solution is implicit — nothing communicates *why* the property is hidden, and any developer can accidentally expose or misuse it.

The project also has a strict design requirement that endpoint handlers receive a fully-populated request object so that `ValidationFilter<TRequest>` and `AuthorizationFilter<TRequest>` can operate on a complete, consistent request without knowledge of route parameters.

### Decision

Introduce a `[RouteParam]` attribute (`RouteParamAttribute`) in `MyHomeRamen.Api.Common.Endpoint.Models`.

**Rules for use:**

1. Any property on an API request record that is bound from the URL route (not the body) must be annotated with `[RouteParam]`.
2. The property must use `init`-only access (`{ get; init; }`) to preserve record immutability.
3. The endpoint must populate it using a `with` expression: `handler.Handle(request with { Id = id.Id }, ...)`.
4. The `BlazorRequest_ShouldMatch_ApiRequestShape` architecture test excludes `[RouteParam]`-decorated properties from the contract comparison, as they are not part of the body contract shared with the frontend.

**Example:**

```csharp
// API request (body contract + route param)
public sealed record UpdateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds) : IRequest<UpdateIngredientResponse>
{
    [RouteParam]
    public Guid Id { get; init; }
}

// Blazor request (body contract only — no Id)
public sealed record UpdateIngredientRequest(
    string Name,
    string Description,
    decimal Price,
    IEnumerable<Guid> CategoryIds);

// Endpoint — populates Id from route using 'with'
private static async Task<IResult> HandleAsync(
    [FromRoute] UpdateIngredientIRequestId id,
    [FromBody] UpdateIngredientRequest request,
    [FromServices] IRequestHandler<UpdateIngredientRequest, UpdateIngredientResponse> handler,
    CancellationToken cancellationToken)
{
    UpdateIngredientResponse response = await handler.Handle(request with { Id = id.Id }, cancellationToken);
    return Results.Ok(response);
}
```

### Alternatives Considered

#### Option A: `internal Guid Id { get; set; }`
Mark `Id` as `internal` so the architecture test ignores it (only public properties are compared).

**Rejected because:**
- Implicit — no signal to the developer about *why* the property is internal.
- Mutable setter breaks record immutability; `request.Id = id.Id` is a side effect.
- Exploits a side effect of the architecture test rather than expressing a real design rule.

#### Option B: Separate route and body parameters in the handler
Receive `Id` and the body request as separate handler arguments, avoiding the need to merge them.

**Rejected because:**
- `ValidationFilter<TRequest>` and `AuthorizationFilter<TRequest>` are generic over a single `TRequest`. A split model would require reworking both filters and all endpoint filter infrastructure.
- Inconsistent with the existing REPR pattern used across all other features.

#### Option C: Include `Id` in the Blazor request body
Have the Blazor client send `Id` in the body alongside the route.

**Rejected because:**
- Violates REST conventions — the authoritative resource identifier is the URL, not the body.
- Creates ambiguity if route `Id` and body `Id` differ.
- Bloats the Blazor contract with a field the backend ignores from the body.

### Consequences

#### Positive

- **POS-001: Explicit intent.** `[RouteParam]` is self-documenting — any developer reading the request record immediately understands the property originates from the route.
- **POS-002: Immutability preserved.** Using `init` and a `with` expression keeps the record immutable after construction.
- **POS-003: Architecture test remains meaningful.** The test continues to enforce body contract parity between Blazor and the API, with a precise, well-reasoned exclusion for route-bound properties.
- **POS-004: Consistent pattern.** All update endpoints follow the same shape with no special cases.

#### Negative

- **NEG-001: Convention must be followed.** Forgetting `[RouteParam]` on a route-bound property will cause the architecture test to fail, requiring developers to understand the rule. This is mitigated by the ADR and the attribute's XML doc comment.
