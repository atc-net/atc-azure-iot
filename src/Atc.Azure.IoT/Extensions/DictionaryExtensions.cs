namespace Atc.Azure.IoT.Extensions;

public static class DictionaryExtensions
{
    /// <summary>
    /// Converts a dictionary of properties into a TwinCollection.
    /// The keys in the dictionary should use a dot (.) as the grouping character to denote the hierarchy of properties.
    /// </summary>
    /// <param name="dictionary">The dictionary containing the properties to be converted.</param>
    /// <example>
    /// As an example, a key-value pair of "Group.Property": "value" in the dictionary will be translated to the following JSON structure in the TwinCollection:
    /// {
    ///     "Group":
    ///     {
    ///         "Property": "value"
    ///     }
    /// }
    /// </example>
    /// <returns>A TwinCollection that represents the hierarchical structure of the properties in the dictionary, or null if the dictionary is null or empty.</returns>
    public static TwinCollection? ToTwinCollection(
        this Dictionary<string, string>? dictionary)
    {
        if (dictionary is null ||
            !dictionary.Any())
        {
            return null;
        }

        var dynamicJson = new DynamicJson();
        foreach (var property in dictionary)
        {
            dynamicJson.SetValue($"{property.Key}", property.Value);
        }

        return new TwinCollection(dynamicJson.ToJson());
    }
}