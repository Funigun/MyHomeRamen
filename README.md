# MyHomeRamen

## 1. Introduction

MyHomeRamen is an application for Ramen restaurant management following Modular Monolith with some Microservices concepts.

Originally it was meant to be mainly a learning project for `Blazor dla backendowca` course from `szkoladotneta.pl`.
At some point it evolved to more complext idea where I want to learn how to design modular monolith and handle challenges that occur within microservices (data consistency, cache, communication).

Currently there are following modules:

a) **Basket**: Handles collecting products that user wants to order based on the `Menu` module. It also handles discounts (percentage, fixed amount, BOGO, free shipping) and acts as the source of truth for the shopping cart state.
b) **Menu**: Acts as the single source of truth for the product domain, handling product catalog management including categories, ingredients, and pricing.
c) **Orders**: Manages the lifecycle of customer orders based on the `Basket` module. It ensures data consistency with `Menu` module and acts as input for the `Payments` module.
d) **Payments**: Handles payment provider integration and payment processing. It notifies the `Orders` module about payment status changes.
e) **Reservations**: Handles user bookings for tables and time slots. It is a fully independent module.
f) **Users**: Acts as the single source of truth for the user domain, serving other modules. It handles authentication (via a separate Identity API), user profiles, roles, and permissions, using RabbitMQ for data synchronization.

### Skills to learn
- Modular monolith design (including database)
- Handle microservices challenges (data consistency, communication, roll back chained operation)
- Use RabbitMQ (synchronize user data)
- Use Redis in efficient way
- Background workers implementation for different scenarios
- Server Sent Events implementation (Kitchen dashboard with new orders)
- SignalR - handle order status view

## 2. Technology Stack

- **Platform**: .NET 10
- **Orchestration**: .NET Aspire
- **Architecture**: Modular Monolith, Vertical Slice Architecture, Clean Architecture
- **Frontend**: Blazor Server & Blazor WebAssembly
- **Database**: Entity Framework Core (PostgreSQL)
- **Messaging**: RabbitMQ (MassTransit)
- **Caching**: Redis
- **Scheduling**: Quartz.NET
- **Real-time**: Server Sent Events, SignalR
- **Code Analysis**: StyleCop, SonarAnalyzer

## 3. Getting Started

**Prerequisites:**
- **Docker Desktop**: Required to run the infrastructure containers (Redis, RabbitMQ).
- .NET 10 SDK or later.

**How to run:**
1. Clone the repository.
2. Open the solution in Visual Studio or your preferred IDE.
3. Set the **Aspire** host project (`MyHomeRamen.AppHost`) as the startup project.
4. Run the application. .NET Aspire will automatically provision and configure the local development environment (Redis, RabbitMQ) and launch the services.

## 4. Architecture Overview

The project follows a **Modular Monolith** architecture pattern with **Vertical Slice** architecture principles.

### Solution Structure
- **Aspire Orchestration**: Setups Redis, RabbitMQ, and databases alongside API, Blazor, and Worker projects.
- **Backend API**:
    - **Core Api**: Main API project exposing REST endpoints, split into modules (Basket, Menu, Orders, etc.).
    - **Identity**: Separate ASP.NET Core Identity implementation for user management.
- **Workers**:
    - **Worker.MailSender**: Background worker for email sending.
    - **Worker.MessagesHandler**: Background worker for RabbitMq messaging handling.
- **Blazor Frontend**: Blazor Server and Client (WASM) projects.

Each module in the backend (Basket, Menu, Orders, etc.) is organized according to DDD principles, containing its own API, Domain, Persistence, and Infrastructure layers to ensure separation of concerns.

## 5. Testing Strategy

The project employs a comprehensive testing strategy:

- **Unit Tests**: Focus on testing individual components in isolation using mocks for dependencies.
- **Integration Tests**: Test the interaction between multiple components, the database, and Redis instances using **Testcontainers**.
- **Architecture Tests**: Enforce architectural rules and module boundaries using **NetArchRules**, ensuring the modular monolith structure remains non-coupled.
