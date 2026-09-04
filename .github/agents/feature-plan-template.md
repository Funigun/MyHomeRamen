# Plan: {Module} - {feature title}

## 1. Problem
<What user wants, why, what already exists — 2-3 sentences max>

## 2. Files to create / modify

| Action | Module | Aggregate | Feature Name | Endpoint Kind | Route |
|--------|--------|-----------|--------------|---------------|-------|
| create | Identity | Employee | RegisterEmployee | Command | api/identity/employees/register |
| create | Identity | Employee | GetEmployees | Query | api/identity/employees |
| create | Identity | Employee | GetById | Query | api/identity/employees/{id} |
| create | Identity | Employee | UpdateEmployeeRoles | Command | api/identity/employees/{id} |


Valid `Action`: Create, Modify, Delete 
Valid `Module`: Identity, Menu, Orders, ShoppingCart, Reservations, Payments, Restaurants
Valid `Aggregate`: Required Aggregate name, does not have to match domain model
Valid `Endpoint Kind`: Command, Query

## 2.1 Constructors

public sealed record RegisterEmployeeRequest(string FirstName, string LastName, string Email, string Password, List<int> Roles)
public sealed record RegisterEmployeeCommand(RegisterEmployeeRequest request)
public sealed record GetEmployeesRequest(int PageNumber, int PageSize, string? SearchTerm)
public sealed record GetEmployeesQuery(GetEmployeesRequest request)
public sealed record GetEmployeesResponse(int PageNumber, int PageSize, int TotalCount, List<EmployeeListDto> Employees)
public sealed record EmployeeListDto(int Id, string FirstName, string LastName, string Email, List<string> Roles)
public sealed record GetByIdRequest(int Id)
public sealed record GetByIdResponse(int Id, string FirstName, string LastName, string Email, List<string> Roles)
public sealed record UpdateEmployeeRolesRequest(int Id, List<int> Roles)

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