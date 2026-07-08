## Why

The project already references EF Core (`AppDbContext` + a compiled model) and registers it via
`AddDbContext`, but every data-access handler opens the EF connection and executes hand-written
`Microsoft.Data.Sqlite` ADO.NET commands (raw inline SQL with manual `SqliteParameter` wiring and
manual `DbDataReader` → `MemoryFact` mapping). This duplicates EF's responsibility, is error-prone
(column/parameter mismatches, string-formatted `DateTimeOffset`), and bypasses EF's change tracking,
compiled queries, and AOT-friendly LINQ. Using EF Core for the actual persistence brings the code in
line with the already-present `DbContext` and removes a class of runtime bugs.

## What Changes

- Replace direct `Microsoft.Data.Sqlite` ADO.NET inline SQL in all 6 handlers with EF Core's
  AOT-safe **source-generated compiled queries** (`EF.CompileAsyncQuery` backed by `FromSql` for reads,
  `ExecuteSql(Async)` for writes). This keeps raw SQL (no runtime LINQ translation) per the AOT mandate
  while removing the manual ADO.NET connection/parameter/reader plumbing:
  - `Application/Queries/GetFacts.cs` — compiled `FromSql` query with parameterized `WHERE` + `LIMIT`/`OFFSET`; count via compiled query.
  - `Application/Queries/GetFactById.cs` — compiled `FromSql` by Id.
  - `Application/Queries/SearchFacts.cs` — compiled `FromSql` with `LIKE` on Key/Value (+ optional filters).
  - `Application/Commands/UpsertFact.cs` — compiled `FromSql` select + `ExecuteSql` insert/update (keep conflict resolution).
  - `Application/Commands/UpdateFact.cs` — compiled `FromSql` select + `ExecuteSql` update.
  - `Application/Commands/DeleteFact.cs` — compiled `FromSql` select + `ExecuteSql` delete.
- Add a `Data/CompiledQueries.cs` static class declaring the `EF.CompileAsyncQuery` definitions.
- Remove `Microsoft.Data.Sqlite` usings and manual `conn.OpenAsync`/`CloseAsync` plumbing from handlers.
- Retain the startup inline DDL in `Program.cs`, executed via `db.Database.ExecuteSqlRawAsync` (no EF
  migrations, per AOT constraint).
- Keep the unique (Category, Key, Scope) constraint and existing paging/page-size clamping semantics.

## Capabilities

### New Capabilities
<!-- none -->

### Modified Capabilities
- `persistence`: Update implementation wording — data access SHALL go through EF Core
  (`DbContext` / LINQ / `SaveChangesAsync`) rather than raw ADO.NET inline SQL, while startup
  schema creation remains inline DDL executed via EF.

## Impact

- Affected files: `src/AotMemoryServer/Application/Queries/*` (3 files),
  `src/AotMemoryServer/Application/Commands/*` (3 files), possibly `Program.cs`.
- Dependencies: still EF Core 9 + SQLite; `Microsoft.Data.Sqlite` may be removable from handler files
  (still transitively used by EF SQLite provider). No new package required.
- Behavior: identical external behavior (REST + MCP contracts unchanged). Internal change-tracking and
  query generation now handled by EF. AOT constraints preserved (compiled model + source-gen JSON).
- Risk: ensure `DateTimeOffset` is stored/loaded via EF's SQLite provider (no manual string parsing),
  and that pagination counts match prior behavior.
