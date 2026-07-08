## MODIFIED Requirements

### Requirement: Raw SQL in handlers (no LINQ query translation)
The system SHALL use raw SQL for fact persistence in CQRS handlers for AOT compatibility (no EF Core
runtime LINQ query translation). SQL SHALL be executed through EF Core's AOT-safe source-generated
compiled queries (`EF.CompileAsyncQuery` with `FromSql` for reads, `ExecuteSql`/`ExecuteSqlAsync` for
writes) rather than direct `Microsoft.Data.Sqlite` `SqliteCommand` usage.

#### Scenario: Handlers use EF compiled queries
- **WHEN** GetFactsHandler, SearchFactsHandler, GetFactByIdHandler, UpsertFactHandler, UpdateFactHandler, DeleteFactHandler execute
- **THEN** they execute raw SQL via `EF.CompileAsyncQuery`/`FromSql`/`ExecuteSql` (no runtime LINQ translation, no `SqliteCommand`)

## ADDED Requirements

### Requirement: Schema created via EF on startup
The system SHALL create the MemoryFacts table and indexes on startup using inline DDL executed through
EF Core (`db.Database.ExecuteSqlRawAsync`), with no EF migrations.

#### Scenario: Startup applies DDL via EF
- **WHEN** the application starts
- **THEN** the MemoryFacts table and unique (Category, Key, Scope) index are created if not present via `ExecuteSqlRawAsync`
