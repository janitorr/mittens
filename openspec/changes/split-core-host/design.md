## Context

The current `Mittens` project is a single ASP.NET Core web application containing:
- Domain model (`MittensFact`), validation (`MittensFactValidator`)
- CQRS commands and queries with handlers that inject `AppDbContext` directly
- EF Core persistence with compiled model and raw SQL helpers (`FactReader`, `FactWriter`)
- REST endpoints and MCP tools
- All in one `.csproj` with `PublishAot=true`

Handlers are tightly coupled to `AppDbContext`, making unit testing require EF Core setup. Domain logic cannot be reused outside the web host.

## Goals / Non-Goals

**Goals:**
- Extract pure business logic into `Mittens.Core` class library with zero ASP.NET/EF Core/MCP dependencies
- Introduce `IFactReader` and `IFactWriter` interfaces in Core, implemented in Host
- Reorganize into feature folders (`Fact/`) with shared types in `Shared/`
- Enable unit testing of handlers with `NullLogger<T>` and mock interfaces
- Preserve all existing API contracts (REST, MCP) — no breaking changes
- Maintain AOT compatibility throughout

**Non-Goals:**
- No database schema changes — table `MittensFacts` preserved
- No MCP tool name changes — `mittens_list`, `mittens_get`, etc. stay as-is
- No new features or behavioral changes
- No migration to other databases or ORMs
- No changes to serialization format or API response shapes

## Decisions

### 1. Interface granularity: `IFactReader` + `IFactWriter` (not a single repository)

**Rationale:** Mirrors the existing `FactReader`/`FactWriter` split and aligns with CQRS — reads and writes are conceptually separate. A single `IFactRepository` would conflate concerns and make mocking harder in tests that only exercise one side.

**Alternatives considered:**
- Single `IFactRepository`: simpler but less aligned with CQRS
- Per-use-case interfaces (e.g., `IGetFactById`): overkill for a single entity

### 2. `ILogger<T>` stays in Core

**Rationale:** `Microsoft.Extensions.Logging.Abstractions` is a lightweight, AOT-compatible package with no transitive dependencies. `ILogger<T>` is an interface — trivially mockable with `NullLogger<T>.Instance` in unit tests. Defining a custom logging interface would lose `[LoggerMessage]` source generation benefits.

### 3. Feature folder structure: `Fact/Commands/` and `Fact/Queries/` (not flat)

**Rationale:** ~12 files in Core's `Fact/` directory. Sub-grouping by Commands/Queries keeps navigation clean without over-nesting. Host's `Fact/Data/` and `Fact/Endpoints/` follow the same principle.

### 4. Compiled EF model stays in Host

**Rationale:** The compiled model (`Data/Compiled/`) is an EF Core concern. It references the `Fact` entity from Core via namespace, but the model itself belongs to the persistence layer. The `[DbContextModel]` assembly attribute points to the model class, which lives in Host.

### 5. `AppJsonContext` stays in Host

**Rationale:** JSON serialization is an API/transport concern. `AppJsonContext` references types from both Core (`Fact`, `PagedResult<Fact>`) and Host (`HealthStatus`, `ErrorResponse`). Keeping it in Host avoids circular references and keeps Core free of serialization concerns.

### 6. Host project stays named `Mittens`

**Rationale:** The existing project name and output binary (`Mittens`) are used in Docker, CI, and deployment. Renaming would cascade into infrastructure changes. `Mittens.Core` is the new library.

### 7. Mediator source generator runs in Host, discovers handlers in Core

**Rationale:** `Mediator.SourceGenerator` scans all referenced assemblies for `IRequestHandler` implementations. Host references Core, so handlers defined in Core are discovered at compile time. This is the standard pattern for CQRS with source-generated mediators.

## Risks / Trade-offs

| Risk | Mitigation |
|---|---|
| **Compiled EF model namespace mismatch** — generated code references old `Mittens.Models.MittensFact` | Update namespace in compiled files to `Mittens.Core.Fact.Fact`; regenerate model after entity rename |
| **Mediator source generator misses Core handlers** — if assembly reference or analyzer config is wrong | Verify `dotnet build` output includes all handler registrations; add explicit `<ProjectReference>` |
| **`InternalsVisibleTo` breaks** — integration tests lose access to internal types | Update `InternalsVisibleTo` attribute in Core to include test assembly name |
| **AOT trim warnings from cross-assembly references** | Test with `dotnet publish -c Release` after split; suppress known warnings as before |
| **MCP tool serialization context missing Core types** | Add `Fact` and `PagedResult<Fact>` to `AppJsonContext` `[JsonSerializable]` attributes |

## Migration Plan

1. Create `Mittens.Core` project with extracted domain, interfaces, and handlers
2. Update `Mittens` project to reference Core, implement interfaces, reorganize folders
3. Update solution file, test project references
4. `dotnet build` — verify compilation
5. `dotnet test` — verify unit and integration tests pass
6. `dotnet publish -c Release` — verify AOT build
7. Run integration tests against Release binary

**Rollback:** Revert git commit. No database changes, no API contract changes.

## Open Questions

- None identified — all design decisions resolved.
