# Feature Frontend Plan — GetDetails

- **Date**: 2025-07-17
- **Feature**: GetDetails — Blazor user profile page

---

## 11) Create frontend feature structure

```
MyHomeRamen.Blazor.Client/
└── Features/
    └── Account/
        └── UserDetails/
            ├── UserDetailsPage.razor
            └── UserDetailsPage.razor.cs
```

## 12) Create or update API communication services and API Response model

- Create `AccountApiClient` in `MyHomeRamen.Blazor.Client` (pattern: `MenuApiClient`)
- Method: `GetDetailsAsync()` returning `GetDetailsResponse`
- Register in DI

## 13) Create or update models, DTOs and mappings

- `GetDetailsResponse` record: `Username`, `FirstName`, `LastName`, `Email`
- Shared with backend or defined client-side

## 14) Create or update Blazor components and pages

- `UserDetailsPage` — route `/users/me`, authenticated only
- Calls `AccountApiClient.GetDetailsAsync()` on init
- Displays username, first name, last name, email in read-only card layout
- Loading indicator while request in flight
- Error message on failure

## 15) Create unit tests for Blazor components and services

- Not in scope for this iteration (per brief)
