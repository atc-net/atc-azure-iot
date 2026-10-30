namespace Atc.Azure.IoT.Extensions;

public static class IotHubExceptionExtensions
{
    /// <summary>
    /// Describes the exception as a JSON object with its innermost message and its error code, for example to return
    /// it as the payload of a direct method response.
    /// </summary>
    /// <param name="ex">The IoT Hub exception.</param>
    /// <returns>The JSON object, for example <c>{"message":"...","errorCode":"DeviceNotOnline"}</c>.</returns>
    public static string ToJsonPayload(this IotHubException ex)
    {
        ArgumentNullException.ThrowIfNull(ex);

        return JsonSerializer.Serialize(new Dictionary<string, string?>(StringComparer.Ordinal)
        {
            ["message"] = ex.GetLastInnerMessage(),
            ["errorCode"] = ex.Code.ToString(),
        });
    }
}