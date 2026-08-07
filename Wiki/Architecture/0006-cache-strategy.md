---
title: "ADR-0006: Cache Strategy"
status: "Accepted"
date: "2026-07-29"
authors: "Funigun"
tags: ["architecture", "cache", "repository", "infrastructure"]
---

### Status: Accepted

### Context: 

We need a robust caching strategy for our modular monolith application. Initially, we evaluated implementing caching exclusively via our custom MediatR pipeline behaviors (ICachePolicy / CacheInvalidationPolicy). 
However, limitations regarding non-MediatR entry points, background execution, and data-access granularity led us to pivot to a repository-level approach.

### Decision
We will implement a Module-Scoped Caching Strategy via Repository Decorators (ICacheableRepository) combined with Tag-Based Cache Invalidation.
Caching logic will reside at the data access layer, keeping the business and application layers entirely free of caching infrastructure concerns.

## 1. Cacheable Repository
Caching is implemented via the Decorator Pattern wrapping our module-scoped repositories (e.g., Menu.CachedUserRepository wrapping Menu.UserRepository).

Repositories project and cache DTOs or query projections, avoiding EF Core change-tracking traps, serialization issues, and circular references associated with caching raw entities.

Repositories remain strictly scoped to their respective bounded contexts (modules), preventing cross-module database/cache leakage.

## 2. Cache Invalidation
We use a Tag-Based Caching mechanism where query/paged results are tagged with the IDs of the underlying entities they depend on (e.g., product-1, product-2).

Tracked Entities: An EF Core SaveChanges interceptor automatically inspects ChangeTracker.Entries<BaseEntity>(), extracts modified/deleted entity IDs, and triggers corresponding cache tag evictions.

Bulk Operations: Because ExecuteUpdate and ExecuteDelete bypass the EF Core change tracker, repository methods executing these operations must explicitly invoke manual tag evictions.

## 3. Cross-Module Invalidation
Modules maintain strict boundaries. When a mutation occurs in one module that affects data cached in another, the source module publishes an Integration Event (via our message bus / modular event dispatcher).

The consuming module listens to the integration event and triggers local cache tag evictions within its own isolated repository/caching layer.

## 4. Background Worker Invalidations
Background workers (IHostedService) and alternative entry points (such as gRPC or real-time hubs) operate outside the MediatR pipeline.

Background workers use the same module-scoped repositories or publish integration events, ensuring that cache invalidation rules are identically enforced regardless of whether the action originated from an API request or a background process.

### Reasoning
Universal Scope: Centralized cache management at the data access layer allows us to utilize caching across all execution paths—API endpoints, background workers, supporting services, and alternative communication protocols.

Separation of Concerns: Business logic and MediatR handlers remain completely clean of caching details (like cache keys, TTLs, and serialization), focusing solely on business rules.

Granular Control & Performance: Solves both fine-grained data fetching bottlenecks and coarse-grained collection/paged-result caching needs cleanly.

### Considered Alternative: Custom MediatR Pipeline Behaviors
We considered using a pipeline behavior pattern consisting of:

ICachePolicy for GET requests (intercepting queries and caching responses).

CacheInvalidationPolicy for mutation requests (evicting specific cache keys upon command execution).

### Reason for Alternative
Caching was cleanly treated as a cross-cutting concern managed declaratively via attributes or pipeline interfaces.

It provided a centralized place to inspect what requests are cached and how mutations trigger invalidations.

### Rejection Reason
Incomplete Coverage: It fails to cover scenarios outside the MediatR pipeline (such as background workers, gRPC, or direct service-to-service calls within supporting services).

Granularity Limits: Coarse-grained request/response caching makes it difficult to manage complex query assembly or leverage tag-based cache strategies for partial/paged data structures.