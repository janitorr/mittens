## MODIFIED Requirements

### Requirement: Raw SQL in handlers (no LINQ query translation)
The system SHALL use raw SQL for fact persistence in CQRS handlers for AOT compatibility (no EF Core
runtime LINQ query translation). SQL SHALL be executed directly through EF Core via `FromSqlRaw` (for
entity reads) and `SqlQueryRaw`/`SqlQueryRaw<int>` (for counts) on the `DbContext`/`DatabaseFacade` —
NOT via `EF.CompileAsyncQuery` + `FromSql`/`FromSqlRaw` on `DbSet<T>` (unsupported in EF Core 10;
throws `ArgumentException` at runtime). No `Microsoft.Data.Sqlite` `SqliteCommand` usage.

#### Scenario: Handlers use EF raw SQL directly
- **WHEN** GetFactsHandler, SearchFactsHandler, GetFactByIdHandler, UpsertFactHandler, UpdateFactHandler, DeleteFactHandler execute
- **THEN** they call `FromSqlRaw`/`SqlQueryRaw` directly (no runtime LINQ translation, no `SqliteCommand`, no `EF.CompileAsyncQuery`)

## ADDED Requirements

### Requirement: AOT correctness verified by integration tests
The system SHALL verify AOT/trimming correctness by running the integration test suite against the
Release build (`dotnet build -c Release`), not only a Debug build, because `dotnet publish -c Release`
does not execute queries and can mask runtime SQL errors.

#### Scenario: Release integration run
- **WHEN** CI builds in the Release configuration
- **THEN** integration tests execute against a real SQLite database and must pass
