using System.Text.Json;
using Resultant.Serialization.Json;

namespace Resultant.Serialization.Json.Tests;

public class JsonSerializationTests
{
    private readonly JsonSerializerOptions _options = JsonSerializerOptionsExtensions.CreateResultantOptions();

    [Fact]
    public void Result_Success_ShouldRoundtrip()
    {
        var result = Result.Ok();
        var json = JsonSerializer.Serialize(result, _options);
        var deserialized = JsonSerializer.Deserialize<Result>(json, _options);

        Assert.True(deserialized.IsSuccess);
    }

    [Fact]
    public void Result_Failure_ShouldRoundtrip()
    {
        var result = Result.Fail(new ConflictError("Error", "409"));
        var json = JsonSerializer.Serialize(result, _options);
        var deserialized = JsonSerializer.Deserialize<Result>(json, _options);

        Assert.False(deserialized.IsSuccess);
        Assert.Single(deserialized.Errors);
        Assert.IsType<ConflictError>(deserialized.Errors[0]);
        Assert.Equal("Error", deserialized.Errors[0].Message);
    }

    [Fact]
    public void ResultT_Success_ShouldRoundtrip()
    {
        var result = Result.Ok(42);
        var json = JsonSerializer.Serialize(result, _options);
        var deserialized = JsonSerializer.Deserialize<Result<int>>(json, _options);

        Assert.True(deserialized.IsSuccess);
        Assert.Equal(42, deserialized.Value);
    }

    [Fact]
    public void ResultT_Failure_ShouldRoundtrip()
    {
        var result = Result.Fail<int>(new NotFoundError("Not found", "404", "User"));
        var json = JsonSerializer.Serialize(result, _options);
        var deserialized = JsonSerializer.Deserialize<Result<int>>(json, _options);

        Assert.False(deserialized.IsSuccess);
        var error = Assert.IsType<NotFoundError>(deserialized.Errors[0]);
        Assert.Equal("User", error.Entity);
    }

    [Fact]
    public void ValidationError_ShouldPreserveProperty()
    {
        var result = Result.Fail<string>(new ValidationError("Required", "V", "Email"));
        var json = JsonSerializer.Serialize(result, _options);
        var deserialized = JsonSerializer.Deserialize<Result<string>>(json, _options);

        var error = Assert.IsType<ValidationError>(deserialized.Errors[0]);
        Assert.Equal("Email", error.Property);
    }

    [Fact]
    public void MultipleErrors_ShouldRoundtrip()
    {
        ResultError[] errors = [
            new ValidationError("Bad email", "V", "Email"),
            new NotFoundError("Not found", "NF", "User")
        ];
        var result = Result.Fail<int>(errors);
        var json = JsonSerializer.Serialize(result, _options);
        var deserialized = JsonSerializer.Deserialize<Result<int>>(json, _options);

        Assert.Equal(2, deserialized.Errors.Count);
        Assert.IsType<ValidationError>(deserialized.Errors[0]);
        Assert.IsType<NotFoundError>(deserialized.Errors[1]);
    }

    [Fact]
    public void ResultT_WithComplexValue_ShouldRoundtrip()
    {
        var result = Result.Ok(new TestDto { Name = "Test", Age = 25 });
        var json = JsonSerializer.Serialize(result, _options);
        var deserialized = JsonSerializer.Deserialize<Result<TestDto>>(json, _options);

        Assert.True(deserialized.IsSuccess);
        Assert.Equal("Test", deserialized.Value.Name);
        Assert.Equal(25, deserialized.Value.Age);
    }

    private class TestDto
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }
}
