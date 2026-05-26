# Ideas

List of "To do" items that are on conceptual stage / refactor ideas etc that do not have high priority.

## Scaffolding scripts

- `domain-scaffold.ps1` - script to create initial layout of domain model including:
	- strongly typed ID
	- class that inherits from `AuditableEntity`
	- Domain Validator skeleton
	- BaseDbContext configuration skeleton (if new module), update to existing module not covered by scaffold
	- EF Core strongly-typed ID converter skeleton
	- Unit Test for domain validation skeleton

- `blazor-slice-scaffold.ps1` - script to scaffold a new Blazor slice
	- figure out what can be standarized in terms of scaffolding

- blazor - refactor MenuApiClient etc. to make them more readable e.g. partial class with split by concern e.g.
	- `MenuApiClient.Products.cs` for all product related API calls, 
	- `MenuApiClient.Categories.cs` for all category related API calls, etc.

