
# Resultant

Resultant is a robust and flexible C# library designed for implementing the Result pattern, enhancing error handling in .NET applications. It offers a structured way to return success or error information, making your code more readable, maintainable, and less prone to errors.

## Features

- **Generic and Non-Generic Result Types**: Handle operations with and without return values.
- **Fluent API**: Chain operations for readability and efficiency.
- **Async Support**: Seamlessly integrate with async methods.
- **Error Handling**: Advanced error handling with messages, codes, and multiple errors.
- **Paged Results**: Special handling for operations returning paginated data.
- **Implicit Conversion Operators**: Simplify usage with intuitive type conversions.

## Getting Started

### Installation

Install Resultant via NuGet:

```shell
dotnet add package Resultant
```

### Basic Usage

#### Creating a Successful Result

```csharp
var successResult = Result.Ok();
var successResultWithValue = Result.Ok("Success value");
```

#### Creating a Failure Result

```csharp
var failResult = Result.Fail("Error message");
var failResultWithCode = Result.Fail(new Error("Error message", errorCode));
```

#### Working with Result

```csharp
public Result<string> GetData()
{
    if (someCondition)
        return Result.Fail("Error occurred");

    return Result.Ok("Data");
}

var result = GetData();
if (result.IsSuccess)
{
    Console.WriteLine(result.Value); // Use the data
}
else
{
    Console.WriteLine(result.Error); // Handle the error
}
```

#### Using Async Methods

```csharp
public async Task<Result<string>> GetDataAsync()
{
    // Async operation...
    return await Result.Ok("Async data");
}

// Usage
var result = await GetDataAsync();
```


#### Fluent API with Map and Bind

The `Map` and `Bind` methods provide a fluent way to transform and chain result operations.

- **Map**: Use this method to transform the value of a successful result. It doesn't execute if the result is a failure.

```csharp
public Result<int> ParseData(string data)
{
    if (int.TryParse(data, out var number))
        return Result.Ok(number);
    return Result.Fail("Invalid data");
}

var result = Result.Ok("123").Map(ParseData);
// If parsing succeeds, 'result' is a successful Result<int>
```

- **Bind**: Use this method to chain results, where each result depends on the previous one.

```csharp
public Result<string> FetchData(int id)
{
    // Fetch data logic...
    return Result.Ok("Fetched data");
}

public Result<string> ProcessData(string data)
{
    // Data processing logic...
    return Result.Ok("Processed data");
}

var result = Result.Ok(1)
    .Bind(FetchData)   // Fetch data with the id
    .Bind(ProcessData); // Then process the fetched data
// 'result' holds the final result of these chained operations
```


```csharp
public Result<int> ParseData(string data)
{
    if (int.TryParse(data, out var number))
        return Result.Ok(number);

    return Result.Fail("Invalid data");
}

var result = Result.Ok("123").Map(ParseData);
```

## Advanced Topics

### Handling Paged Results

```csharp
public PagedResult<Item> GetItems(int page, int pageSize)
{
    var items = FetchItems(page, pageSize); // Your logic to fetch items
    return PagedResult<Item>.Create(items, page, pageSize, totalItemCount);
}
```

### Combining Results

Use `Result.Combine` to aggregate multiple results into one.

### Error Handling

Customize error handling by using the `Error` class to include error codes and detailed messages.

## Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

Check out our [contributing guidelines](https://github.com/adomorn/Resultant/blob/master/CONTRIBUTING.md) for more information.

## License

Distributed under the MIT License. See [LICENSE](https://github.com/adomorn/Resultant/blob/master/LICENSE) for more information.

## Contact

Arda Terekeci - [@ardaterekeci](https://www.linkedin.com/in/ardaterekeci/)

Project Link: [https://github.com/adomorn/Resultant](https://github.com/adomorn/Resultant)
