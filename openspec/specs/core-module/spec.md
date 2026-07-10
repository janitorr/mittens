# core-module

## Purpose
Defines the structure and constraints of the `Mittens.Core` class library — pure domain logic with no infrastructure dependencies.

## Requirements

### Requirement: Core module contains pure domain logic
The `Mittens.Core` class library SHALL contain all domain models, validation logic, CQRS commands, CQRS queries, and handler implementations. It SHALL NOT reference ASP.NET Core, EF Core, ModelContextProtocol, or any web/infrastructure packages.

#### Scenario: Core builds without web dependencies
- **WHEN** `Mittens.Core` is built in isolation
- **THEN** the build succeeds without referencing `Microsoft.AspNetCore.*`, `Microsoft.EntityFrameworkCore.*`, or `ModelContextProtocol.*`

#### Scenario: Core references only abstractions
- **WHEN** inspecting `Mittens.Core.csproj` package references
- **THEN** only `Mediator.Abstractions` and `Microsoft.Extensions.Logging.Abstractions` are present

### Requirement: Domain entity is named Fact
The primary domain entity SHALL be named `Fact` (not `MittensFact`) and reside in the `Mittens.Core.Fact` namespace. It SHALL be a plain POCO with properties: `Id`, `Category`, `Key`, `Value`, `Scope`, `Confidence`, `Source`, `UpdatedAt`, `IsDeprecated`.

#### Scenario: Fact entity is usable without EF Core
- **WHEN** a handler instantiates a `Fact` object
- **THEN** no EF Core attributes or infrastructure dependencies are required on the entity

### Requirement: IFactReader interface defines read operations
Core SHALL define an `IFactReader` interface with methods for: get by ID, get by category/key/scope, paginated list with optional filters, and text search with optional filters.

#### Scenario: Handler depends on IFactReader
- **WHEN** a query handler is instantiated
- **THEN** it receives `IFactReader` via constructor injection, not `AppDbContext`

### Requirement: IFactWriter interface defines write operations
Core SHALL define an `IFactWriter` interface with methods for: insert, update, and delete operations on `Fact` entities.

#### Scenario: Command handler depends on IFactWriter
- **WHEN** a command handler performs a write operation
- **THEN** it calls `IFactWriter` methods, not `AppDbContext` methods directly

### Requirement: Validation logic is in Core
The `FactValidator` class SHALL reside in Core and perform: required field validation, max length checks (10,000 chars), secret/API key detection via source-generated regex, and known category validation. Unknown categories SHALL produce warnings, not errors.

#### Scenario: Validation runs without infrastructure
- **WHEN** `FactValidator.Validate()` is called on a `Fact` instance
- **THEN** validation completes without any database, logging, or web framework dependencies

### Requirement: ValidationException carries validation errors
Core SHALL define a `ValidationException` that wraps an `IReadOnlyList<ValidationError>`, where each error includes `Property`, `Message`, and `IsWarning`.

#### Scenario: Exception is thrown on validation failure
- **WHEN** a command handler encounters validation errors (not warnings)
- **THEN** it throws `ValidationException` with the list of errors

### Requirement: PagedResult is a shared generic type
`PagedResult<T>` SHALL reside in `Mittens.Core.Shared` and contain `Items`, `TotalCount`, `Page`, and `PageSize` properties.

#### Scenario: PagedResult is reusable across features
- **WHEN** a future feature needs paginated results
- **THEN** it can use `PagedResult<T>` from the Shared namespace

### Requirement: Conflict resolution uses confidence-based logic
When upserting a fact that already exists, the system SHALL resolve conflicts by comparing confidence scores: higher confidence wins. A `force` parameter SHALL override this behavior.

#### Scenario: Higher confidence wins
- **WHEN** upserting a fact with lower confidence than the existing one
- **THEN** the existing fact's values are preserved (conflict resolved in favor of higher confidence)

#### Scenario: Force overrides conflict resolution
- **WHEN** upserting with `force=true`
- **THEN** the incoming fact's values replace the existing ones regardless of confidence
