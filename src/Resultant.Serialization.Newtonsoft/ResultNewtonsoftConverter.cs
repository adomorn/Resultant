using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Resultant.Serialization.Newtonsoft;

public class ResultNewtonsoftConverter : JsonConverter
{
    public override bool CanConvert(Type objectType) => objectType == typeof(Result);

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        var isSuccess = obj["isSuccess"]?.ToObject<bool>() ?? false;

        if (isSuccess)
            return Result.Ok();

        var errors = new List<ResultError>();
        var errorsArray = obj["errors"] as JArray;
        if (errorsArray != null)
        {
            var errorConverter = new ResultErrorNewtonsoftConverter();
            foreach (var errorToken in errorsArray)
            {
                using var errorReader = errorToken.CreateReader();
                errorReader.Read();
                var error = errorConverter.ReadJson(errorReader, typeof(ResultError), null, false, serializer);
                if (error != null) errors.Add(error);
            }
        }

        return Result.Fail(errors);
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null) { writer.WriteNull(); return; }

        var result = (Result)value;
        var errorConverter = new ResultErrorNewtonsoftConverter();

        writer.WriteStartObject();
        writer.WritePropertyName("isSuccess");
        writer.WriteValue(result.IsSuccess);

        if (result.IsFailure)
        {
            writer.WritePropertyName("errors");
            writer.WriteStartArray();
            foreach (var error in result.Errors)
            {
                errorConverter.WriteJson(writer, error, serializer);
            }
            writer.WriteEndArray();
        }

        writer.WriteEndObject();
    }
}
