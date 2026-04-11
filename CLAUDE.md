# CLAUDE.md

## Project Overview

**Resultant** is a high-performance, struct-based Result pattern library for .NET. It provides `Result`, `Result<T>`, a sealed error hierarchy, `PagedResult<T>`, and functional pipelines (Map, Bind, Then, Tap, Ensure, Match) as a structured alternative to exception-based error handling. Published as multiple NuGet packages with zero dependencies in the core library.

- **Repository:** adomorn/Resultant
- **License:** MIT
- **Current Version:** 2.0.0
- **Target Framework:** net8.0
- **Author:** Arda Terekeci

## Build & Test Commands

```bash
dotnet restore                              # Restore all dependencies
dotnet build --no-restore                   # Build the solution
dotnet test --no-build                      # Run all tests
dotnet test --no-build --verbosity normal   # Run tests with detailed output
dotnet pack --no-build --configuration Release  # Create NuGet packages
```

The solution file is `Resultant.sln` at the repository root.

## Project Structure

```
Resultant.sln                                    # Solution file (18 projects)
Directory.Build.props                            # Shared build properties
Directory.Packages.props                         # Central package management

src/
  Resultant/                                     # Core library (net8.0, zero deps)
    Resultant.csproj
    IResult.cs                                   # IResult / IResult<T> interfaces
    Result.cs                                    # readonly record struct Result : IResult
    Result.T.cs                                  # readonly record struct Result<T> : IResult<T>
    ResultAsyncExtensions.cs                     # Extension methods on Task<Result<T>>
    ResultHelpers.cs                             # Combine / WhenAll helpers
    PagedResult.cs                               # PagedResult<T> for paginated collections
    Errors/
      ResultError.cs                             # abstract record ResultError(Message, Code)
      ValidationError.cs                         # sealed record + Property
      NotFoundError.cs                           # sealed record + Entity
      ConflictError.cs                           # sealed record
      UnauthorizedError.cs                       # sealed record
      ForbiddenError.cs                          # sealed record
      InfrastructureError.cs                     # sealed record + Exception? Inner

  Resultant.AspNetCore/                          # ASP.NET Core integration
  Resultant.Serialization.Json/                  # System.Text.Json converters
  Resultant.Serialization.Newtonsoft/            # Newtonsoft.Json converters
  Resultant.FluentValidation/                    # FluentValidation -> Result bridge
  Resultant.Analyzers/                           # Roslyn analyzers (netstandard2.0)
  Resultant.Generators/                          # Source generators (netstandard2.0)
  Resultant.MediatR/                             # MediatR validation pipeline
  Resultant.OpenTelemetry/                       # Telemetry / metrics integration

tests/
  Resultant.Tests/                               # Core library tests
  Resultant.AspNetCore.Tests/
  Resultant.Serialization.Json.Tests/
  Resultant.Serialization.Newtonsoft.Tests/
  Resultant.FluentValidation.Tests/
  Resultant.MediatR.Tests/
  Resultant.OpenTelemetry.Tests/

.github/workflows/
  alpha_package.yml                              # CI for feature/*/bugfix/* branches
  release_package.yml                            # CI for GitHub releases -> NuGet publish
```

## Architecture

### Core Types
- `Result` is a `readonly record struct` implementing `IResult` with `IsSuccess`, `IsFailure`, `Errors`, `FirstError` properties and static factory methods (`Ok()`, `Fail()`, `Try()`, `TryAsync()`)
- `Result<T>` is a `readonly record struct` implementing `IResult<T>` with a `Value` property and a full functional pipeline: `Map`, `Bind`, `Tap`, `Ensure`, `Match`, `Switch`, `Else` + async variants
- `ResultAsyncExtensions` provides extension methods on `Task<Result<T>>` enabling single-await async chaining
- `ResultHelpers` provides `Combine` and `WhenAll` for aggregating multiple results

### Error Hierarchy
- `ResultError` is an abstract record with `Message` (string) and `Code` (string)
- 6 sealed subtypes: `ValidationError`, `NotFoundError`, `ConflictError`, `UnauthorizedError`, `ForbiddenError`, `InfrastructureError`
- Pattern matching works naturally: `error switch { NotFoundError nf => ..., ValidationError v => ... }`

### Key Design Decisions
- **Struct-based** (zero heap allocation on success path)
- **`default(Result)` is failure** (bool defaults to false -- safe by default)
- **`IReadOnlyList<ResultError>`** instead of `IEnumerable` (prevents multiple enumeration)
- **LINQ query syntax** supported via `SelectMany` on `Result<T>`
- **No `implicit Result<T> -> T`** (dangerous with structs; replaced with `implicit T -> Result<T>`)

### Integration Packages
- **AspNetCore**: `ToActionResult()`, `ToMinimalApiResult()`, ProblemDetails mapping, `TranslateResultToActionResultFilter`
- **Serialization.Json**: System.Text.Json converters with polymorphic `$type` discriminator for error types
- **Serialization.Newtonsoft**: Newtonsoft.Json converters with same discriminator pattern
- **FluentValidation**: `ValidateToResult()` / `ValidateToResultAsync()` extension methods
- **MediatR**: `ValidationBehavior<TRequest, TResponse>` pipeline behavior
- **OpenTelemetry**: Activity span tags + metrics counters for result outcomes
- **Analyzers**: RES001 (Value without check), RES002 (Result ignored), RES003 (Errors on success)
- **Generators**: Source generator for custom error type boilerplate

## Code Conventions

- **Naming:** PascalCase for types, methods, properties. camelCase for locals/parameters.
- **Indentation:** 4 spaces
- **Namespace:** Core library types in `Resultant`. Integration packages in `Resultant.AspNetCore`, `Resultant.Serialization.Json`, etc. Tests in `*.Tests`.
- **No external dependencies** in the core `Resultant` package.
- **Test framework:** xUnit with `[Fact]` and `[Theory]`. Test naming: `MethodName_Condition_ShouldExpectedBehavior`.
- **Implicit usings** and **nullable** enabled via `Directory.Build.props`.
- **Central Package Management** via `Directory.Packages.props` -- never put Version on PackageReference in individual csproj files.

## Git & Branching Conventions

- **Default branch:** `master`
- **Branch naming:** `feature/*` and `bugfix/*` for development branches
- **Commit messages:** Present tense, imperative mood, max 72 characters
- **CI triggers:** `feature/*` and `bugfix/*` branches trigger alpha builds; GitHub releases trigger release builds

## CI/CD

Two GitHub Actions workflows:

1. **Alpha Package CI** (`alpha_package.yml`): Runs on push to `feature/*` and `bugfix/*`. Builds, tests, and packs with a timestamped alpha version suffix.
2. **Release Package CI** (`release_package.yml`): Runs on GitHub release creation. Builds in Release config, tests, packs with the git tag version, and publishes to NuGet.

Both use .NET 8.0.x SDK and require the `NUGET_API_KEY` secret for publishing.

## Key Patterns for Contributors

- All core types are `readonly record struct` -- no class inheritance. Use interfaces (`IResult`, `IResult<T>`) for polymorphism.
- New error types should extend `ResultError` as sealed records.
- Every public type and method needs corresponding xUnit tests.
- The core library targets `net8.0` with zero external dependencies.
- Integration packages only reference their specific dependency (e.g., `Resultant.MediatR` references only MediatR).
- Analyzers and Generators must target `netstandard2.0` (Roslyn requirement) with `ImplicitUsings=disable`.
