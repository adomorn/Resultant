using System.Text.Json;

namespace Resultant.Serialization.Json;

public static class JsonSerializerOptionsExtensions
{
    public static JsonSerializerOptions AddResultant(this JsonSerializerOptions options)
    {
        options.Converters.Add(new ResultJsonConverter());
        options.Converters.Add(new ResultOfTJsonConverterFactory());
        options.Converters.Add(new ResultErrorJsonConverter());
        return options;
    }

    public static JsonSerializerOptions CreateResultantOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.AddResultant();
        return options;
    }
}
