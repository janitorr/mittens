## Context

The memory server uses EF Core 9 with SQLite under AOT (`PublishAot=true`), with a compiled model
(`Data/Compiled/AppDbContextModel`) registered via `UseModel(AppDbContextModel.Instance)`. The
CQRS handlers today open `db.Database.GetDbConnection()` and run hand-written `Microsoft.Data.Sqlite`
`SqliteCommand` strings with positional `@p0`/`@p1` parameters and manual `DbDataReader` →
`MemoryFact` mapping (including string parsing `DateTimeOffset` via `"O"`).

The `persistence` spec mandates: **raw SQL for AOT compatibility, no EF Core LINQ query translation**
and **no EF migrations**. EF Core's standard LINQ pipeline is not AOT-safe (it relies on runtime
expression translation/codegen). The correct way to "use EF Core instead of inline ADO.NET SQL" while
honoring the AOT constraint is **source-generated compiled queries** (`EF.CompileAsyncQuery` /
`EF.CompileAsyncQuery` with `FromSql` for reads, and parameterized `ExecuteSql`/`ExecuteSqlInterpolated`
for writes). These compile the query up front (build time), so no runtime reflection/translation occurs
and the project stays trimmer/AOT-safe.

## Goals / Non-Goals

**Goals:**
- Replace direct `Microsoft.Data.Sqlite` ADO.NET usage in handlers with EF Core surface area:
  - Reads: source-generated `EF.CompileAsyncQuery` backed by `FromSql` raw SQL.
  - Writes: EF `ExecuteSql` / `ExecuteSqlInterpolatedAsync` (compiled where applicable) over the
    `DbContext` connection.
- Preserve exact behavior: REST (`/api/memory`) + MCP (`/mcp`) contracts, paging semantics, conflict
  resolution, validation, and the unique (Category, Key, Scope) constraint.
- Keep AOT compatibility: no LINQ query translation at runtime, compiled model retained, no EF migrations.

**Non-Goals:**
- Switching database providers (stays SQLite).
- Introducing EF migrations.
- Using runtime-translated LINQ (`Where(...).OrderBy(...)`) — explicitly out of scope for AOT.
- Changing the `MemoryFact` model shape or API surface.

## Decisions

1. **Use source-generated compiled queries (`EF.CompileAsyncQuery`) for reads.**
   - Rationale: compiled queries are AOT-safe (no runtime expression translation) and are the
     sanctioned way to use EF under trimming/AOT. They still execute parameterized raw SQL via
     `FromSql`, satisfying the "raw SQL, no LINQ translation" spec requirement.
   - Alternative considered: standard LINQ `Where`/`OrderBy` — rejected: not AOT-safe.
   - Alternative considered: keep raw `SqliteCommand` — rejected: that is the inline-SQL pattern we are
     replacing.

2. **Writes go through EF `ExecuteSql(Async)` with `FromSql`/`ExecuteSqlInterpolated`, not ADO.NET.**
   - Rationale: keeps parameter handling and connection management inside EF (compiled model), removes
     `Microsoft.Data.Sqlite` from handlers, and stays AOT-safe because the SQL is static/compiled.

3. **Keep inline DDL in `Program.cs` executed via `db.Database.ExecuteSqlRawAsync`.**
   - Rationale: satisfies "no EF migrations, inline DDL" while consolidating on EF's SQL execution path.

4. **Paging via `Skip`/`Take` is NOT used** (that is LINQ). Instead compiled `FromSql` queries embed
   `LIMIT @p OFFSET @p` parameters, mirroring the existing two-statement count+page pattern.

## Risks / Trade-offs

- [Risk] `EF.CompileAsyncQuery` with `FromSql` still requires the compiled model to recognize the
  entity mapping. → Mitigation: the compiled model already fully maps `MemoryFact`; `FromSql` returns
  tracked/untracked entities from the model.
- [Risk] Compiled queries must be defined as `static`/`partial` members usable by the source generator.
  → Mitigation: declare them in a dedicated `Data/CompiledQueries.cs` static class.
- [Risk] `DateTimeOffset` storage parity. → Mitigation: EF SQLite provider uses ISO-8601; existing rows
  written via `"O"` format are compatible.
- [Trade-off] We keep raw SQL text (now inside compiled EF queries) rather than strongly-typed LINQ —
  accepted per the AOT mandate.

## Migration Plan

- Pure refactor; no schema/data migration. Deploy as normal build.
- Rollback: revert commit / redeploy previous image. Data untouched.
- Verify AOT: `dotnet publish -c Release` must succeed (trimming + compiled queries).

## Open Questions

- None; approach stays within AOT and spec constraints.
