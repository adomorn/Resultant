
# Resultant

A robust and flexible C# library for implementing the Result pattern, designed to enhance error handling in your .NET applications.

## About Resultant

Resultant is a C# library targeting `netstandard2.0`, offering an implementation of the Result pattern. This pattern is an alternative to exception-based error handling, providing a way to return success or error information from methods in a clear and consistent manner. It's especially useful in complex applications where you need to handle multiple error conditions gracefully.

### Key Features

- **Generic and Non-Generic Result Types**: Handle operations that return values and those that don't.
- **Fluent API**: Chain operations with `Map`, `Bind`, `MapAsync`, and `BindAsync` methods.
- **Error Handling**: Support for error messages, codes, and multiple error aggregation.
- **Async Support**: Seamlessly integrate with asynchronous methods.
- **Paged Results**: Special handling for paginated data.
- **Implicit Conversion Operators**: For improved usability.

## Getting Started

### Installation

Resultant is available as a NuGet package. You can install it using the NuGet Package Manager or the .NET CLI:

```shell
dotnet add package Resultant
```

### Basic Usage

Here's a quick example of how to use the Resultant library:

```csharp
public Result<string> ProcessData(string input)
{
    if (string.IsNullOrEmpty(input))
        return Result.Fail("Input cannot be empty");

    // Process the data...
    return Result.Ok("Processed data");
}
```

For more detailed documentation, please visit our [Wiki](https://github.com/adomorn/Resultant/wiki).

## Contributing

We welcome contributions to Resultant! Please read our [Contributing Guidelines](https://github.com/adomorn/Resultant/blob/master/CONTRIBUTING.md) for more information on how to get involved.
