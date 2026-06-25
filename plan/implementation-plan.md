# AOT Memory Server — Implementation Plan

## Step 1: Create project skeleton
- `dotnet new web -o src/AotMemoryServer --use-minimal-apis`
- Edit `.csproj`: `PublishAot=true`, `StripSymbolsAfterPublish=true`, target `net10.0`
- Add NuGet packages: `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Design`

## Step 2: Data model & DbContext
- Write `Models/MemoryFact.cs` — entity with all columns, data annotations
- Write `Data/AppDbContext.cs` — `OnModelCreating` for indexes + unique constraint
- Write `Data/AppDbContextModel.cs` — EF Core compiled model (AOT requirement)
- Create initial migration: `dotnet ef migrations add InitialCreate`

## Step 3: Validation service
- Write `Services/ValidationService.cs` — static methods:
  - `ValidateLength` (10k limit)
  - `ValidateNoSecrets` (regex on key patterns)
  - `ValidateNoRawCode` (C#/JS/Python markers)
  - `ValidateCategory` (warn on unknown, accept)
  - `ResolveConflict` (confidence comparison, force flag)

## Step 4: Memory service
- Write `Services/MemoryService.cs` — wraps DbContext:
  - `GetAsync(category?, scope?, key?)`
  - `GetByIdAsync(id)`
  - `UpsertAsync(fact, force?)` — validates, then upserts
  - `UpdateAsync(id, fact)`
  - `DeleteAsync(id)`
  - `SearchAsync(q, category?, scope?, limit?)` — LIKE on Value
- All writes logged via `ILogger`

## Step 5: REST endpoints
- Write `Endpoints/MemoryEndpoints.cs` — extension method `MapMemoryEndpoints`:
  - `GET /memory` — calls `GetAsync`
  - `GET /memory/{id}` — calls `GetByIdAsync`
  - `POST /memory` — calls `UpsertAsync`
  - `PUT /memory/{id}` — calls `UpdateAsync`
  - `DELETE /memory/{id}` — calls `DeleteAsync`
- Write `Endpoints/HealthEndpoints.cs`:
  - `GET /health` — `db.Database.CanConnectAsync()`
  - `GET /ready` — also checks migration applied

## Step 6: MCP endpoint
- Write `Endpoints/McpEndpoints.cs`
- Parse JSON-RPC 2.0 from request body
- Route to service methods via `method` field
- Return standard JSON-RPC response format

## Step 7: Security middleware
- Write `Middleware/ApiKeyMiddleware.cs`:
  - Reads `X-Memory-ApiKey` from header
  - Compares against `MEMORY_API_KEY` env
  - Only enforced on POST/PUT/DELETE

## Step 8: Backup endpoint
- `POST /admin/backup` — timestamped copy of `memory.db` to `/data/backups/`

## Step 9: Metrics
- `/metrics` endpoint — Prometheus text format
- Track `memory_request_total`, `memory_request_duration_seconds`

## Step 10: Wire everything in Program.cs
- Register services (DI)
- Configure Kestrel binding from env/args
- Apply migrations on startup
- Map endpoint groups
- JSON source generator for AOT-safe serialization

## Step 11: OpenCode hook scripts
- `hooks/beforeAgentStart.sh` — bash, polls `/health`, starts compose
- `hooks/beforeAgentStart.ps1` — PowerShell equivalent

## Step 12: Dockerfile
- Multi-stage AOT build

## Step 13: docker-compose.yml
- Service definition with volume, port, env vars

## Step 14: Tests
- `tests/AotMemoryServer.Tests.Unit/`:
  - `ValidationServiceTests.cs`
  - `ConflictResolutionTests.cs`
- `tests/AotMemoryServer.Tests.Integration/`:
  - `CustomWebApplicationFactory.cs`
  - `RestEndpointTests.cs`
  - `McpEndpointTests.cs`

## Step 15: Housekeeping
- `.gitignore`, `.env.example`, `README.md`
