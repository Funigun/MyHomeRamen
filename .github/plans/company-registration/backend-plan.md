# Plan: Users - Company owner registration

## 1. Problem
Add synchronous company registration through an Identity endpoint. The flow must validate user data, register the user through the existing Keycloak/customer process, verify globally unique normalized company name before local user persistence, create the Company and owner membership atomically, and return the Company ID.

## 2. Files to create / modify
| Action | Module | Aggregate | Feature Name | Endpoint Kind | Route |
|--------|--------|-----------|---------------|---------------|-------|
| Create | Users | CompanyMembership | RegisterCompanyOwner | Command | `POST /api/identity/companies/register-owner` |
| Create | Users | Company | CheckCompanyNameAvailability | Command | Internal application contract |
| Modify | Users | Company | RenameCompanyDetailsToCompany | Command | — |
| Modify | Users | Company | CompanyNameNormalization | Command | — |
| Modify | Users | User, CompanyMembership, Role | CompanyOwnerAssignment | Command | — |

## 3. Domain changes
- Rename `CompanyDetails` aggregate root to `Company` and `CompanyDetailsId` to `CompanyId`.
- Preserve company display data and `BusinessDetails`; add normalized company-name ownership needed for global uniqueness.
- Normalize company name case-insensitively by retaining letters and numbers only.
- Keep original company name for display.
- Add Company factory/update validation and global normalized-name uniqueness enforcement.
- Add synchronous registration orchestration in Identity using internal `UserId`.
- Reuse existing customer/Keycloak registration behavior; do not introduce a second user identity model.
- Create Company, CompanyMembership, and CompanyOwner assignment in one local transaction.
- Require idempotency and return the original Company ID for repeated successful requests.
- Do not add Company lifecycle, archive, delete, restaurant creation, or restaurant provisioning behavior.
- Migration needed: yes. Rename the CompanyDetails table/key model as required, add normalized-name storage and unique index, and preserve existing data.

## 4. Persistence extensions
- Add Company repository/query/specification support for normalized-name lookup and Company creation.
- Add a uniqueness specification/query that compares normalized names without loading unrelated Companies.
- Add unique database index for normalized company name; map duplicate-key failure to conflict behavior.
- Add/update Company configuration, strongly typed ID conversion, owned `BusinessDetails`, and social-media persistence.
- Add Identity persistence changes for the registration transaction, CompanyMembership uniqueness, owner assignment, and idempotency records if not already available.
- Ensure the local transaction covers internal User save, Company creation, membership creation, and CompanyOwner assignment.
- Use an internal cross-module application contract; Identity must not reference Restaurants domain entities, DbContext, or persistence types.

## 5. API details
**RegisterCompanyOwner**
- Aggregate: CompanyMembership and Company
- Kind: Command
- Method and route: `POST /api/identity/companies/register-owner`
- Authorization: authenticated user; reject callers that cannot perform owner registration according to Identity policy.
- Request: registration contract containing existing customer user-registration fields, `CompanyName`, and `Idempotency-Key` header. User fields use the existing customer registration rules.
- Validation order: validate user data, validate company data, complete existing Keycloak/customer registration and obtain internal `UserId`, verify normalized company-name uniqueness, then persist the internal user and company workflow.
- Response: `RegisterCompanyOwnerResponse` containing `CompanyId`.
- Status and error responses: `201 Created`, `400` invalid data, `401` unauthenticated, `403` unauthorized registration, `409` duplicate normalized company name or idempotency conflict, `500` unexpected failure.
- Idempotency/concurrency: `Idempotency-Key` required; same valid retry returns original `CompanyId`; concurrent duplicate names rely on unique database constraint and return `409`.
- Events: emit `CompanyRegistered` after the local transaction commits. Include Company ID and internal User ID; consumers must process idempotently.
- External failure: Keycloak success followed by local failure must surface failure and support reconciliation/retry; never return Company ID before local commit.
- Endpoint handler: coordinate existing user registration, Restaurants company contract, local membership/role assignment, transaction, idempotency record, and response mapping.
- Endpoint: use standard POST endpoint conventions, explicit name/description/tags, authorization policy, and Location/result mapping as supported by project endpoint builders.

**Company name validation support**
- Aggregate: Company
- Kind: Command support
- Contract: internal module boundary used by `RegisterCompanyOwner`.
- Request: original `CompanyName`.
- Response: normalized name and availability/result needed by registration orchestration; do not expose persistence entities.
- Rules: remove all non-letter/non-number characters and compare case-insensitively; empty normalized output is invalid.

## 6. Tests
- Unit tests for Company creation, display/normalized name behavior, invalid empty normalized names, case-insensitive equality, and permitted characters.
- Unit tests for registration validation order, owner assignment, idempotency behavior, and transaction failure outcomes.
- Unit tests for duplicate CompanyMembership/CompanyOwner invariants where Identity owns those rules.
- Integration tests for successful synchronous registration, Keycloak/customer registration integration, returned Company ID, persisted Company and owner membership, duplicate normalized names, concurrent duplicate names, repeated idempotent requests, invalid user data before persistence, and authorization failures.
- Integration tests for migration compatibility with existing CompanyDetails data and normalized-name unique index.
