using System.Text.Json;
using System.Text.Json.Serialization;
using Api.Converters;

namespace Api.Configurations;

public static class ControllersConfiguration
{
    public static void Configure(IServiceCollection services)
    {
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new OptionalJsonConverterFactory());
            });
    }
}
