# Performance Instructions

## Overview
Performance guidelines for optimizing the application.

## Guidelines
- Profile with tools like dotTrace or built-in.
- Use caching (Redis) for frequent data.
- Optimize DB queries with includes and filters.
- Minimize allocations in hot paths.
- Monitor with Application Insights.

## Patterns
- Async operations.
- Background processing for heavy tasks.
- Pagination for large data.

## Metrics
- Response times <500ms.
- Memory usage monitoring.