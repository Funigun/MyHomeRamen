# Restaurant-aware admin UI workflows

## Status

**Deferred.** Not part of the current refactor. Backend work in [`Domain Model Refactor Plan`](../Domain%20Model%20Refactor%20Plan.md) comes first. Revisit and rewrite this document from real endpoints once that backend exists.

## What stays true now

- The UI behaves as one Company with several restaurants.
- Staff: `RestaurantsView` shows restaurants from the permission-filtered `GetAvailableRestaurantsQuery` (Company owners see all; restaurant managers see their own). A row opens `EditRestaurantPage`. The restaurant ID in the route is the context; the API re-validates it on every request.
- Customers: a public landing page lists active restaurants from a separate anonymous endpoint. The chosen restaurant ID is attached to the basket and sent with requests. Do not reuse the staff query for customers.
- UI selection is never proof of access. Authorization is server-side only.
- No Company selector.

## Deferred topics

- Admin pages for offers, extra-ingredient prices, and MenuCards in restaurant context.
- Company selector and multi-Company administration.
- Capability-specific restaurant selection contracts (for example restaurants eligible for Menu management). The name `GetManageableRestaurants` is not used because it does not say which capability, Company, or actions it covers.
- Stale-route, permission-change, and inactive-restaurant behavior for admin routes.
- Custom ramen builder UI.

## Related

- [ADR-0004: Company and Restaurant Scoped Roles and Authorization](../Users%20ADR/0004-company-restaurant-roles-and-authorization.md)
- [Backend authorization](backend-authorization.md)
- [Domain Model Analysis](../Domain%20Model%20Analysis.md)
