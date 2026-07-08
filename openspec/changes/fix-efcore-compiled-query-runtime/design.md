## Context

The memory server uses EF Core 10 with SQLite under AOT (`PublishAot=true`), with a compiled model
(`Data/Compiled/AppDbContextModel`) registered via `UseModel(AppDbContextModel.Instance)`.
The `persistence` spec mandates raw SQL for AOT compatibility, no EF Core LINQ query translation,
and no EF migrations.

The previous change `use-efcore-instead-sql` attempted to satisfy this with `EF.CompileAsyncQuery` +
`FromSqlRaw`/`FromSql` on `DbSet<T>`. EF Core 10's compiled-query source generator does **not**
support raw-SQL extension methods on `DbSet<T>` inside compiled queries — it emits a call that passes
an `IQueryable<MemoryFact>` where the generated `FromSqlRaw` method expects a `DbSet<MemoryFact>`,
throwing `ArgumentException` at execution. This fails in CI (Release build) even though it compiles
and `dotnet publish -c Release` succeeds (native compilation never executes the queries).

The working, AOT-safe alternative is to call `FromSqlRaw`/`SqlQueryRaw` directly — the SQL is fully
provided as raw text and the compiled model handles entity mapping, satisfying the AOT mandate without
a broken compiled-query wrapper.

## Goals / Non-Goals

**Goals:**
- Replace `EF.CompileAsyncQuery` + `FromSqlRaw` reads with direct `db.MemoryFacts.FromSqlRaw(...)`
  and `db.Database.SqlQueryRaw<T>(...)` calls.
- Keep `Data/FactWriter.cs` for writes (unchanged approach).
- Centralize SQL strings in a new `Data/MemoryFactSql.cs` so handlers remain thin.
- Correct the `persistence` spec (MODIFY the raw-SQL requirement; ADD an AOT-verification requirement).
- Add a Release-build integration-test gate to CI verification.

**Non-Goals:**
- `EF.CompileAsyncQuery` + `FromSql`/`FromSqlRaw` on `DbSet<T>` — unsupported in EF Core 10.
- Runtime-translated LINQ (`Where`/`OrderBy`/`Like`) — not AOT-safe.
- EF migrations; switching database providers.
- Changing the `MemoryFact` model shape, REST/MCP contracts, or validation logic.

## Decisions

1. **Direct `FromSqlRaw` / `SqlQueryRaw` via EF for reads.**
   AOT-safe: SQL is fully provided as raw text and the compiled model is used. No runtime expression
   translation. Alternative considered (and rejected from the `use-efcore-instead-sql` change):
   `EF.CompileAsyncQuery` + `DbSet.FromSqlRaw` — rejects at runtime in EF Core 10.

2. **Optional filters use `@p IS NULL OR "Col" = @p`.**
   Each optional filter parameter (Category, Scope, Key) is passed as a nullable `string?`. When null,
   the `IS NULL` condition short-circuits (effectively removes the filter). There is no runtime SQL
   string building — the SQL template is a compile-time constant. The search `LIKE` term uses SQLite's
   `'%' || @p0 || '%'` string concatenation so the `%` wrapping stays in SQL, not C#.

3. **SQL centralized in `Data/MemoryFactSql.cs`.**
   A static class with `const string` fields for each query template. Handlers call
   `db.MemoryFacts.FromSqlRaw(MemoryFactSql.X, p0, p1, ...)` — no inline SQL strings in handlers,
   and no compiled-query wrapper.

4. **Verification gate: integration tests against Release build.**
   `dotnet publish -c Release` only verifies native compilation, not query correctness. The CI/CD
   workflow must run `dotnet build -c Release` then `dotnet test --no-build` to catch runtime SQL
   issues that Debug builds may mask.

## Risks / Trade-offs

- [Risk] Debug-only test runs mask runtime SQL errors. → Mitigation: spec and CI mandate integration
  tests against the Release build.
- [Trade-off] No compiled-query wrapper (no source-generated query caching). Accepted: `FromSqlRaw` is
  already AOT-safe via the compiled model, and the compiled-query path is broken for raw SQL in EF Core 10.

## Migration Plan

- Pure refactor; no schema or data migration. Deploy as normal build.
- Rollback: revert commit / redeploy previous image. Data untouched.

## Open Questions

- None.
