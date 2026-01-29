---
agent: agent
model: Gemini 3 Pro (Preview)
description: Create a domain model id
---

Your task is to create domain DbContext for a specific module in the MyHomeRamen solution.
Following steps should be taken:

1. Define `I{Module}DbContext` interface in `MyHomeRamen.Domain` project
- interface should be placed under {Module}/Database folder

2. Define `DBSets`
- each class that implements IEntity in the module should have a corresponding DbSet in the I{Module}DbContext interface

3. Prepare Persistence layer (MyHomeRamen.Persistance project)
- create {Module} folder
- create {Module}/Configurations folder
- create {Module}/Converters folder

4. Create `Converters`
- each class that implmenets `IEntityId` interface should have corresponding converter
- converters should be placed under {Module}/Converters folder
- converters should inherit from `ValueConverter<TId, Guid>` class

5. Create `Configurations`
- each class that implements IEntity in the module should have a corresponding configuration class
- configurations should be placed under {Module}/Configurations folder
- configurations should inherit from `IEntityTypeConfiguration<TEntity>` interface
- configure primary key using the appropriate Id property
- do not configure properties and relationships at this point

6. Implement `DbContext`
- create {Module}DbContext class under {Module} folder
- {Module}DbContext should implement `I{Module}DbContext` interface
- configure `HasDefaultSchema` to `{module}` and primary key using the appropriate Id property
- override `OnModelCreating` method to apply configurations
- override `ConfigureConventions` method to add converters

