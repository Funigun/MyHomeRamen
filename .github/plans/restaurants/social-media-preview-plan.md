# Restaurants - Social Media Features

Route base: `.../company/{id}/social-media`

## CreateSocialMedia

Module: Restaurants
Aggregate: Company

Domain logic: Create Company-owned SocialMedia with `Name`, `LogoUrl`, and `Url`.
Domain events:

Route: `.../company/{id}/social-media`

Authorization logic: `PermissionConstants.CompanySocialMediaEdit`
Validation logic: Validate `Name`, `LogoUrl`, and `Url` using existing SocialMedia rules.
Handler logic: Return empty `201 Created`; no response body, ID, or Location header.

## GetSocialMediaForManage

Module: Restaurants
Aggregate: Company

Domain logic:
Domain events:

Route: `.../company/{id}/social-media/manage`

Authorization logic: `PermissionConstants.CompanySocialMediaView`
Validation logic: Validate Company ID.
Handler logic: Return management response models including SocialMedia ID, `Name`, `LogoUrl`, and `Url`; return `200`.

## UpdateSocialMedia

Module: Restaurants
Aggregate: Company

Domain logic: Update Company-owned SocialMedia by ID with `Name`, `LogoUrl`, and `Url`.
Domain events:

Route: `.../company/{id}/social-media/{socialMediaId}`

Authorization logic: `PermissionConstants.CompanySocialMediaEdit`
Validation logic: Validate Company ID, SocialMedia ID, and all SocialMedia fields.
Handler logic: Update existing SocialMedia and return `204`; no concurrency handling.

## DeleteSocialMedia

Module: Restaurants
Aggregate: Company

Domain logic: Remove Company-owned SocialMedia by ID.
Domain events:

Route: `.../company/{id}/social-media/{socialMediaId}`

Authorization logic: `PermissionConstants.CompanySocialMediaEdit`
Validation logic: Validate Company ID and SocialMedia ID.
Handler logic: Delete existing SocialMedia and return `204`.

## GetAvailableSocialMedia

Module: Restaurants
Aggregate: Company

Domain logic: Return all Company SocialMedia entries without exposing IDs.
Domain events:

Route: `.../company/{id}/social-media/available`

Authorization logic: `AllowAnonymous`
Validation logic: Validate Company ID.
Handler logic: Return separate response models containing only `Name`, `LogoUrl`, and `Url`; return `200`.
