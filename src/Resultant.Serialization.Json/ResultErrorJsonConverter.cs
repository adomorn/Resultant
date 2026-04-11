using System.Text.Json;
using System.Text.Json.Serialization;

namespace Resultant.Serialization.Json;

public class ResultErrorJsonConverter : JsonConverter<ResultError>
{
    private const string TypeDiscriminator = "$type";

    public override ResultError? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty(TypeDiscriminator, out var typeProp))
            throw new JsonException($"Missing '{TypeDiscriminator}' discriminator property.");

        var typeString = typeProp.GetString();
        var message = root.GetProperty("message").GetString() ?? "";
        var code = root.GetProperty("code").GetString() ?? "";

        return typeString switch
        {
            "validation" => new ValidationError(
                message, code,
                root.GetProperty("property").GetString() ?? ""),
            "notFound" => new NotFoundError(
                message, code,
                root.GetProperty("entity").GetString() ?? ""),
            "conflict" => new ConflictError(message, code),
            "unauthorized" => new UnauthorizedError(message, code),
            "forbidden" => new ForbiddenError(message, code),
            "infrastructure" => new InfrastructureError(message, code),
            _ => throw new JsonException($"Unknown error type: {typeString}")
        };
    }

    public override void Write(Utf8JsonWriter writer, ResultError value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        var (typeName, writeExtra) = value switch
        {
            ValidationError v => ("validation", (Action)(() =>
            {
                writer.WriteString("property", v.Property);
            })),
            NotFoundError n => ("notFound", (Action)(() =>
            {
                writer.WriteString("entity", n.Entity);
            })),
            ConflictError => ("conflict", (Action)(() => { })),
            UnauthorizedError => ("unauthorized", (Action)(() => { })),
            ForbiddenError => ("forbidden", (Action)(() => { })),
            InfrastructureError => ("infrastructure", (Action)(() => { })),
            _ => ("unknown", (Action)(() => { }))
        };

        writer.WriteString(TypeDiscriminator, typeName);
        writer.WriteString("message", value.Message);
        writer.WriteString("code", value.Code);
        writeExtra();

        writer.WriteEndObject();
    }
}
