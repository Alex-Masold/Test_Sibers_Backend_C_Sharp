using System.Text.Json;
using System.Text.Json.Serialization;
using Domain.Common;

namespace Api.Converters;

public class OptionalJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType
            && typeToConvert.GetGenericTypeDefinition() == typeof(Optional<>);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var innerType = typeToConvert.GetGenericArguments()[0];

        var converterType = typeof(OptionalJsonConverter<>).MakeGenericType(innerType);

        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }
}
