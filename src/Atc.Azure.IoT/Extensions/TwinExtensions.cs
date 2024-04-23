namespace Atc.Azure.IoT.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="Twin"/> class.
/// These methods enable the easy extraction of desired and reported properties
/// as strongly typed objects using custom JSON serialization settings.
/// </summary>
public static class TwinExtensions
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = JsonSerializerOptionsFactory.Create(
        new JsonSerializerFactorySettings
        {
            UseConverterDatetimeOffsetMinToNull = true,
        });

    static TwinExtensions()
    {
        JsonSerializerOptions.Converters.Add(new JsonModulesConverter());
    }

    public static T GetDesiredProperties<T>(
        this Twin twin)
        => JsonSerializer.Deserialize<T>(
            twin.Properties.Desired.ToJson(),
            JsonSerializerOptions)!;

    public static T GetReportedProperties<T>(
        this Twin twin)
        => JsonSerializer.Deserialize<T>(
            twin.Properties.Reported.ToJson(),
            JsonSerializerOptions)!;
}