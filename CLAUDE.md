# CLAUDE.md

## Project Overview

**Resultant** is a C# library implementing the Result pattern for .NET applications. It provides `Result`, `Result<T>`, `Error`, and `PagedResult<T>` types as a structured alternative to exception-based error handling. Published as a NuGet package targeting .NET Standard 2.0 for broad compatibility.

- **Repository:** adomorn/Resultant
- **License:** MIT
- **Current Version:** 1.0.2
- **Author:** Arda Terekeci

## Build & Test Commands

```bash
dotnet restore                              # Restore all dependencies
dotnet build --no-restore                   # Build the solution
dotnet test --no-build                      # Run all tests
dotnet test --no-build --verbosity normal   # Run tests with detailed output
dotnet pack --no-build --configuration Release  # Create NuGet package
```

The solution file is `Resultant.sln` at the repository root.

## Project Structure

```
Resultant.sln                    # Solution file (2 projects)
Resultant/                       # Library project (netstandard2.0)
  Resultant.csproj
  Result.cs                      # Non-generic Result with Ok/Fail factories
  Result.T.cs                    # Generic Result<T> with Map/Bind/async ops
  Error.cs                       # Error model (Message + Code)
  PagedResult.cs                 # PagedResult<T> for paginated collections
  ResultHelpers.cs               # Static helpers (Combine, WhenAll)
Resultant.Tests/                 # Test project (net8.0, xUnit)
  Resultant.Tests.csproj
  GlobalUsings.cs                # global using Xunit;
  ResultTests.cs                 # Tests for non-generic Result
  ResultOfTTests.cs              # Tests for Result<T>
  ErrorTests.cs                  # Tests for Error
  PagedResultTests.cs            # Tests for PagedResult<T>
  ResultHelpersTests.cs          # Tests for ResultHelpers
.github/workflows/
  alpha_package.yml              # CI for feature/*/bugfix/* branches
  release_package.yml            # CI for GitHub releases -> NuGet publish
```

## Architecture

- `Result` is the base class with `IsSuccess`, `IsFailure`, `Errors` properties and static factory methods (`Ok()`, `Fail()`)
- `Result<T>` extends `Result`, adding a `Value` property and functional operations: `Map`, `Bind`, `MapAsync`, `BindAsync`
- `Error` is a simple model with `Message` (string) and `Code` (int, defaults to 0)
- `PagedResult<T>` extends `Result<List<T>>`, adding pagination metadata (`CurrentPage`, `PageSize`, `TotalCount`, `TotalPages`)
- `ResultHelpers` provides `Combine(params Result[])` and `WhenAll(IEnumerable<Task<Result>>)` static methods
- Implicit operators: `Result` converts to `bool`; `Result<T>` converts to `T` (throws `InvalidOperationException` on failure)

## Code Conventions

- **Naming:** PascalCase for classes, methods, and properties. camelCase for local variables and parameters. Underscore prefix for private fields.
- **Indentation:** 4 spaces (no tabs in source files; csproj files use tabs)
- **Namespace:** All library code in the `Resultant` namespace. Tests in `Resultant.Tests`.
- **No external dependencies** in the library project -- it is pure .NET Standard 2.0.
- **Test framework:** xUnit with `[Fact]` and `[Theory]` attributes. Test naming: `MethodName_Condition_ShouldExpectedBehavior` (e.g., `Ok_ShouldReturnSuccessResult`, `Map_ShouldNotTransformOnFailure`).
- **Async tests** use `async Task` return type with `Task.FromResult` for test values.
- **Implicit usings** are enabled in the test project but not the library (library has explicit `using` statements).
- **Nullable** reference types are enabled in the test project.

## Git & Branching Conventions

- **Default branch:** `master`
- **Branch naming:** `feature/*` and `bugfix/*` for development branches
- **Commit messages:** Present tense, imperative mood, max 72 characters (e.g., "Add feature" not "Added feature")
- **CI triggers:** `feature/*` and `bugfix/*` branches trigger alpha builds; GitHub releases trigger release builds

## CI/CD

Two GitHub Actions workflows:

1. **Alpha Package CI** (`alpha_package.yml`): Runs on push to `feature/*` and `bugfix/*`. Builds, tests, and packs with a timestamped alpha version suffix.
2. **Release Package CI** (`release_package.yml`): Runs on GitHub release creation. Builds in Release config, tests, packs with the git tag version, and publishes to NuGet.

Both use .NET 8.0.x SDK and require the `NUGET_API_KEY` secret for publishing.

## Key Patterns for Contributors

- New result types should extend `Result` or `Result<T>` and follow the existing factory method pattern (static `Create`/`Ok`/`Fail` methods, private constructors)
- Every public type and method needs corresponding xUnit tests in the `Resultant.Tests` project
- The library targets `netstandard2.0` -- do not use APIs unavailable in .NET Standard 2.0
- Keep the library dependency-free
