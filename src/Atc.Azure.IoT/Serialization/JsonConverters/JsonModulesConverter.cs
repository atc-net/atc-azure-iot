// ReSharper disable ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
namespace Atc.Azure.IoT.Serialization.JsonConverters;

/// <summary>
/// Implements a custom JSON converter for lists of <see cref="Models.Module"/> objects.
/// This converter reads a JSON object and converts its properties into a list of Module objects.
/// Each property in the JSON object is treated as a separate Module, with the property name being treated as the Module's Name.
/// Writing JSON from a list of Module objects is not supported by this converter.
/// </summary>
public sealed class JsonModulesConverter : JsonConverter<List<Models.Module>>
{
    public override List<Models.Module> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var result = new List<Models.Module>();

        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        foreach (var element in jsonDocument.RootElement.EnumerateObject())
        {
            var module = JsonSerializer.Deserialize<Models.Module>(element.Value.ToString(), options);
            if (module is null)
            {
                continue;
            }

            module = module with { Name = element.Name };
            result.Add(module!);
        }

        return result;
    }

    public override void Write(
        Utf8JsonWriter writer,
        List<Models.Module> value,
        JsonSerializerOptions options)
        => throw new NotSupportedException();
}