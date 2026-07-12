Prepare module db access refactor plan according to guidance:

1) Move `I{Module}DbContext}` from domain to features project including updates:
   - implement `IUnitOfWork` instead `BaseDbContext`
   - update `DbInitializerJob` in `MyHomeRamen.Worker.DatabaseInitializer` project to use `IUnitOfWork`
   - remove unused `BeginTransaction, RollbackTransaction, CommitTransaction` methods form `{Module}DbContext` implementation
   - make `{Module}DbContext` partial class

2) Analyze `{Aggregate}DbExtensions` and usage of current `{Module}DbContext.DbSet<Aggregate>` in features and integration tests project and prepare plan to split
   into `I{Aggregate}Repository`, `I{Aggregate}Query`, and `I{Aggregate}Specification` per guidance:

   - Exists / AnyAsync etc - `I{Aggregate}Repository.Exists` - using AsNoTracking()
   - extension / direct query call AsNoTracting -> move to `I{Aggregate}Query`
   - extension / direct query call with tracking -> move to `I{Aggregate}Specification`

   If any method exposes `DbSet<Aggregate>` or `IQueryable<Aggregate>` plan to refactor to return expected state like boolean, int, entity, entities or projected DTOs.

3) Create `I{Aggregate}Repository`, `I{Aggregate}Query`, and `I{Aggregate}Specification` interfaces in the features project
   and create implementations in the persistence project as partial classes of `{Module}DbContext` including methods planned in step 2.


4) Replace DbSets in `I{Module}DbContext` with repositories

5) List validators, handlers and integration tests that require updates

6) List files to cleanup: old interfaces, db exensions and other legacy code that is no longer used
