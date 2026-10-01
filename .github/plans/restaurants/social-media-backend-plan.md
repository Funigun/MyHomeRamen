# Plan: Restaurants - Company social media

## 1. Problem
Add Company-scoped social-media management and anonymous available-media reads. Existing Company-owned SocialMedia data and field validation must be exposed through separate endpoint contracts without exposing IDs in available responses.

## 2. Files to create / modify
| Action | Module | Aggregate | Feature Name | Endpoint Kind | Route |
|--------|--------|-----------|--------------|---------------|-------|
| Create | Restaurants | Company | CreateSocialMedia | Command | POST: /api/restaurants/company/social-media |
| Create | Restaurants | Company | GetSocialMediaForManage | Query | GET: /api/restaurants/company/social-media/manage |
| Create | Restaurants | Company | UpdateSocialMedia | Command | PUT: /api/restaurants/company/social-media/{socialMediaId} |
| Create | Restaurants | Company | DeleteSocialMedia | Command | DELETE: /api/restaurants/company/social-media/{socialMediaId} |
| Create | Restaurants | Company | GetAvailableSocialMedia | Query | GET: /api/restaurants/company/social-media/available |

Additional modified surfaces: Company social-media aggregate behavior, Company repository/query access, and Restaurants database context support.`r`n`r`n## 3. Domain changes
- Keep Company as aggregate owner; do not create an independent SocialMedia repository.
- Preserve `SocialMedia.Create` validation for required and maximum-length `Name`, `LogoUrl`, and `Url`.
- Add Company-owned SocialMedia update and removal behavior.
- Enforce Company ownership when updating or deleting by `socialMediaId`.
- No availability flag, archive state, concurrency handling, domain events, or integration events.
- Migration required: no.

## 4. Persistence extensions
- Extend Company repository/query/specification support to load Company by route ID and project its SocialMedia collection.
- Add Company-owned SocialMedia lookup by both Company ID and SocialMedia ID for update/delete.
- Persist create, update, and delete through Company aggregate and Restaurants unit of work.
- Add read-only projections for:
  - Manage: `Id`, `Name`, `LogoUrl`, `Url`.
  - Available: `Name`, `LogoUrl`, `Url`.
- Do not expose `DbSet<SocialMedia>` or create a separate SocialMedia repository.

## 5. API details
**CreateSocialMedia**
- Aggregate: Company
- Kind: Command
- Method and route: `POST /api/restaurants/company/social-media`
- Request: separate request containing `Name`, `LogoUrl`, and `Url`.
- Response: empty `201 Created`; no body, ID, or Location header.
- Authorization: require `PermissionConstants.CompanySocialMediaEdit`.
- Validation: validate Company ID and all SocialMedia fields using existing rules.
- Handler: load Company, create SocialMedia through domain factory, attach it, save, and return Created.

**GetSocialMediaForManage**
- Aggregate: Company
- Kind: Query
- Method and route: `GET /api/restaurants/company/social-media/manage`
- Request: N/A
- Response: separate collection response containing `Id`, `Name`, `LogoUrl`, and `Url`.
- Status: `200 OK`.
- Authorization: require `PermissionConstants.CompanySocialMediaView`.
- Validation: validate Company ID.
- Handler: query Company-owned media and map management response.

**UpdateSocialMedia**
- Aggregate: Company
- Kind: Command
- Method and route: `PUT /api/restaurants/company/social-media/{socialMediaId}`
- Request: separate request containing `Name`, `LogoUrl`, and `Url`.
- Response: empty `204 No Content`.
- Authorization: require `PermissionConstants.CompanySocialMediaEdit`.
- Validation: validate both IDs and all SocialMedia fields; reject missing or foreign media.
- Handler: load Company-owned media, update through domain behavior, save, and return No Content.
- Concurrency: no concurrency handling; last successful write wins.

**DeleteSocialMedia**
- Aggregate: Company
- Kind: Command
- Method and route: `DELETE /api/restaurants/company/social-media/{socialMediaId}`
- Request: SocialMedia ID route parameters.
- Response: empty `204 No Content`.
- Authorization: require `PermissionConstants.CompanySocialMediaEdit`.
- Validation: validate both IDs; reject missing or foreign media.
- Handler: load Company-owned media, remove through Company behavior, save, and return No Content.

**GetAvailableSocialMedia**
- Aggregate: Company
- Kind: Query
- Method and route: `GET /api/restaurants/company/social-media/available`
- Request: N/A
- Response: separate collection response containing only `Name`, `LogoUrl`, and `Url`; never expose ID.
- Status: `200 OK`.
- Authorization: `AllowAnonymous`.
- Validation: validate Company ID.
- Handler: query all media for Company and map available response.

## 6. Tests
- Unit tests for SocialMedia creation and update validation of required and maximum-length fields.
- Unit tests for Company attach, update, and remove behavior, including foreign-child rejection.
- Unit tests for manage mapping including ID and available mapping excluding ID.
- Unit tests for command/query request validation and response mappings.
- Integration tests for successful create returning empty `201`.
- Integration tests for manage query returning `200` with IDs.
- Integration tests for update and delete returning empty `204` and persisting changes.
- Integration tests for available query returning `200` anonymously with no IDs.
- Integration tests for invalid requests, missing Company/media, foreign media, and missing permissions returning project-standard errors.
- Integration tests verify `CompanySocialMediaView` and `CompanySocialMediaEdit` permission boundaries.
