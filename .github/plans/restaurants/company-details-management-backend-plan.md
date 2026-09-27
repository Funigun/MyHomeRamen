# Plan: Restaurants - Company details management

## 1. Problem
Provide company owner view for reading and editing current company details through Restaurants features. Company belongs to Restaurants; application currently assumes one company, and updates apply immediately without concurrency checks or events.

## 2. Files to create / modify
| Action | Module | Aggregate | Feature Name | Endpoint Kind | Route |
|--------|--------|-----------|---------------|---------------|-------|
| Create | Restaurants | Company | GetDetails | Query | `GET /api/restaurants/company/details` |
| Create | Restaurants | Company | UpdateDetails | Command | `PUT /api/restaurants/company/details` |
| Modify | Restaurants | Company | CompanyDetailsPermissions | Command | — |

## 3. Domain changes
- Restaurants `Company` remains aggregate owner.
- Add no domain state changes, events, concurrency checks, or logo storage behavior.
- Expose company display data and business details.
- Define UI action key `CanEditCompanyDetails`; map it from current-user `CompanyEdit` permission.
- Use `RestaurantsPermissionConstants.CompanyView` and `RestaurantsPermissionConstants.CompanyEdit`.
- Migration required: no.

## 4. Persistence extensions
- Use existing Company repository/query/specification support in Restaurants persistence.
- Add read projection for company ID, name, description, logo URL, legal name, and tax ID.
- Resolve current Company using current single-company application context.
- Load current Company for update, apply aggregate changes, and save through Restaurants unit of work.

## 5. API details
**GetDetails**
- Aggregate: Company
- Kind: Query
- Method and route: `GET /api/restaurants/company/details`
- Request: no request body; current company context.
- Response: `GetCompanyDetailsResponse(Id, Name, Description, LogoUrl, BusinessDetails(LegalName, TaxId), AllowedActions)`.
- `AllowedActions`: one key, `CanEditCompanyDetails`; value is true when current user has `CompanyEdit`.
- Authorization: require `RestaurantsPermissionConstants.CompanyView`; return `403` when missing.
- Validation: return `400` for invalid or unavailable company context according to validation policy.
- Handler: load current Company and map response.
- Endpoint: Restaurants vertical slice using standard GET endpoint configuration.

**UpdateDetails**
- Aggregate: Company
- Kind: Command
- Method and route: `PUT /api/restaurants/company/details`
- Request: `UpdateCompanyDetailsRequest` containing description, logo URL, and `BusinessDetailsForUpdateDto(LegalName, TaxId)`.
- Response: `204 No Content`.
- Authorization: require both `RestaurantsPermissionConstants.CompanyView` and `RestaurantsPermissionConstants.CompanyEdit`; return `403` when either is missing.
- Validation: validate request fields and current company context; return `400` for validation failures.
- Handler: load current Company, update description, logo URL, and business details, save immediately, and return no content.
- Endpoint: Restaurants vertical slice using standard PUT endpoint configuration.

## 6. Tests
- Unit tests for `CanEditCompanyDetails` mapping with and without `CompanyEdit`.
- Unit tests for response mapping with populated and empty optional company fields.
- Unit tests for update mapping of description, logo URL, legal name, and tax ID.
- Integration test successful GET with `CompanyView`.
- Integration test successful PUT with `CompanyView` and `CompanyEdit`, returning `204`.
- Integration tests verify updated details through GET and immediate persistence.
- Integration tests return `403` when required permissions are missing.
- Integration tests return `400` for invalid request data or unavailable company context.

## 7. Risks / decisions for human approval
- No concurrency protection is planned; last successful update wins.
- Logo URL remains a string until storage handling is introduced.

## 8. Out of scope
- Multiple-company or multiple-restaurant user assignment.
- Logo file or external storage service.
- Company name editing.
- Social-media editing.
- Domain or integration events.
- Company deletion, archival, or restaurant provisioning.
