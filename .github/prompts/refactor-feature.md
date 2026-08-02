We are refactoring feature slices to new format, 

previous: Common.Contracts contains request+dto, response+dto, validators shared between API and Blazor. 

Scope: Backend feature slices: requests, responses and their DTOs

Out of scope: 
 - validators (already migrater)
 - Blazor (will be migrated later)
 - clearing up Common.Contracts (will be done after all features are migrated)

Refactor steps:

1) Move Request, Response and DTOs
   - target location: Endpoint file e.g. CreateCategoryEndpoint
   - types order: Request specific DTOs, Request, Response specific DTOs, Response, Endpoint
   - No nested classes
   - naming conventions {Feature}RequestDto, {Feature}Request, {Feature}ResponseDto, {Feature}Response
   - Remove namespace Common.Contracts
   - verify if Enpoint returns TypedResult and adjust if needed


2) Merge Command/Query with Handler into single file
	- Keep class names {Feature}Command, {Feature}Query, {Feature}Handler
	- No nested classes
	- File name = Command/Query file name (e.g. CreateUserCommand.cs, GetUserQuery.cs)
	- Remove namespace Common.Contracts

3) Refactor feature:
   - Add missing Request/DTOs (e.g. endpoint has route/query parameters but no Request/DTOs)
   - refactor query handlers as repositories currently return Domain enttities, 
     - refactor to return DTOs (create if missing),
	 - define DbQueryOptions<{Model}, DTO> and update I{Model}Query method signature and implementation  
     - map DTO to Response in handler:
	   - to Response directly if its single DTO
	   - pass IEnumerable<DTO> to Response if its a collection of DTOs
	   - paged results response shape: IEnumerable<DTO> Items, int TotalCount, int PageNumber, int PageSize

4) Update integration tests to use new locations for Request, Response and DTOs