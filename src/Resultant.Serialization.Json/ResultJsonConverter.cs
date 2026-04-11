using System.Text.Json;
using System.Text.Json.Serialization;

namespace Resultant.Serialization.Json;

public class ResultJsonConverter : JsonConverter<Result>
{
    public override Result Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var isSuccess = root.GetProperty("isSuccess").GetBoolean();

        if (isSuccess)
            return Result.Ok();

        var errorConverter = new ResultErrorJsonConverter();
        var errors = new List<ResultError>();

        if (root.TryGetProperty("errors", out var errorsElement))
        {
            foreach (var errorElement in errorsElement.EnumerateArray())
            {
                var errorJson = errorElement.GetRawText();
                var readerInner = new Utf8JsonReader(System.Text.Encoding.UTF8.GetBytes(errorJson));
                readerInner.Read();
                var error = errorConverter.Read(ref readerInner, typeof(ResultError), options);
                if (error != null) errors.Add(error);
            }
        }

        return Result.Fail(errors);
    }

    public override void Write(Utf8JsonWriter writer, Result value, JsonSerializerOptions options)
    {
        var errorConverter = new ResultErrorJsonConverter();

        writer.WriteStartObject();
        writer.WriteBoolean("isSuccess", value.IsSuccess);

        if (value.IsFailure)
        {
            writer.WriteStartArray("errors");
            foreach (var error in value.Errors)
            {
                errorConverter.Write(writer, error, options);
            }
            writer.WriteEndArray();
        }

        writer.WriteEndObject();
    }
}
