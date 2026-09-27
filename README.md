# Resultant

A C# library for returning explicit success or failure results, with typed values, error aggregation, and synchronous or asynchronous composition. The library targets .NET Standard 2.0; repository tests target .NET 8.

## Install

```sh
dotnet add package Resultant
```

## Return and inspect a result

```csharp
using Resultant;

static Result<int> ParseNumber(string text)
{
    return int.TryParse(text, out var value)
        ? Result.Ok(value)
        : Result.Fail<int>(new[] { new Error("Expected an integer", 400) });
}

var result = ParseNumber("42");
if (result.IsSuccess)
{
    Console.WriteLine(result.Value);
}
else
{
    foreach (var error in result.Errors)
        Console.WriteLine($"{error.Code}: {error.Message}");
}
```

For an operation without a return value, use `Result.Ok()` or `Result.Fail("message", code: 400)`. Typed failures take a collection of `Error` instances through `Result.Fail<T>(errors)`.

Check `IsSuccess` before reading `Value`. A failed result's `Value` is the type's default value; implicitly converting a failed result to its value type throws `InvalidOperationException`.

## Compose operations

`Map` transforms a successful value. `Bind` chains a function that itself returns a result. Both propagate an existing failure without invoking the supplied function.

```csharp
var doubled = Result.Ok(21).Map(value => value * 2);
var parsed = Result.Ok("42").Bind(ParseNumber);
```

Use `MapAsync` for an asynchronous value transformation and `BindAsync` for an asynchronous function returning a result:

```csharp
var doubledAsync = await Result.Ok(21)
    .MapAsync(value => Task.FromResult(value * 2));

var parsedAsync = await Result.Ok("42")
    .BindAsync(text => Task.FromResult(ParseNumber(text)));
```

The tasks above illustrate the signatures. In an application, pass the actual asynchronous operation. `Result<T>` itself is not awaitable, and exceptions thrown by callbacks are not automatically converted into failed results.

## Aggregate errors and paged values

```csharp
var combined = ResultHelpers.Combine(
    Result.Ok(),
    Result.Fail("Validation failed", code: 400));

var page = PagedResult<string>.Create(
    new List<string> { "first", "second" },
    currentPage: 1,
    pageSize: 20,
    totalCount: 2);
```

`ResultHelpers.WhenAll` awaits a collection of `Task<Result>` and combines the results. Supply valid pagination values when creating a `PagedResult<T>`; the current implementation does not validate them.

## Build and test

With the .NET 8 SDK installed:

```sh
dotnet restore Resultant.sln
dotnet build Resultant.sln --configuration Release --no-restore
dotnet test Resultant.sln --configuration Release --no-build --no-restore
```

See [CONTRIBUTING.md](CONTRIBUTING.md) for contribution guidance and [SECURITY.md](SECURITY.md) for security reporting.

## License

[MIT](LICENSE.txt).
