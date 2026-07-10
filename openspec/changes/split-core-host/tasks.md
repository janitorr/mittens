## 1. Create Core Project

- [x] 1.1 Create `src/Mittens.Core/` directory and `Mittens.Core.csproj` with references to `Mediator.Abstractions` and `Microsoft.Extensions.Logging.Abstractions`
- [x] 1.2 Create folder structure: `Fact/Commands/`, `Fact/Queries/`, `Shared/`
- [x] 1.3 Add `Mittens.Core` to `Mittens.slnx`

## 2. Move Domain Models to Core

- [x] 2.1 Move `MittensFact.cs` to `src/Mittens.Core/Fact/Fact.cs`, rename class to `Fact`, update namespace to `Mittens.Core.Fact`
- [x] 2.2 Move `ValidationError.cs` to `src/Mittens.Core/Fact/ValidationError.cs`, update namespace to `Mittens.Core.Fact`
- [x] 2.3 Move `PagedResult.cs` to `src/Mittens.Core/Shared/PagedResult.cs`, update namespace to `Mittens.Core.Shared`
- [x] 2.4 Move `ValidationException.cs` to `src/Mittens.Core/Shared/ValidationException.cs`, update namespace to `Mittens.Core.Shared`

## 3. Create Interfaces in Core

- [x] 3.1 Create `IFactReader.cs` in `src/Mittens.Core/Fact/` with methods: `GetByIdAsync`, `GetByCategoryKeyScopeAsync`, `ListAsync`, `SearchAsync`
- [x] 3.2 Create `IFactWriter.cs` in `src/Mittens.Core/Fact/` with methods: `InsertAsync`, `UpdateAsync`, `DeleteAsync`

## 4. Move Validation to Core

- [x] 4.1 Move `MittensFactValidator.cs` to `src/Mittens.Core/Fact/FactValidator.cs`, update namespace and references to `Fact` (not `MittensFact`)

## 5. Move CQRS Handlers to Core

- [x] 5.1 Move `GetFacts.cs` to `src/Mittens.Core/Fact/Queries/GetFacts.cs`, update handler to inject `IFactReader` instead of `AppDbContext`, update namespace to `Mittens.Core.Fact.Queries`
- [x] 5.2 Move `GetFactById.cs` to `src/Mittens.Core/Fact/Queries/GetFactById.cs`, update handler to inject `IFactReader`, update namespace
- [x] 5.3 Move `SearchFacts.cs` to `src/Mittens.Core/Fact/Queries/SearchFacts.cs`, update handler to inject `IFactReader`, update namespace
- [x] 5.4 Move `UpsertFact.cs` to `src/Mittens.Core/Fact/Commands/UpsertFact.cs`, update handler to inject `IFactReader` + `IFactWriter`, update namespace to `Mittens.Core.Fact.Commands`
- [x] 5.5 Move `UpdateFact.cs` to `src/Mittens.Core/Fact/Commands/UpdateFact.cs`, update handler to inject `IFactReader` + `IFactWriter`, update namespace
- [x] 5.6 Move `DeleteFact.cs` to `src/Mittens.Core/Fact/Commands/DeleteFact.cs`, update handler to inject `IFactWriter`, update namespace

## 6. Reorganize Host Project

- [x] 6.1 Create `src/Mittens/Memory/Data/` and `src/Mittens/Memory/Endpoints/` directories
- [x] 6.2 Move `AppDbContext.cs` to `src/Mittens/Memory/Data/AppDbContext.cs`, update namespace to `Mittens.Memory.Data`, update `DbSet<MittensFact>` → `DbSet<Fact>`, add project reference to `Mittens.Core`
- [x] 6.3 Move `FactReader.cs` to `src/Mittens/Memory/Data/FactReader.cs`, convert from static methods to instance class implementing `IFactReader`, inject `AppDbContext` via constructor
- [x] 6.4 Move `FactWriter.cs` to `src/Mittens/Memory/Data/FactWriter.cs`, convert from static methods to instance class implementing `IFactWriter`, inject `AppDbContext` via constructor
- [x] 6.5 Move `MittensFactSql.cs` to `src/Mittens/Memory/Data/FactSql.cs`, update namespace to `Mittens.Memory.Data`
- [x] 6.6 Move `Compiled/` directory to `src/Mittens/Memory/Data/Compiled/`, update namespace references from `Mittens.Models.MittensFact` to `Mittens.Core.Fact.Fact`
- [x] 6.7 Move `MittensEndpoints.cs` to `src/Mittens/Memory/Endpoints/FactEndpoints.cs`, update namespace to `Mittens.Memory.Endpoints`, update all type references
- [x] 6.8 Move `MittensMcpTools.cs` to `src/Mittens/Memory/Endpoints/FactMcpTools.cs`, update namespace to `Mittens.Memory.Endpoints`, update all type references
- [x] 6.9 Move `HealthEndpoints.cs` to `src/Mittens/Endpoints/HealthEndpoints.cs`, update namespace to `Mittens.Endpoints`
- [x] 6.10 Move `AppJsonContext.cs` to `src/Mittens/Serialization/AppJsonContext.cs`, update namespace to `Mittens.Serialization`, update `[JsonSerializable]` attributes to reference `Fact` and `Mittens.Core.Shared` types
- [x] 6.11 Move `Dtos.cs` to `src/Mittens/Serialization/Dtos.cs`, update namespace to `Mittens.Serialization`
- [x] 6.12 Delete old directories (`Models/`, `Application/`, `Data/`, `Endpoints/` at old locations)

## 7. Update Program.cs

- [x] 7.1 Update `using` statements to new namespaces
- [x] 7.2 Register `IFactReader` → `FactReader` and `IFactWriter` → `FactWriter` in DI container
- [x] 7.3 Update `AddMediator` to discover handlers from `Mittens.Core` assembly
- [x] 7.4 Update DDL startup code to reference `Fact` entity (table name `MittensFacts` preserved)
- [x] 7.5 Update endpoint mapping calls to use renamed classes

## 8. Update Test Projects

- [x] 8.1 Update `Mittens.Tests.Unit` project reference from `Mittens` to `Mittens.Core`
- [x] 8.2 Update `Mittens.Tests.Integration` project reference to include `Mittens` (Host)
- [x] 8.3 Update unit test namespaces and type references (`MittensFact` → `Fact`, etc.)
- [x] 8.4 Update integration test `WebApplicationFactory<Program>` to reference Host's `Program`
- [x] 8.5 Update `InternalsVisibleTo` in Core and Host to reference test assemblies

## 9. Build and Verify

- [x] 9.1 Run `dotnet build` — all projects compile without errors
- [x] 9.2 Run `dotnet test` — all unit and integration tests pass
- [x] 9.3 Run `dotnet publish -c Release` — AOT native binary produced without errors
- [x] 9.4 Run integration tests against Release binary — all pass
