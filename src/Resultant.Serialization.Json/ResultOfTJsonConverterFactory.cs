using System.Text.Json;
using System.Text.Json.Serialization;

namespace Resultant.Serialization.Json;

public class ResultOfTJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) =>
        typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Result<>);

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(ResultOfTJsonConverter<>).MakeGenericType(valueType);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

internal class ResultOfTJsonConverter<T> : JsonConverter<Result<T>>
{
    public override Result<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        var isSuccess = root.GetProperty("isSuccess").GetBoolean();

        if (isSuccess)
        {
            var value = JsonSerializer.Deserialize<T>(root.GetProperty("value").GetRawText(), options)!;
            return Result.Ok(value);
        }

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

        return Result.Fail<T>(errors);
    }

    public override void Write(Utf8JsonWriter writer, Result<T> value, JsonSerializerOptions options)
    {
        var errorConverter = new ResultErrorJsonConverter();

        writer.WriteStartObject();
        writer.WriteBoolean("isSuccess", value.IsSuccess);

        if (value.IsSuccess)
        {
            writer.WritePropertyName("value");
            JsonSerializer.Serialize(writer, value.Value, options);
        }
        else
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
