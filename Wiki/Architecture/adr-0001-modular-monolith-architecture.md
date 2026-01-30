---
title: "ADR-0001: Modular Monolith Architecture with Microservices Transition Strategy"
status: "Accepted"
date: "2025-01-30"
authors: "Funigun"
tags: ["architecture", "decision", "structure"]
---

### Status

**Accepted**

### Context

The MyHomeRamen project requires a robust architecture to manage a complete Ramen restaurant system. The system needs to support multiple domains (Menu, Orders, Basket, Reservations, Payments, Identity) while maintaining high velocity in development.

Starting directly with a Microservices architecture introduces significant operational complexity (distributed transactions, eventual consistency, deployment overhead) which is premature for the current stage of the project. 
However, the business anticipates potential future growth that might require scaling specific components independently.

The challenge is to choose an architecture that provides the benefits of clean separation of concerns without the immediate overhead of distributed systems, while keeping the door open for future decomposability.

### Decision

We will adopt a **Modular Monolith** architecture.

Key aspects of this decision:
1.  **Logical Isolation**: The application will be structured into distinct modules (e.g., Menu, Orders, Identity) defined by boundaries in the code, enforcing strict separation of concerns and verified through architecture tests.
2.  **Shared Host**: Disparate modules will be hosted within a single executable (API) initially.
3.  **In-Process Communication**: Communication between modules will primarily occur in-process, avoiding network latency and serialization overhead found in microservices, but will be designed via public interfaces/contracts to prevent tight coupling.
4.  **Vertical Slice Architecture**: Inside each module, we will follow Vertical Slice Architecture principles to group code by feature rather than technical layer.
5.  **Clean Architecture**: We will apply Clean Architecture principles to define layers (Domain, Persistance, API) so other parts of the system (e.g. workers) can reuse domain and data access logic without duplicating code.
6.  **Asynchronous Communication**: Regardless of modular monolith architecture we still will use some asynchronous communication e.g. RabbitMQ to make consistent user data across modules, SignalR for real-time orders status updates or SSE for kitchen dashboard updates with new orders.

This architecture serves as a stepping stone. By enforcing module boundaries now, we ensure that extracting a module into a standalone microservice in the future is a matter of changing the hosting model and communication transport, rather than a full rewrite ("Transition to Microservices Perspective").

### Consequences

#### Positive

- **POS-001**: **Simplified Operations**: Single deployment unit reduces complexity in CI/CD and monitoring compared to a fleet of microservices.
- **POS-002**: **Development Velocity**: Easier refactoring, debugging, and integration testing since everything is in one solution.
- **POS-003**: **Performance**: Local method calls between modules are faster than network calls; limited network partitions to handle synchronously.
- **POS-004**: **Future Proofing**: Strict boundaries allow for relatively painless extraction of hot modules into microservices if scaling requirements demand it.

#### Negative

- **NEG-001**: **Runtime Coupling**: A crash in one module (e.g., memory leak) brings down the entire application.
- **NEG-002**: **Scaling Limits**: We must scale the entire application, not just the busy modules (though this is mitigated by the ability to extract later).
- **NEG-003**: **Build Times**: As the codebase grows, build and test times for the monolith may increase compared to small individual services.

### Alternatives Considered

#### Traditional Layered Monolith

- **ALT-001**: **Description**: Organize code by technical layer (Controllers, Services, Repositories) horizontally across the entire application.
- **ALT-001**: **Rejection Reason**: Leads to "Big Ball of Mud". High coupling between business features makes refactoring difficult and extraction to microservices nearly impossible without rewrite.

#### Microservices First

- **ALT-002**: **Description**: Build each domain capability (Menu, Orders, etc.) as an independent deployed service from day one.
- **ALT-002**: **Rejection Reason**: Premature optimization. Introduces high infrastructure and distributed system complexity (observability, consistency, latency) that slows down initial feature development.
