using Newtonsoft.Json;
using Resultant.Serialization.Newtonsoft;

namespace Resultant.Serialization.Newtonsoft.Tests;

public class NewtonsoftSerializationTests
{
    private readonly JsonSerializerSettings _settings;

    public NewtonsoftSerializationTests()
    {
        _settings = new JsonSerializerSettings().AddResultant();
    }

    [Fact]
    public void ResultError_ShouldRoundtrip()
    {
        ResultError error = new ConflictError("Conflict", "409");
        var json = JsonConvert.SerializeObject(error, _settings);
        var deserialized = JsonConvert.DeserializeObject<ResultError>(json, _settings);

        Assert.NotNull(deserialized);
        Assert.IsType<ConflictError>(deserialized);
        Assert.Equal("Conflict", deserialized!.Message);
    }

    [Fact]
    public void ValidationError_ShouldPreserveProperty()
    {
        ResultError error = new ValidationError("Required", "V", "Email");
        var json = JsonConvert.SerializeObject(error, _settings);
        var deserialized = JsonConvert.DeserializeObject<ResultError>(json, _settings);

        var ve = Assert.IsType<ValidationError>(deserialized);
        Assert.Equal("Email", ve.Property);
    }

    [Fact]
    public void NotFoundError_ShouldPreserveEntity()
    {
        ResultError error = new NotFoundError("Not found", "NF", "User");
        var json = JsonConvert.SerializeObject(error, _settings);
        var deserialized = JsonConvert.DeserializeObject<ResultError>(json, _settings);

        var nf = Assert.IsType<NotFoundError>(deserialized);
        Assert.Equal("User", nf.Entity);
    }

    [Fact]
    public void AllErrorTypes_ShouldRoundtrip()
    {
        ResultError[] errors =
        [
            new ValidationError("Bad", "V", "Email"),
            new NotFoundError("Missing", "NF", "User"),
            new ConflictError("Exists", "C"),
            new UnauthorizedError("No auth", "U"),
            new ForbiddenError("Denied", "F"),
            new InfrastructureError("Timeout", "I"),
        ];

        foreach (var error in errors)
        {
            var json = JsonConvert.SerializeObject(error, _settings);
            var deserialized = JsonConvert.DeserializeObject<ResultError>(json, _settings);

            Assert.NotNull(deserialized);
            Assert.Equal(error.GetType(), deserialized!.GetType());
            Assert.Equal(error.Message, deserialized.Message);
        }
    }
}
