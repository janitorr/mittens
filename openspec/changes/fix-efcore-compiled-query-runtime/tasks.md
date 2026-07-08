## 1. Spec correction

- [x] 1.1 MODIFY `persistence` "Raw SQL in handlers" requirement: replace `EF.CompileAsyncQuery` + `FromSql` mandate with direct `FromSqlRaw`/`SqlQueryRaw`; forbid `EF.CompileAsyncQuery` + `DbSet.FromSqlRaw`.
- [x] 1.2 ADD `persistence` "AOT correctness verified by integration tests" requirement (Release-build gate).

## 2. Code fix (reads)

- [x] 2.1 Delete `Data/CompiledQueries.cs`.
- [x] 2.2 Add `Data/MemoryFactSql.cs` with static SQL constants for all read operations, using `@p IS NULL OR` pattern for optional filters and `'%' || @p0 || '%'` for search LIKE terms.
- [x] 2.3 Rewrite `GetFactByIdHandler`, `GetFactsHandler`, `SearchFactsHandler` to call `FromSqlRaw`/`SqlQueryRaw` via `MemoryFactSql`.
- [x] 2.4 Rewrite `UpsertFactHandler`, `UpdateFactHandler`, `DeleteFactHandler` selects to use `MemoryFactSql`.

## 3. Verification

- [x] 3.1 `dotnet build -c Release` succeeds.
- [x] 3.2 Run integration + unit tests against the Release build → target 59/59 pass.
- [x] 3.3 `dotnet publish -c Release` AOT succeeds.
- [ ] 3.4 Commit, tag `v1.0.3` (PATCH), and push per AGENTS.md (tag before push).
