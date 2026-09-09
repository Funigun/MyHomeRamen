# Plan: {Module} - {feature title}

## 1. Problem
<What user wants, why, what already exists — 2-3 sentences max>

## 2. Files to create / modify

| Action | Module | Aggregate | Feature Name | Endpoint Kind | Route |
|--------|--------|-----------|--------------|---------------|-------|
| create | Identity | Employee | RegisterEmployee | Command | Post:api/identity/employees/register |
| create | Identity | Employee | GetEmployees | Query | Get:api/identity/employees |
| create | Identity | Employee | GetById | Query | Get:api/identity/employees/{id} |
| create | Identity | Employee | UpdateEmployeeRoles | Command | Put:api/identity/employees/{id} |


Valid `Action`: Create, Modify, Delete 
Valid `Module`: Identity, Menu, Orders, ShoppingCart, Reservations, Payments, Restaurants
Valid `Aggregate`: Required Aggregate name, does not have to match domain model
Valid `Endpoint Kind`: Command, Query

## 3. Domain changes
- <Implementation details>
- Migration needed: yes / no

## 4. Persistance
- define implementation of I{Aggregate}Repository.Specification.{Method} or I{Aggregate}Repository.Query().{Method} or point to existing methods

## 5. API details
<Events details> (if any)
<Request details>
<Response details>
<Command/Query details>
<Authorization policy details>
<Validator details>
<Endpoint handler details>
<Endpoint details>

## 6. Tests
<Unit tests details>
<Integration tests details>