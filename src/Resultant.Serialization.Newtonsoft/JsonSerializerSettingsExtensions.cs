using Newtonsoft.Json;

namespace Resultant.Serialization.Newtonsoft;

public static class JsonSerializerSettingsExtensions
{
    public static JsonSerializerSettings AddResultant(this JsonSerializerSettings settings)
    {
        settings.Converters.Add(new ResultNewtonsoftConverter());
        settings.Converters.Add(new ResultErrorNewtonsoftConverter());
        return settings;
    }
}
