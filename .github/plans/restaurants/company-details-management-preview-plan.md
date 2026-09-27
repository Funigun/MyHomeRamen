# Company details management

## GetDetails

Module: Restaurants
Aggregate: Company

Domain logic:
Domain events:

Route:
Authorization logic: Requires `CompanyView` permission; authorization failures return `403`. Use `RestaurantsPermissionConstants`.
Validation logic: Validation-policy failures return `400`.
Handler logic: Return `GetCompanyDetailsResponse` containing `Id`, `Name`, `Description`, `LogoUrl`, `BusinessDetails` (`LegalName`, `TaxId`), and `AllowedActions` (`Dictionary<string, bool>`). Include one UI action with key `CanEditCompanyDetails`, mapped from `CompanyEdit`. Application currently assumes one company.

## UpdateDetails

Module: Restaurants
Aggregate: Company

Domain logic: Apply description, logo URL, and business-details changes immediately. Logo remains a string value; storage/service integration is out of scope for now.
Domain events:

Route:
Authorization logic: Requires both `CompanyView` and `CompanyEdit` permissions; authorization failures return `403`. Current permission source is `ICurrentUser.Permissions`.
Validation logic: Validation-policy failures return `400`.
Handler logic: Update `Description`, `LogoUrl`, and `BusinessDetails` from `BusinessDetailsForUpdateDto` (`LegalName`, `TaxId`), persist changes, and return `204`. Future multi-restaurant assignment is out of scope.
