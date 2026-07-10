## ADDED Requirements

### Requirement: Host references Core and implements its interfaces
The `Mittens` web project SHALL reference `Mittens.Core` and provide concrete implementations of `IFactReader` and `IFactWriter` using EF Core and SQLite.

#### Scenario: DI wires concrete implementations to Core interfaces
- **WHEN** the application starts
- **THEN** `IFactReader` is registered as `FactReader` and `IFactWriter` as `FactWriter` in the DI container

#### Scenario: Handlers receive concrete implementations
- **WHEN** a CQRS handler is resolved via `ISender`
- **THEN** its `IFactReader`/`IFactWriter` dependencies are satisfied by the Host's implementations

### Requirement: Host provides REST API endpoints
The Host SHALL expose REST endpoints at `/api/memory` for: list facts (GET), get by ID (GET), upsert (POST), update by ID (PUT), and delete by ID (DELETE). Endpoints SHALL dispatch to CQRS handlers via `ISender`.

#### Scenario: REST endpoints return correct responses
- **WHEN** a client sends a valid REST request to `/api/memory`
- **THEN** the response matches the existing API contract (status codes, response body shape)

#### Scenario: Validation errors return 400
- **WHEN** a POST or PUT request contains invalid data
- **THEN** the endpoint catches `ValidationException` and returns 400 with `ErrorResponse`

### Requirement: Host provides MCP tools
The Host SHALL expose MCP tools at `/mcp` (stateless HTTP transport): `mittens_list`, `mittens_get`, `mittens_search`, `mittens_set`, `mittens_update`, `mittens_delete`. Tool names SHALL remain unchanged from the current implementation.

#### Scenario: MCP tools dispatch to CQRS handlers
- **WHEN** an MCP client invokes a tool
- **THEN** the tool method uses `ISender` to dispatch to the appropriate CQRS handler

### Requirement: Host provides health check endpoint
The Host SHALL expose a health check at `/api/health` that verifies database connectivity and returns a `HealthStatus` response.

#### Scenario: Health endpoint returns 200 when healthy
- **WHEN** the database is accessible
- **THEN** `GET /api/health` returns 200 with `{"status": "healthy"}`

#### Scenario: Health endpoint returns 503 when unhealthy
- **WHEN** the database is not accessible
- **THEN** `GET /api/health` returns 503

### Requirement: Host manages EF Core with compiled model
The Host SHALL use a precompiled EF Core model (`Data/Compiled/`) for AOT compatibility. Database schema SHALL be created via inline DDL at startup (no migrations).

#### Scenario: Database is created on startup
- **WHEN** the application starts
- **THEN** the `MittensFacts` table and indices are created if they do not exist

### Requirement: Host provides source-generated JSON serialization
The Host SHALL define `AppJsonContext` with `[JsonSerializable]` attributes for all types crossing the wire, including `Fact`, `PagedResult<Fact>`, `ValidationError`, `HealthStatus`, and `ErrorResponse`.

#### Scenario: Serialization works without reflection
- **WHEN** a response is serialized to JSON
- **THEN** it uses `AppJsonContext.Default` (source-generated), not runtime reflection

### Requirement: AOT publishing works for Host
The Host SHALL support `dotnet publish -c Release` with `PublishAot=true`, producing a native binary (~33 MB).

#### Scenario: Release publish succeeds
- **WHEN** running `dotnet publish -c Release`
- **THEN** a native binary is produced without trim or AOT warnings (suppressed warnings excluded)
