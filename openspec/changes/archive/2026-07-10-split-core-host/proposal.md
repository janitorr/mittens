## Why

The monolithic `Mittens` project mixes domain logic, persistence, and HTTP transport in a single assembly. This makes unit testing CQRS handlers difficult (they depend on `AppDbContext`), prevents reuse of the business logic outside the web host, and violates the dependency inversion principle — handlers reach directly into infrastructure rather than depending on abstractions. Splitting into a pure **Core** library and a **Host** shell enables testable domain logic, clear dependency boundaries, and prepares the codebase for future features without entangling them in infrastructure concerns.

## What Changes

- Extract a new `Mittens.Core` class library containing domain models, validation, CQRS commands/queries, and interfaces for external resources
- Rename `MittensFact` → `Fact` (namespace provides the `Mittens` prefix)
- Introduce `IFactReader` and `IFactWriter` interfaces in Core, implemented by concrete classes in Host
- Reorganize Host into feature folders (`Fact/Data/`, `Fact/Endpoints/`) with plumbing (`Health/`, `Serialization/`) at root level
- Update handler injection from `AppDbContext` to `IFactReader`/`IFactWriter`
- Update solution, project references, test project references, and `InternalsVisibleTo`

## Capabilities

### New Capabilities
- `core-module`: Pure business logic library with domain models, validation, CQRS handlers, and interfaces for external resources. No references to ASP.NET, EF Core, or MCP.
- `host-module`: Web application shell that references Core, implements its interfaces, and wires REST/MCP endpoints, EF Core persistence, and startup configuration.
- `feature-folders`: Code organized by feature (`Fact/`) rather than by layer, with shared types in `Shared/` and plumbing at root level.

### Modified Capabilities
- *(none — no existing specs to modify)*

## Impact

- **Project structure**: Single project → two projects (`Mittens.Core` + `Mittens`)
- **Solution file**: `Mittens.slnx` updated with new project
- **Namespaces**: `Mittens.Models.*` → `Mittens.Core.Fact.*`, `Mittens.Application.*` → `Mittens.Core.Fact.*`
- **Handler dependencies**: `AppDbContext` → `IFactReader`/`IFactWriter`
- **Entity naming**: `MittensFact` → `Fact`, `MittensFactValidator` → `FactValidator`, `MittensFactSql` → `FactSql`
- **Endpoint naming**: `MittensEndpoints` → `FactEndpoints`, `MittensMcpTools` → `FactMcpTools`
- **Test projects**: Project references updated; unit tests target Core, integration tests target Host
- **Database**: No schema changes — table name `MittensFacts` preserved for backward compatibility
- **API surface**: No breaking changes to REST or MCP contracts
