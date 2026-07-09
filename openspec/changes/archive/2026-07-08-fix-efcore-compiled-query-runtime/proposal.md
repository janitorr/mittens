## Why

The archived change `use-efcore-instead-sql` replaced `Microsoft.Data.Sqlite` ADO.NET with
`EF.CompileAsyncQuery` + `FromSqlRaw`/`FromSql` on `DbSet<T>`. This combination is **unsupported in
EF Core 10** — the compiled-query source generator emits a call passing `IQueryable<MemoryFact>`
where the generated `FromSqlRaw` method expects `DbSet<MemoryFact>`, throwing `ArgumentException` at
runtime. CI caught it on every push; local Debug builds masked it because `dotnet publish -c Release`
only verifies native compilation (queries never execute) and Debug test runs did not surface the
generated shim bug. The currently-synced main `persistence` spec is now **incorrect**: it mandates
`EF.CompileAsyncQuery` with `FromSql` for reads. This change corrects the approach and the spec.

## What Changes

- Replace `EF.CompileAsyncQuery` + `FromSqlRaw` reads with direct `db.MemoryFacts.FromSqlRaw(...)`
  / `db.Database.SqlQueryRaw<T>(...)` called directly in handlers (AOT-safe, no runtime LINQ
  translation, no ADO.NET).
- Delete `Data/CompiledQueries.cs`.
- Add `Data/MemoryFactSql.cs` with static SQL string constants so handlers stay thin.
- Keep `Data/FactWriter.cs` for writes (unchanged).
- Correct the `persistence` spec: MODIFY the "Raw SQL in handlers" requirement to mandate direct
  `FromSqlRaw`/`SqlQueryRaw` and forbid `EF.CompileAsyncQuery` + `DbSet.FromSqlRaw`; ADD an
  "AOT correctness verified by integration tests" requirement (Release-build verification gate).
- No REST, MCP, or model behavior changes.

## Capabilities

### New Capabilities
<!-- none -->

### Modified Capabilities
- `persistence`: The "Raw SQL in handlers (no LINQ query translation)" requirement currently mandates
  `EF.CompileAsyncQuery` with `FromSql` for reads — this is unsupported in EF Core 10 and must be
  corrected to direct `FromSqlRaw`/`SqlQueryRaw`. Add a new requirement for AOT verification via
  integration tests against the Release build.

## Impact

- Affected files: `src/AotMemoryServer/Application/Commands/*` (3), `src/AotMemoryServer/Application/Queries/*` (3),
  `src/AotMemoryServer/Data/CompiledQueries.cs` (delete), `src/AotMemoryServer/Data/MemoryFactSql.cs` (new).
- Dependencies: unchanged (EF Core 10 + SQLite; `Microsoft.Data.Sqlite` removed from handlers).
- Behavior: identical external REST + MCP contracts. Internal reads now use direct EF raw SQL.
- Risk: none material; the direct `FromSqlRaw`/`SqlQueryRaw` approach is proven (it passed 26/26
  integration tests before switching to broken compiled queries).
