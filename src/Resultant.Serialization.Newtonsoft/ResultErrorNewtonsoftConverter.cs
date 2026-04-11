using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Resultant.Serialization.Newtonsoft;

public class ResultErrorNewtonsoftConverter : JsonConverter<ResultError>
{
    private const string TypeDiscriminator = "$type";

    public override ResultError? ReadJson(JsonReader reader, Type objectType, ResultError? existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var obj = JObject.Load(reader);
        var typeString = obj[TypeDiscriminator]?.ToString();
        var message = obj["message"]?.ToString() ?? "";
        var code = obj["code"]?.ToString() ?? "";

        return typeString switch
        {
            "validation" => new ValidationError(message, code, obj["property"]?.ToString() ?? ""),
            "notFound" => new NotFoundError(message, code, obj["entity"]?.ToString() ?? ""),
            "conflict" => new ConflictError(message, code),
            "unauthorized" => new UnauthorizedError(message, code),
            "forbidden" => new ForbiddenError(message, code),
            "infrastructure" => new InfrastructureError(message, code),
            _ => throw new JsonSerializationException($"Unknown error type: {typeString}")
        };
    }

    public override void WriteJson(JsonWriter writer, ResultError? value, JsonSerializer serializer)
    {
        if (value == null) { writer.WriteNull(); return; }

        writer.WriteStartObject();

        var typeName = value switch
        {
            ValidationError => "validation",
            NotFoundError => "notFound",
            ConflictError => "conflict",
            UnauthorizedError => "unauthorized",
            ForbiddenError => "forbidden",
            InfrastructureError => "infrastructure",
            _ => "unknown"
        };

        writer.WritePropertyName(TypeDiscriminator);
        writer.WriteValue(typeName);
        writer.WritePropertyName("message");
        writer.WriteValue(value.Message);
        writer.WritePropertyName("code");
        writer.WriteValue(value.Code);

        switch (value)
        {
            case ValidationError v:
                writer.WritePropertyName("property");
                writer.WriteValue(v.Property);
                break;
            case NotFoundError n:
                writer.WritePropertyName("entity");
                writer.WriteValue(n.Entity);
                break;
        }

        writer.WriteEndObject();
    }
}
