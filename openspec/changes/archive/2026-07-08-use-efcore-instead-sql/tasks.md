## 1. Compiled query definitions

- [x] 1.1 Add `Data/CompiledQueries.cs` static class declaring 7 `EF.CompileAsyncQuery` definitions (by Id, by Category/Key/Scope, filtered list count+page, search count+page) using `FromSqlRaw` with `@pN` parameters.
- [x] 1.2 Ensure the compiled model (`Data/Compiled/`) and AOT source generator recognize the `FromSqlRaw` mapped entity returns.

## 2. Query Handlers (read paths)

- [x] 2.1 Rewrite `GetFactsHandler` to invoke the compiled `FromSqlRaw` list + count queries with `@p IS NULL OR column = @p` pattern for optional filters; no runtime SQL building.
- [x] 2.2 Rewrite `GetFactByIdHandler` to invoke the compiled `FromSqlRaw` by-Id query; no ADO.NET plumbing.
- [x] 2.3 Rewrite `SearchFactsHandler` to invoke the compiled `FromSqlRaw` search query (`LIKE` on Key/Value + optional filters via `IS NULL OR` pattern); no runtime SQL building.

## 3. Command Handlers (write paths)

- [x] 3.1 Rewrite `UpsertFactHandler` to select existing via compiled `FromSqlRaw`, apply `ResolveConflict`, then `ExecuteSqlInterpolatedAsync` update / `Add`+`SaveChangesAsync` insert.
- [x] 3.2 Rewrite `UpdateFactHandler` to select via compiled `FromSqlRaw`, return null if missing, `ExecuteSqlInterpolatedAsync` update.
- [x] 3.3 Rewrite `DeleteFactHandler` to select via compiled `FromSqlRaw`, return false if missing, `ExecuteSqlInterpolatedAsync` delete.

## 4. Startup DDL

- [x] 4.1 `Program.cs` already uses `db.Database.ExecuteSqlRawAsync(...)` for DDL; no `Microsoft.Data.Sqlite` references remain.

## 5. Verification

- [x] 5.1 Build and run `dotnet publish -c Release` — AOT/trimming succeeds with compiled queries + compiled model.
- [x] 5.2 All 59 tests pass (33 unit + 26 integration), confirming REST/MCP parity, pagination, conflict resolution, and unique constraint behavior.
- [x] 5.3 No regressions in pagination, conflict resolution, unique-constraint behavior, or validation.
- [x] 5.4 Bump version with a semver tag per AGENTS.md after CI passes (tagged `v1.0.2`).
