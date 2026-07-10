# feature-folders

## Purpose
Defines the folder structure conventions for both Core and Host projects.

## Requirements

### Requirement: Core uses feature folder structure
Code in `Mittens.Core` SHALL be organized under a `Fact/` feature folder. Fact-related commands, queries, models, validation, and interfaces SHALL reside within `Fact/`.

#### Scenario: Fact commands are in Fact/Commands
- **WHEN** locating a Fact command handler
- **THEN** it is found at `Mittens.Core/Fact/Commands/`

#### Scenario: Fact queries are in Fact/Queries
- **WHEN** locating a Fact query handler
- **THEN** it is found at `Mittens.Core/Fact/Queries/`

### Requirement: Shared types are in Shared folder
Generic, reusable types (`PagedResult<T>`, `ValidationException`) SHALL reside in `Mittens.Core.Shared/`, separate from feature-specific code.

#### Scenario: Shared types are accessible across features
- **WHEN** a future feature needs pagination
- **THEN** it imports `Mittens.Core.Shared` without pulling in Fact-specific code

### Requirement: Host uses feature folder structure for Fact
Fact-related infrastructure in the Host (data access, endpoints) SHALL be organized under `Memory/` subdirectories: `Memory/Data/` for persistence, `Memory/Endpoints/` for API definitions.

#### Scenario: Fact data access is in Memory/Data
- **WHEN** locating `FactReader`, `FactWriter`, or `FactSql`
- **THEN** they are found at `Mittens/Memory/Data/`

#### Scenario: Fact endpoints are in Memory/Endpoints
- **WHEN** locating REST or MCP endpoint definitions for Facts
- **THEN** they are found at `Mittens/Memory/Endpoints/`

### Requirement: Plumbing lives outside feature folders
Non-feature code (health checks, serialization, Program.cs) SHALL reside at the Host root level or in dedicated root-level directories (`Endpoints/`, `Serialization/`), not inside feature folders.

#### Scenario: Health endpoint is not in Memory folder
- **WHEN** locating the health check endpoint
- **THEN** it is found at `Mittens/Endpoints/HealthEndpoints.cs`, not inside `Memory/`

### Requirement: Compiled EF model is in Memory/Data/Compiled
The precompiled EF Core model files SHALL reside in `Mittens/Memory/Data/Compiled/`, co-located with the data access layer they support.

#### Scenario: Compiled model is discoverable with data access code
- **WHEN** navigating the Host project
- **THEN** `AppDbContextModel.cs` and related files are found under `Memory/Data/Compiled/`
