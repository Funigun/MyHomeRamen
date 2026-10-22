# Restaurant and company configuration

Configuration currently has two overlapping sources. AppHost settings still
identify the deployment and its infrastructure. The Restaurants module is
being refactored to store company and restaurant business data in the
application database.

## Deployment configuration

`RestaurantConfiguration:InfrastructurePrefix` is deployment-level
configuration used by Aspire to name resources and services, such as API,
Redis, RabbitMQ, and Keycloak. Keep infrastructure names, connection
information, and service endpoints in deployment configuration; they are
needed before application data can be queried.

`RestaurantConfiguration:Name` is currently passed through AppHost and used
by application configuration providers. The API uses it for the Scalar API
title. Blazor also reads display and home-page values from
`RestaurantConfiguration`, including `Tagline`, `Description`,
`EstablishedYear`, `Location`, `SeasonLabel`, and `Copyright`.

## Company data

The current `Company` aggregate stores `Name`, `Description`, `LogoUrl`, and
`BusinessDetails` (`LegalName` and `TaxId`). The in-progress company setup
workflow creates the initial Company and Restaurant. These persisted fields
overlap with some values still supplied through configuration, but they do not
yet replace the Blazor home-page settings.

Company data and deployment configuration have different lifecycles:

- Deployment settings choose and connect infrastructure.
- Company data describes the business and can be managed through application
  workflows.
- Home-page content currently remains in Blazor configuration; it is not yet
  loaded from the Company API.

## Open design decision: public and legal content

Copyright text, terms of service, and policies are not currently Company
fields. Decide whether they are application-wide defaults or editable,
company-specific content before adding persistence or changing the home page.

If content must vary by Company and be editable at runtime, database-backed
Company content and a public read API are a natural fit. Terms and policies
need more than a mutable text field when users must accept them: preserve
versions, publication/effective dates, locales where needed, and acceptance
records. Copyright text may be a simple Company setting or derived from Company
name and year. This remains a design option, not an accepted decision.

The home page will need to read public Company content from an API if it moves
from static configuration to database-managed content. Do not move
infrastructure settings into Company data as part of that refactor.

## Related configuration

- [Database configuration](0002-database-configuration.md)
- [Keycloak configuration](0003-keycloak-configuration.md)
- [Centralized configuration ADR](../Architecture/0004-restaurant-configuration.md)
